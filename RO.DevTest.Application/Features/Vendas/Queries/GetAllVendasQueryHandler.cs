// Arquivo: RO.DevTest.Application\Features\Vendas\Queries\GetVendaByIdQueryHandler.cs
using MediatR;
using RO.DevTest.Application.Contracts.Persistence.Repositories; // Para IVendaRepository
using RO.DevTest.Domain.Entities; // Necessário para a entidade Venda
using System; // Necessário para Guid (no request)
using System.Threading;
using System.Threading.Tasks;
// Usings necessários para a query e o resultado (assumindo que estão nesta pasta/namespace)
using RO.DevTest.Application.Features.Vendas.Queries;

// O namespace para o handler
namespace RO.DevTest.Application.Features.Vendas.Queries // VERIFIQUE SE ESTE NAMESPACE ESTÁ CORRETO PARA ESTE ARQUIVO
{
    // O handler implementa IRequestHandler para a query GetVendaByIdQuery e retorna GetVendaByIdResult (ou null)
    // Adicionamos '?' ao tipo de retorno para permitir retornar null se a venda não for encontrada.
    public class GetVendaByIdQueryHandler : IRequestHandler<GetVendaByIdQuery, GetVendaByIdResult?>
    {
        // Injetamos a interface do repositório diretamente
        private readonly IVendaRepository _vendaRepository;

        // Construtor para injeção de dependência
        public GetVendaByIdQueryHandler(IVendaRepository vendaRepository)
        {
            _vendaRepository = vendaRepository;
        }

        // Método de tratamento da requisição
        public async Task<GetVendaByIdResult?> Handle(GetVendaByIdQuery request, CancellationToken cancellationToken)
        {
            // Obtendo a venda pelo Id usando o repositório
            // Assumimos que _vendaRepository.GetByIdAsync retorna Task<Venda?>
            var venda = await _vendaRepository.GetByIdAsync(request.Id);

            // Se a venda não for encontrada, retornamos null (por isso o tipo de retorno é GetVendaByIdResult?)
            if (venda == null)
            {
                return null;
            }

            // Mapeando a entidade Venda encontrada para o formato de resultado esperado
            // Se a entidade Venda e ItemVenda estiverem corretas, este mapeamento deve funcionar.
            // O DTO VendaItemDto também deve estar acessível.
            return new GetVendaByIdResult
            {
                Id = venda.Id, // Propriedades da entidade Venda
                ClienteId = venda.ClienteId,
                DataVenda = venda.DataVenda,
                Total = venda.Total,
                Itens = venda.Itens.Select(i => new VendaItemDto // Propriedades da entidade ItemVenda
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            };
        }
    }
}