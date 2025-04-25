using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RO.DevTest.Application.Features.Vendas.Commands
{
    public class UpdateVendaCommand : IRequest<UpdateVendaResult>
    {
        [Required]
        public Guid VendaId { get; set; }

        [Required]
        public Guid ClienteId { get; set; }

        [Required]
        public decimal ValorTotal { get; set; }

        [Required]
        public List<UpdateVendaItemCommand> Itens { get; set; }

        [Required]
        public DateTime DataVenda { get; set; }
    }

    public class UpdateVendaItemCommand
    {
        [Required]
        public Guid ProdutoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PrecoUnitario { get; set; }
    }

    public class UpdateVendaResult
    {
        public bool Success { get; set; }
        public Guid VendaId { get; set; }
        public string Message { get; set; }
    }
}