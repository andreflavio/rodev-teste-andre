using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Produtos.CreateProdutoCommand
{
    public class CreateProdutoCommandHandler : IRequestHandler<CreateProdutoCommand, CreateProdutoResult>
    {
        private readonly IProdutoRepository _produtoRepository;

        // Construtor que injeta o repositório de produto
        public CreateProdutoCommandHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        // Método Handle que processa a criação do produto
        public async Task<CreateProdutoResult> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Nome) || request.Preco <= 0)
            {
                return new CreateProdutoResult
                {
                    ErrorMessage = "Nome do produto e preço devem ser informados corretamente."
                };
            }

            var produto = new Produto
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                Preco = request.Preco,
                Estoque = request.Estoque
            };

            await _produtoRepository.CreateAsync(produto, cancellationToken);

            return new CreateProdutoResult
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Estoque = produto.Estoque,
                ErrorMessage = string.Empty // <-- LIMPA ERROS se sucesso
            };
        }

    }
}
