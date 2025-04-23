namespace RO.DevTest.Application.Features.Clientes.CreateClienteCommand
{
    public record CreateClienteCommand(
        string Nome,
        string Email,
        string? Telefone,
        string? Endereco
    );
}