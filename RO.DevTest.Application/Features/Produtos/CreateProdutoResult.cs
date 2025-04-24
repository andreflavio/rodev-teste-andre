public class CreateProdutoResult
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Estoque { get; set; }

    public bool Sucesso => Id != Guid.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
