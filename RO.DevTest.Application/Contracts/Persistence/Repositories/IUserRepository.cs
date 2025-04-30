using RO.DevTest.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Contracts.Persistence.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task AddAsync(User user);
        Task<IEnumerable<User>> GetAllAsync(string? name = null, string? userName = null);
        Task<User?> GetByEmailAsync(string email);
    }
}