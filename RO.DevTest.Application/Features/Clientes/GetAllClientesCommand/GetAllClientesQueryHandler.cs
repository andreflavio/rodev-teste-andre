using MediatR;
using RO.DevTest.Application.Features.Clientes.GetAllClientesCommand;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace RO.DevTest.Application.Features.Clientes.GetAllClientesCommand

{
    public class GetAllClientesQueryHandler : IRequestHandler<GetAllClientesQuery, List<GetAllClientesResult>>

    {
        private readonly IClienteRepository _clienteRepository;

        // Construtor com injeção de dependência do repositório
        public GetAllClientesQueryHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<List<GetAllClientesResult>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
        {
            // Usando o repositório para pegar todos os clientes
            var clientes = await _clienteRepository.GetAllAsync();

            // Se você precisa aplicar filtros ou ordenação, adicione aqui antes de mapear os resultados
            if (!string.IsNullOrEmpty(request.Nome))
            {
                clientes = clientes.Where(c => c.Nome.Contains(request.Nome));
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                clientes = clientes.Where(c => c.Email.Contains(request.Email));
            }
            if (request.OrdemDecrescente)
            {
                clientes = clientes.OrderByDescending(c => EF.Property<object>(c, request.OrdenarPor));
            }
            else
            {
                clientes = clientes.OrderBy(c => EF.Property<object>(c, request.OrdenarPor));
            }

            // Paginação
            clientes = clientes.Skip((request.Pagina - 1) * request.TamanhoPagina)
                               .Take(request.TamanhoPagina);

            // Convertendo para o formato do resultado esperado
            var result = clientes.Select(c => new GetAllClientesResult
            {
                Id = c.Id,
                Nome = c.Nome,
                Email = c.Email
            }).ToList();

            return result;
        }
    }
}
