using MediatR;
using RO.DevTest.Application.Features.Clientes.GetAllClientesCommand;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
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
            // Converte para IQueryable para permitir filtragem e paginação
            var clientes = (await _clienteRepository.GetAllAsync()).AsQueryable();

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

            var result = await clientes
                .Select(c => new GetAllClientesResult
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Email = c.Email
                })
                .ToListAsync(cancellationToken); // Usa ToListAsync se estiver usando EF Core

            return result;
        }

    }
}
