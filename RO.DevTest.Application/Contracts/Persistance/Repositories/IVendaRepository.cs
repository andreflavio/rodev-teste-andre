using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories
{
    public interface IVendaRepository : IBaseRepository<Venda>
    {
        Task<int> CountAsync();
        Task DeleteAsync(Venda vendaExistente);
        Task<Venda?> GetByIdAsync(Guid id);// Métodos específicos podem ser definidos aqui
        Task<List<Venda>> GetVendasPaginated(int page, int pageSize);

    }
}

