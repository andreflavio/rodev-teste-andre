using MediatR;

namespace RO.DevTest.Application.Features.Clientes.CreateClienteCommand
{
    public class CreateClienteCommand : IRequest<CreateClienteResult>
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Endereco { get; set; }
    }
}
