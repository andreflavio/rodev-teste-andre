using MediatR;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Application.Features.Produtos.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Produtos
{
    public class DeleteProdutoCommandHandler : IRequestHandler<DeleteProdutoCommand, bool>
    {
        private readonly IProdutoRepository _produtoRepository;

        public DeleteProdutoCommandHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<bool> Handle(DeleteProdutoCommand request, CancellationToken cancellationToken)
        {
            var produto = await _produtoRepository.GetByIdAsync(request.Id);
            if (produto == null)
            {
                return false;
            }

            await _produtoRepository.DeleteAsync(request.Id);
            return true;
        }
    }
}
