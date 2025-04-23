using CreateClienteCommand = RO.DevTest.Application.Features.Clientes.CreateClienteCommand.CreateClienteCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using CreateClienteResult = RO.DevTest.Application.Features.Clientes.CreateClienteCommand.CreateClienteResult;

namespace RO.DevTest.Application.Features.Clientes.CreateClienteCommand
{
    public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, CreateClienteResult>
    {
        private readonly IBaseRepository<Cliente> _clienteRepository;
        private string errorMessage;

        public CreateClienteCommandHandler(IBaseRepository<Cliente> clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public Guid ClienteId { get; private set; }

        public async Task<CreateClienteResult> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = new Cliente
            {
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                Endereco = request.Endereco,
                DataCadastro = System.DateTime.UtcNow
            };

            try
            {
                var novoCliente = await _clienteRepository.CreateAsync(cliente, cancellationToken);
                return new CreateClienteResult(true, guid: ClienteId = novoCliente.Id);
            }
            catch (Exception ex)
            {
                return new CreateClienteResult(false, errorMessage = "Erro ao criar o cliente.");
            }
        }
    }

    public interface IRequestHandler<T1, T2>
    {
    }
}