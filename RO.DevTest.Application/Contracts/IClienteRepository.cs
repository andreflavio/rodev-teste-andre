using RO.DevTest.Domain.Entities;
using RO.DevTest.Application.Contracts.Persistance.Repositories; // Namespace correto para IBaseRepository
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace RO.DevTest.Application.Contracts
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
        Task<Cliente> AddAsync(Cliente cliente);
        Task<Cliente?> GetByIdAsync(Guid id);
        Task<List<Cliente>> GetAllAsync();
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(Guid id);
        // Outros métodos conforme necessário
    }
}