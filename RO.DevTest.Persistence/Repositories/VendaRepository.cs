using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<int> CountAsync()
        {
            return await _context.Vendas.CountAsync();
        }

        public async Task DeleteAsync(Venda vendaExistente)
        {
            _context.Vendas.Remove(vendaExistente);
            await _context.SaveChangesAsync();
        }

        public async Task<Venda?> GetByIdAsync(Guid id)
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Venda>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}