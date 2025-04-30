// No arquivo IVendaRepository.cs

using RO.DevTest.Domain.Entities; // Certifique-se de ter o using para sua entidade Venda
using System; // Necessário para DateTime
using System.Collections.Generic; // Necessário para Task<List<Venda>>
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace RO.DevTest.Application.Contracts.Persistence.Repositories;

// Assumindo que IVendaRepository herda de IBaseRepository ou similar,
// mantenha os métodos existentes.
public interface IVendaRepository // : IBaseRepository<Venda> // Exemplo se herdar
{
    // Métodos existentes
    Task<List<Venda>> GetAllAsync(int pageNumber, int pageSize);
    Task<Venda?> GetByIdAsync(Guid id);
    Task<int> CountAsync();
    Task DeleteAsync(Venda venda);
    Task CreateAsync(Venda venda); // Adicionado para suportar criação de vendas

    /// <summary>
    /// Busca todas as vendas dentro de um período especificado.
    /// </summary>
    /// <param name="startDate">Data de início do período (inclusive).</param>
    /// <param name="endDate">Data de fim do período (inclusive).</param>
    /// <returns>Uma lista de vendas que ocorreram entre startDate e endDate.</returns>
    Task<List<Venda>> GetSalesByPeriodAsync(DateTime startDate, DateTime endDate); // <-- NOVO MÉTODO ADICIONADO
    Task<bool> AnyAsync(Expression<Func<Venda, bool>> predicate);


    // ... outros métodos, se houver ...
}