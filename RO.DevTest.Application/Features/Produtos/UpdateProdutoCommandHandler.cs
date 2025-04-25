using MediatR;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Produtos
{
    public class UpdateProdutoCommandHandler : IRequestHandler<UpdateProdutoCommand, UpdateProdutoResult>
    {
        private readonly IProdutoRepository _produtoRepository;

        public UpdateProdutoCommandHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<UpdateProdutoResult> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var produto = await _produtoRepository.GetByIdAsync(request.Id);

                if (produto == null)
                {
                    return new UpdateProdutoResult
                    {
                        Id = request.Id,
                        Sucesso = false
                    };
                }

                produto.Nome = request.Nome;
                produto.Descricao = request.Descricao;
                produto.Preco = request.Preco;
                produto.Estoque = request.Estoque;

                await _produtoRepository.UpdateAsync(produto);

                return new UpdateProdutoResult
                {
                    Id = produto.Id,
                    Sucesso = true
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO ATUALIZAR PRODUTO: {ex.Message}");
                throw;
            }
        }

    }
}
