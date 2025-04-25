using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RO.DevTest.Persistence.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DefaultContext _context;

        // Construtor com injeção de dependência do DefaultContext
        public ClienteRepository(DefaultContext context)
        {
            _context = context;
        }

        // Método para adicionar um cliente (retorna o cliente adicionado)
        public async Task<Cliente> AddAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
            return cliente; // Retorna o cliente adicionado
        }

        // Método para obter um cliente por ID
        public async Task<Cliente?> GetByIdAsync(Guid id)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id); // Pode retornar null se não encontrar o cliente
        }

        // Método para obter todos os clientes
        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _context.Clientes.ToListAsync(); // Retorna todos os clientes
        }

        // Método para atualizar um cliente
        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente); // Atualiza o cliente
            await _context.SaveChangesAsync();
        }

        // Método para deletar um cliente
        public async Task DeleteAsync(Guid id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente); // Remove o cliente
                await _context.SaveChangesAsync();
            }
        }
    }
}
