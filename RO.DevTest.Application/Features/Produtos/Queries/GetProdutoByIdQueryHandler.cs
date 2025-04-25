using MediatR;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Produtos.Queries
{
    public class GetProdutoByIdQueryHandler : IRequestHandler<GetProdutoByIdQuery, GetProdutoByIdResult>
    {
        private readonly IProdutoRepository _produtoRepository;

        public GetProdutoByIdQueryHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<GetProdutoByIdResult> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken)
        {
            var produto = await _produtoRepository.GetByIdAsync(request.Id);

            if (produto == null)
            {
                return null!;
            }

            return new GetProdutoByIdResult
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Estoque = produto.Estoque
            };
        }
    }
}
