// No arquivo VendaRepository.cs

using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RO.DevTest.Persistence.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly DefaultContext _context;

        public VendaRepository(DefaultContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Venda>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .OrderBy(v => v.DataVenda)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Venda?> GetByIdAsync(Guid id)
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Vendas.CountAsync();
        }

        public async Task DeleteAsync(Venda venda)
        {
            _context.Vendas.Remove(venda);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(Venda venda)
        {
            await _context.Vendas.AddAsync(venda);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Venda>> GetSalesByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Vendas
                .Where(v => v.DataVenda >= startDate && v.DataVenda <= endDate)
                .Include(v => v.Itens)
                    .ThenInclude(item => item.Produto)
                .ToListAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<Venda, bool>> predicate)
        {
            // Implemente conforme sua necessidade, por exemplo:
            return _context.Vendas.AnyAsync(predicate);
        }
    }
}
