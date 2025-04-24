using MediatR;
using System;

namespace RO.DevTest.Application.Features.Clientes.UpdateClienteCommand
{
    public class UpdateClienteCommand : IRequest<UpdateClienteResult>
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
    }
}
