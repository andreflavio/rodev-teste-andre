namespace RO.DevTest.Application.Features.Produtos.Commands.CreateProdutoCommand
{
    public class CreateProdutoResult
    {
        public int Id { get; set; }
        public bool Sucesso { get; set; }
        public string? ErrorMessage { get; set; } // Adiciona isso
    }
}
