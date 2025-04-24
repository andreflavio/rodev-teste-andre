using RO.DevTest.Domain.Entities;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Persistence.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly DefaultContext _context;

        public ProdutoRepository(DefaultContext context)
        {
            _context = context;
        }

        // Retorna um IQueryable para permitir composições de query externas
        public IQueryable<Produto> Query()
        {
            return _context.Produtos.AsQueryable();
        }

        public async Task<Produto> CreateAsync(Produto entity, CancellationToken cancellationToken = default)
        {
            await _context.Produtos.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public void Delete(Produto entity)
        {
            _context.Produtos.Remove(entity);
            _context.SaveChanges();
        }

        // Remover a duplicação aqui: 
        public async Task DeleteAsync(Guid id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
        }

        public Produto? Get(Expression<Func<Produto, bool>> predicate, params Expression<Func<Produto, object>>[] includes)
        {
            IQueryable<Produto> query = _context.Produtos.Where(predicate);

            foreach (var includeProperty in includes)
            {
                query = query.Include(includeProperty);
            }

            return query.FirstOrDefault();
        }

        public async Task<List<Produto>> GetAllAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<Produto> GetByIdAsync(Guid id)
        {
            return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public void Update(Produto entity)
        {
            _context.Produtos.Update(entity);
            _context.SaveChanges();
        }

        public async Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
    }
}
