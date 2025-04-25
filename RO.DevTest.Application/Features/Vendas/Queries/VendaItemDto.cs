// Exemplo: Conteúdo para VendaItemDto.cs
using System; // Necessário se VendaItemDto usar Guid
// Adicione outros usings aqui se as propriedades de VendaItemDto precisarem

// Este namespace deve ser o mesmo onde GetAllVendasResult e GetVendaByIdResult estão
namespace RO.DevTest.Application.Features.Vendas.Queries
{
    // Definição da classe VendaItemDto
    public class VendaItemDto
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }

        // Se você tiver outros avisos CS8618 aqui, inicialize as propriedades não anuláveis
        // Ex: public string NomeItem { get; set; } = string.Empty;
    }
}