using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RO.DevTest.Persistence.Repositories
{
    public class VendaRepository : BaseRepository<Venda>, IVendaRepository
    {
        private readonly DefaultContext _context;

        public VendaRepository(DefaultContext context) : base(context)
        {
            _context = context;
        }

        // Implementação de CountAsync
        public async Task<int> CountAsync()
        {
            return await _context.Vendas.CountAsync();
        }

        // Implementação de DeleteAsync
        public async Task DeleteAsync(Venda vendaExistente)
        {
            _context.Vendas.Remove(vendaExistente);  // Remove a venda da DbSet
            await _context.SaveChangesAsync();  // Salva as alterações no banco
        }

        // Implementação de GetByIdAsync
        public async Task<Venda?> GetByIdAsync(Guid id)
        {
            return await _context.Vendas
                .Include(v => v.Itens) // Inclui os itens da venda, se necessário
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        // Implementação de GetVendasPaginated
        public async Task<List<Venda>> GetVendasPaginated(int page, int pageSize)
        {
            return await _context.Vendas
                .Skip((page - 1) * pageSize)  // Pular as vendas anteriores à página solicitada
                .Take(pageSize)  // Limitar a quantidade de vendas para a página
                .ToListAsync();
        }
    }
}
