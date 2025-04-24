using MediatR;
using RO.DevTest.Application.Features.Produtos; // <- Importa CreateProdutoResult

namespace RO.DevTest.Application.Features.Produtos.CreateProdutoCommand
{
    public class CreateProdutoCommand : IRequest<CreateProdutoResult>
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }

        public Domain.Entities.Produto AssignTo()
        {
            return new Domain.Entities.Produto
            {
                Nome = Nome,
                Descricao = Descricao,
                Preco = Preco,
                Estoque = Estoque
            };
        }
    }
}
