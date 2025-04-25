using MediatR;

namespace RO.DevTest.Application.Features.Vendas.Queries
{
    public class GetVendaByIdQuery : IRequest<GetVendaByIdResult>
    {
        public Guid Id { get; set; }

        public GetVendaByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
