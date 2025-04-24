using MediatR;
using System;

namespace RO.DevTest.Application.Features.Produtos.Commands
{
    public class DeleteProdutoCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteProdutoCommand(Guid id)
        {
            Id = id;
        }
    }
}
