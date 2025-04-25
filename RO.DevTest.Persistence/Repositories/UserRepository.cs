using RO.DevTest.Domain.Entities;
using Microsoft.EntityFrameworkCore; // <<-- Garanta que este using está presente
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RO.DevTest.Persistence; // Necessário para DefaultContext
using RO.DevTest.Application.Contracts.Persistence.Repositories;  // Importando a interface IUserRepository

namespace RO.DevTest.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly DefaultContext _context;

        public UserRepository(DefaultContext context) : base(context)
        {
            _context = context;
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

        // Outros métodos do repositório aqui...
    }
}
