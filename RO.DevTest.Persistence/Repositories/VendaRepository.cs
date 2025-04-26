// No arquivo VendaRepository.cs (substitua o conteúdo atual por este)

// Certifique-se de que todos estes usings estão no topo do arquivo:
using Microsoft.EntityFrameworkCore; // Necessário para Include, ToListAsync, FirstOrDefaultAsync, CountAsync, Remove
using RO.DevTest.Application.Contracts.Persistence.Repositories; // Para a interface IVendaRepository
using RO.DevTest.Domain.Entities; // Para a entidade Venda
using RO.DevTest.Persistence; // Assumindo que seu DbContext (DefaultContext) está aqui
using System; // Necessário para Guid, DateTime, ArgumentNullException
using System.Collections.Generic; // Necessário para List
using System.Linq; // Necessário para Where, OrderBy, Skip, Take, Select
using System.Threading.Tasks; // Necessário para Task, async/await


namespace RO.DevTest.Persistence.Repositories; // Ajuste o namespace conforme seu projeto

// A sua classe VendaRepository implementa a interface IVendaRepository
// Se sua interface IVendaRepository herda de uma BaseRepository, ajuste a herança aqui também se necessário.
public class VendaRepository : IVendaRepository
{
    // Certifique-se de que o tipo do seu DbContext (DefaultContext) está correto
    private readonly DefaultContext _context;

    // O construtor deve injetar o DbContext
    public VendaRepository(DefaultContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // >>> Seus métodos originais <<<

    public async Task<List<Venda>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _context.Vendas // Certifique-se que _context.Vendas é o DbSet<Venda> no seu DbContext
            .Include(v => v.Itens) // Inclui os itens da venda
            .OrderBy(v => v.DataVenda) // Ordena por data (para paginação)
            .Skip((pageNumber - 1) * pageSize) // Pula os itens das páginas anteriores
            .Take(pageSize) // Pega o número de itens da página atual
            .ToListAsync(); // Executa a consulta assíncronamente
    }

    public async Task<Venda?> GetByIdAsync(Guid id)
    {
        return await _context.Vendas // Certifique-se que _context.Vendas é o DbSet<Venda>
            .Include(v => v.Itens) // Inclui os itens da venda
            .FirstOrDefaultAsync(v => v.Id == id); // Busca pelo ID
    }

    public async Task<int> CountAsync()
    {
        return await _context.Vendas.CountAsync(); // Conta o total de vendas
    }

    public async Task DeleteAsync(Venda venda)
    {
        _context.Vendas.Remove(venda); // Remove a venda do contexto
        await _context.SaveChangesAsync(); // Salva as mudanças no banco
    }

    public async Task CreateAsync(Venda venda)
    {
        await _context.Vendas.AddAsync(venda); // Adiciona a nova venda ao contexto
        await _context.SaveChangesAsync(); // Salva as mudanças no banco
    }

    // >>> Implementação do novo método GetSalesByPeriodAsync <<<
    public async Task<List<Venda>> GetSalesByPeriodAsync(DateTime startDate, DateTime endDate)
    {
        // Buscar as vendas onde a DataVenda está dentro do período (inclusive)
        // Usamos .Where() para filtrar pelo período
        // Usamos .Include() para carregar os Itens da Venda
        // Usamos .ThenInclude() para carregar o Produto dentro de cada Item de Venda
        // Isso evita problemas de N+1 queries no Handler ao acessar Itens e Produtos
        var vendas = await _context.Vendas // Certifique-se que _context.Vendas é o DbSet<Venda>
            .Where(v => v.DataVenda >= startDate && v.DataVenda <= endDate) // Filtra pelo período
            .Include(v => v.Itens) // Inclui os Itens da Venda
                .ThenInclude(item => item.Produto) // Inclui o Produto para cada Item
            .ToListAsync(); // Executa a query e retorna a lista de vendas

        return vendas;
    }
    // --------------------------------------------------------------

    // Se sua interface IVendaRepository tiver outros métodos, certifique-se de que estão implementados aqui.
    // Se você usava métodos de uma BaseRepository, eles não precisam ser implementados aqui
    // a menos que você queira sobrescrevê-los, mas a interface IVendaRepository
    // deve listá-los ou herdar de uma interface base que os liste.

}