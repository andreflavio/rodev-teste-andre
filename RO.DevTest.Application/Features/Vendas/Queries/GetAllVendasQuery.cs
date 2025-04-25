using MediatR;
using System.Collections.Generic;

namespace RO.DevTest.Application.Features.Vendas.Queries
{
    public class GetAllVendasQuery : IRequest<List<GetAllVendasResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public GetAllVendasQuery() { }

        public GetAllVendasQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}