using RO.DevTest.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Contracts.Persistence.Repositories
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Task<List<Produto>> GetAllAsync();
        Task<Produto> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        IQueryable<Produto> Query();
        Task UpdateAsync(Produto produto);
    }
}