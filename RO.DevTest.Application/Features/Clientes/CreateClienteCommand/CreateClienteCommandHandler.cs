using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using MediatR;

namespace RO.DevTest.Application.Features.Clientes.CreateClienteCommand
{
    public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, CreateClienteResult>
    {
        private readonly IBaseRepository<Cliente> _clienteRepository;

        public CreateClienteCommandHandler(IBaseRepository<Cliente> clienteRepository)
        {
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
        }

        public async Task<CreateClienteResult> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = new Cliente
            {
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                Endereco = request.Endereco,
                DataCadastro = DateTime.UtcNow
            };

            try
            {
                var novoCliente = await _clienteRepository.CreateAsync(cliente, cancellationToken);
                return new CreateClienteResult(true, novoCliente.Id); // 'novoCliente.Id' é Guid, 'ClienteId' em Result é Guid
            }
            catch (Exception)
            {
                // Log the exception for more detailed error information in a real application
                // Console.WriteLine($"Erro ao criar cliente: {ex}");
                return new CreateClienteResult(false, errorMessage: "Erro ao criar o cliente."); // 'ClienteId' em Result é Guid (valor padrão será Guid.Empty)
            }
        }
    }
}