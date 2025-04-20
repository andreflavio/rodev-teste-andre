// No arquivo UserRepository.cs
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using Microsoft.EntityFrameworkCore; // <<-- Garanta que este using está presente
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RO.DevTest.Persistence; // Necessário para DefaultContext

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
                // ** CORREÇÃO AQUI: Usar EF.Functions.ILike para busca case-insensitive no banco **
                // O segundo parâmetro é o padrão de busca com coringas: % para qualquer sequência, _ para um único caractere.
                // name + "%"  => começa com 'name'
                // "%" + name  => termina com 'name'
                // "%" + name + "%" => contém 'name' (o que .Contains faz)
                query = query.Where(u => u.Name != null && EF.Functions.ILike(u.Name, $"%{name}%"));
            }

            if (!string.IsNullOrEmpty(userName))
            {
                // ** CORREÇÃO AQUI: Usar EF.Functions.ILike para busca case-insensitive no banco **
                // Usando "%" + userName + "%" para replicar o comportamento de Contains
                query = query.Where(u => u.UserName != null && EF.Functions.ILike(u.UserName, $"%{userName}%"));
            }
            // Adicione lógica de filtro para outros campos aqui usando EF.Functions.ILike se precisar de case-insensitive "contém" ou "começa com", etc.
            // Para igualdade case-insensitive, string.Equals(..., StringComparison.OrdinalIgnoreCase) geralmente funciona com EF Core mais recente.

            return await query.ToListAsync();
        }

        // ... outros métodos ...
    }
}