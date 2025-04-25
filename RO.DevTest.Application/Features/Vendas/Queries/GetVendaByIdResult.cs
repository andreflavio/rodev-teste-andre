// Arquivo: RO.DevTest.Application\Features\Vendas\Queries\GetVendaByIdResult.cs
using System; // Necessário para Guid, DateTime
using System.Collections.Generic; // Necessário para List<>

// O namespace deve ser o mesmo dos outros arquivos nesta pasta
namespace RO.DevTest.Application.Features.Vendas.Queries
{
    // Definição da classe que o GetVendaByIdQuery e GetVendaByIdQueryHandler precisam
    public class GetVendaByIdResult
    {
        // Propriedades que representam o resultado da busca por uma venda específica
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal Total { get; set; }

        // A lista de itens da venda.
        // É uma boa prática inicializar listas para evitar NullReferenceException,
        // e isso também ajuda a resolver o warning CS8618.
        public List<VendaItemDto> Itens { get; set; } = new List<VendaItemDto>();
    }


}