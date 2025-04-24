using System;

namespace RO.DevTest.Application.Features.Produtos.Queries
{
    public class GetProdutoByIdResult
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
    }
}
