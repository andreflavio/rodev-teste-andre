using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        // Método para obter todos os produtos
        Task<List<Produto>> GetAllAsync();

        // Método para buscar um produto pelo ID
        Task<Produto> GetByIdAsync(Guid id);

        // Método para deletar um produto
        Task DeleteAsync(Guid id);  // Implementação assíncrona do delete

        // Novo método para consultas com IQueryable (necessário para filtros, ordenação e paginação)
        IQueryable<Produto> Query();
    }
}
