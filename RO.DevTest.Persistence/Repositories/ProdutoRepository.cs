using RO.DevTest.Domain.Entities;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RO.DevTest.Persistence.Repositories
{
    public class ProdutoRepository : IProdutoRepository  // Implementando a interface IProdutoRepository
    {
        private readonly DefaultContext _context;

        public ProdutoRepository(DefaultContext context)
        {
            _context = context;
        }

        // Método para criar um produto
        public async Task<Produto> CreateAsync(Produto entity, CancellationToken cancellationToken = default)
        {
            await _context.Produtos.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        // Método para excluir um produto
        public void Delete(Produto entity)
        {
            _context.Produtos.Remove(entity);
            _context.SaveChanges();
        }

        // Método para buscar produtos com base em uma expressão (síncrono)
        public Produto? Get(Expression<Func<Produto, bool>> predicate, params Expression<Func<Produto, object>>[] includes)
        {
            IQueryable<Produto> query = _context.Produtos.Where(predicate);

            // Incluir propriedades relacionadas
            foreach (var includeProperty in includes)
            {
                query = query.Include(includeProperty);
            }

            return query.FirstOrDefault();  // Retorna um único produto ou null
        }

        // Método para atualizar um produto
        public void Update(Produto entity)
        {
            _context.Produtos.Update(entity);
            _context.SaveChanges();
        }

        // Método para buscar todos os produtos
        public async Task<List<Produto>> GetAllAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        // Método para buscar um produto pelo ID
        public async Task<Produto> GetByIdAsync(Guid id)
        {
            return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        }

        // Método assíncrono para deletar um produto
        public async Task DeleteAsync(Guid id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
