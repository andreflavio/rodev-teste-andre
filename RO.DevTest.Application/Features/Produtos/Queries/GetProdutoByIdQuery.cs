using MediatR;
using RO.DevTest.Application.Features.Produtos.Queries;

namespace RO.DevTest.Application.Features.Produtos.Queries
{
    public class GetProdutoByIdQuery : IRequest<GetProdutoByIdResult>
    {
        public Guid Id { get; set; }

        public GetProdutoByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
