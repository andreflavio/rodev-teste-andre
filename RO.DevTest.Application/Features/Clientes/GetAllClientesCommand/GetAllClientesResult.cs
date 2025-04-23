namespace RO.DevTest.Application.Features.Clientes.GetAllClientesCommand
{
    public class GetAllClientesResult
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
    }
}
