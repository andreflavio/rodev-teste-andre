using MediatR;

namespace RO.DevTest.Application.Features.Produtos
{
    public class UpdateProdutoCommand : IRequest<UpdateProdutoResult>
    {
        public Guid Id { get; set; }  // Alterado para Guid
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
    }
}
