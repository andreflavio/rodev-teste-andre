using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Clientes.UpdateClienteCommand
{
    public class UpdateClienteCommandHandler : IRequestHandler<UpdateClienteCommand, UpdateClienteResult>
    {
        private readonly IClienteRepository _clienteRepository;

        public UpdateClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<UpdateClienteResult> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.GetByIdAsync(request.Id);
            if (cliente == null)
            {
                return new UpdateClienteResult
                {
                    Success = false,
                    Message = "Cliente não encontrado."
                };
            }

            cliente.Nome = request.Nome;
            cliente.Email = request.Email;
            // Atualize outros campos se necessário

            await _clienteRepository.UpdateAsync(cliente);

            return new UpdateClienteResult
            {
                Success = true,
                Message = "Cliente atualizado com sucesso."
            };
        }
    }
}
