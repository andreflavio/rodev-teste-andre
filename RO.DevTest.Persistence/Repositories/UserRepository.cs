using RO.DevTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RO.DevTest.Persistence;
using RO.DevTest.Application.Contracts.Persistence.Repositories;

namespace RO.DevTest.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly DefaultContext _context;

        public UserRepository(DefaultContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Normalizar os campos manualmente para evitar NULL
            user.NormalizedUserName = user.UserName?.ToUpper() ?? throw new ArgumentException("UserName não pode ser nulo.", nameof(user.UserName));
            user.NormalizedEmail = user.Email?.ToUpper() ?? throw new ArgumentException("Email não pode ser nulo.", nameof(user.Email));

            // Log do valor de Role antes de salvar
            Console.WriteLine($"[DEBUG] Role antes de adicionar ao contexto: {user.Role}");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Log do valor de Role após salvar
            Console.WriteLine($"[DEBUG] Role após salvar no banco: {user.Role}");

            // Log dos campos normalizados
            Console.WriteLine($"[DEBUG] NormalizedUserName: {user.NormalizedUserName}, NormalizedEmail: {user.NormalizedEmail}");
        }

        public async Task<IEnumerable<User>> GetAllAsync(string? name = null, string? userName = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.Name != null && EF.Functions.ILike(u.Name, $"%{name}%"));
            }

            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(u => u.UserName != null && EF.Functions.ILike(u.UserName, $"%{userName}%"));
            }

            return await query.ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O email não pode ser nulo ou vazio.", nameof(email));

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}