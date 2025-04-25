using MediatR;
using RO.DevTest.Domain.Entities;
using VendaEntity = RO.DevTest.Domain.Entities.Venda; // Alias para a classe Venda

namespace RO.DevTest.Application.Features.Vendas.Commands
{
    public class UpdateVendaCommand : IRequest<VendaEntity>  // Usando o alias VendaEntity
    {
        public Guid VendaId { get; set; }
        public string Status { get; set; }
        public decimal ValorTotal { get; set; }
        public List<ItemVenda> Itens { get; set; }  // Caso precise atualizar os itens da venda, inclua aqui
    }
}
