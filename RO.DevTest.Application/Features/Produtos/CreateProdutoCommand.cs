using MediatR;

namespace RO.DevTest.Application.Features.Produtos.Commands.CreateProdutoCommand
{
    public class CreateProdutoCommand : IRequest<CreateProdutoResult>
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }

        // Método que retorna a entidade Produto pronta para ser salva no banco de dados
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

    public class CreateProdutoResult
    {
    }
}
