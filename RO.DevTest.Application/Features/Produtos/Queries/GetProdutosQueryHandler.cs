using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RO.DevTest.Application.Features.Produtos.Queries
{
    public class GetProdutosQueryHandler : IRequestHandler<GetProdutosQuery, IEnumerable<Produto>>
    {
        private readonly IProdutoRepository _produtoRepository;

        public GetProdutosQueryHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<Produto>> Handle(GetProdutosQuery request, CancellationToken cancellationToken)
        {
            var query = _produtoRepository.Query();

            // Filtro por nome
            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                query = query.Where(p => p.Nome.ToLower().Contains(request.Nome.ToLower()));
            }

            // Filtro por preço
            if (request.PrecoMin.HasValue)
            {
                query = query.Where(p => p.Preco >= request.PrecoMin.Value);
            }

            if (request.PrecoMax.HasValue)
            {
                query = query.Where(p => p.Preco <= request.PrecoMax.Value);
            }

            // Ordenação usando Enum
            if (request.OrdenarPor.HasValue)
            {
                switch (request.OrdenarPor)
                {
                    case OrdenarPorEnum.Nome:
                        query = request.Direcao == DirecaoEnum.Ascendente
                            ? query.OrderBy(p => p.Nome)
                            : query.OrderByDescending(p => p.Nome);
                        break;

                    case OrdenarPorEnum.Preco:
                        query = request.Direcao == DirecaoEnum.Ascendente
                            ? query.OrderBy(p => p.Preco)
                            : query.OrderByDescending(p => p.Preco);
                        break;

                    default:
                        query = query.OrderBy(p => p.Nome); // Default ordering if no case matched
                        break;
                }
            }

            // Paginação
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 10;

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            // Execução da query
            return await query.ToListAsync(cancellationToken);
        }
    }
}
