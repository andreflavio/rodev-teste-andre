using MediatR;
using RO.DevTest.Application.Features.Clientes.Commands.UpdateClienteCommand;
using System;

namespace RO.DevTest.Application.Features.Clientes.UpdateClienteCommand

{
    public class UpdateClienteCommand : IRequest<UpdateClienteResult>
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // Adicione mais campos se o modelo de Cliente tiver
    }
}
