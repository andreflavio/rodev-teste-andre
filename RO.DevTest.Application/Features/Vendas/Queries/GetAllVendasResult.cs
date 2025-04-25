// Arquivo: RO.DevTest.Application\Features\Vendas\Queries\GetAllVendasResult.cs
using System;
using System.Collections.Generic;

namespace RO.DevTest.Application.Features.Vendas.Queries
{
    // Esta é a definição da classe GetAllVendasResult que o handler precisa
    public class GetAllVendasResult
    {
        // Estas são as propriedades que o handler espera encontrar
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal Total { get; set; }

        // A lista de itens da venda.
        // Inicializamos a lista aqui para evitar o aviso CS8618 ("propriedade não anulável precisa de valor").
        public List<VendaItemDto> Itens { get; set; } = new List<VendaItemDto>();

        // Um construtor padrão vazio pode ser útil
        public GetAllVendasResult()
        {
            // O corpo pode estar vazio se as propriedades já forem inicializadas acima
        }
    }

    // Esta é a definição do DTO para os itens de venda.
    // Ela é necessária para que a classe GetAllVendasResult funcione.

}