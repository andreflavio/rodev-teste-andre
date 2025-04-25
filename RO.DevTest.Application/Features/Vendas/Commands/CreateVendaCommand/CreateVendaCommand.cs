using MediatR;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RO.DevTest.Application.Features.Vendas.Commands
{
    // Classe que representa o comando para criar uma venda
    public class CreateVendaCommand : IRequest<CreateVendaResult>
    {
        public Guid ClienteId { get; set; }  // ID do cliente que fez a compra
        public List<ItemVendaDto> Itens { get; set; } = new List<ItemVendaDto>();  // Lista de itens da venda
        public DateTime DataVenda { get; set; } = DateTime.UtcNow;  // Data da venda

        // Construtor da classe, que recebe o clienteId e os itens da venda
        public CreateVendaCommand(Guid clienteId, List<ItemVendaDto> itens)
        {
            ClienteId = clienteId;
            Itens = itens;
        }

        // Método que mapeia o comando para a entidade Venda pronta para ser salva no banco de dados
        public RO.DevTest.Domain.Entities.Venda AssignTo()
        {
            return new RO.DevTest.Domain.Entities.Venda
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

    // DTO (Data Transfer Object) que representa um item na venda
    public class ItemVendaDto
    {
        public Guid ProdutoId { get; set; }  // ID do produto
        public int Quantidade { get; set; }  // Quantidade do produto
        public decimal PrecoUnitario { get; set; }  // Preço unitário do produto
    }
}
