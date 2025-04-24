using MediatR;
using RO.DevTest.Application.Features.Vendas.Commands.CreateVendaCommand;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Features.Vendas.Commands.CreateVendaCommand
{
    public class CreateVendaCommand : IRequest<CreateVendaResult>
    {
        public Guid ClienteId { get; set; }  // ID do cliente que fez a compra
        public List<ItemVendaDto> Itens { get; set; } = new List<ItemVendaDto>();  // Lista de itens da venda
        public DateTime DataVenda { get; set; } = DateTime.UtcNow;  // Data da venda

        public CreateVendaCommand(Guid clienteId, List<ItemVendaDto> itens)
        {
            ClienteId = clienteId;
            Itens = itens;
        }

        // Método que retorna a entidade Venda pronta para ser salva no banco de dados
        public Venda AssignTo()
        {
            return new Venda
            {
                ClienteId = ClienteId,
                DataVenda = DataVenda,
                Itens = Itens.Select(item => new ItemVenda
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                }).ToList()
            };
        }
    }

    // DTO para representar um item da venda
    public class ItemVendaDto
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
