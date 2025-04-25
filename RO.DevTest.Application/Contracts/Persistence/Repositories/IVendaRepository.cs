using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Contracts.Persistence.Repositories
{
    public interface IVendaRepository
    {
        Task<List<Venda>> GetAllAsync(int pageNumber, int pageSize);
        Task<Venda?> GetByIdAsync(Guid id);
        Task<int> CountAsync();
        Task DeleteAsync(Venda venda);
        Task CreateAsync(Venda venda); // Adicionado para suportar criação de vendas
    }
}