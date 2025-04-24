using MediatR;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Features.Vendas.Commands
{
    public class UpdateVendaCommand : IRequest<Venda>
    {
        public Guid VendaId { get; set; }
        public string Status { get; set; }
        public decimal ValorTotal { get; set; }
        public List<ItemVenda> Itens { get; set; }  // Caso precise atualizar os itens da venda, inclua aqui
    }
}
