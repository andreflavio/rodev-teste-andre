namespace RO.DevTest.Application.Contracts.Persistence.Repositories

{
    public interface IClienteRepository
    {
        Task<Domain.Entities.Cliente> AddAsync(Domain.Entities.Cliente cliente);
        Task<Domain.Entities.Cliente?> GetByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Cliente>> GetAllAsync();
        Task UpdateAsync(Domain.Entities.Cliente cliente);
        Task DeleteAsync(Guid id);
        // Outros métodos conforme necessário
    }
}