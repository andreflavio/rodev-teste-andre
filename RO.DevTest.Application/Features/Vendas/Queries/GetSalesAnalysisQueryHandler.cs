// No arquivo GetSalesAnalysisQueryHandler.cs (sugestão de pasta: RO.DevTest.Application/Features/Vendas/Queries/SalesAnalysis)

using MediatR; // Necessário para IRequestHandler
using RO.DevTest.Application.Contracts.Persistence.Repositories; // Necessário para IVendaRepository
using RO.DevTest.Application.Features.Vendas.Queries.SalesAnalysis; // Necessário para a Query e Resultado
using RO.DevTest.Domain.Entities; // Necessário para Venda e ItemVenda
using System;
using System.Collections.Generic;
using System.Linq; // Necessário para Count(), Sum(), GroupBy()
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Vendas.Queries.SalesAnalysis;
// Ajuste o namespace conforme a pasta que você criou

public class GetSalesAnalysisQueryHandler : IRequestHandler<GetSalesAnalysisQuery, GetSalesAnalysisResult>
{
    private readonly IVendaRepository _vendaRepository;

    // O repositório é injetado no construtor
    public GetSalesAnalysisQueryHandler(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public async Task<GetSalesAnalysisResult> Handle(GetSalesAnalysisQuery request, CancellationToken cancellationToken)
    {
        // PASSO 1: Buscar as vendas no repositório usando o método que implementamos
        var vendasNoPeriodo = await _vendaRepository.GetSalesByPeriodAsync(request.StartDate, request.EndDate);

        // Verifica se encontrou vendas (a lista pode estar vazia, o que é válido)
        if (vendasNoPeriodo == null)
        {
            // Embora o método do repositório retorne List<Venda>, uma boa prática é checar null
            // Se o repositório garante que nunca retorna null (apenas lista vazia), esta checagem é opcional.
            vendasNoPeriodo = new List<Venda>(); // Trata como lista vazia se vier null
        }


        // PASSO 2: Calcular as métricas de análise

        // Quantidade Total de Vendas: apenas a contagem de vendas retornadas
        var totalSalesCount = vendasNoPeriodo.Count;

        // Renda Total: soma do campo 'Total' de cada venda
        var totalRevenue = vendasNoPeriodo.Sum(v => v.Total);

        // Renda por Produto:
        // Precisamos iterar sobre todos os ITENS de todas as vendas, agrupar por Produto
        // e somar o valor de cada item (Quantidade * PrecoUnitario) para cada produto.
        var revenuePerProductList = vendasNoPeriodo
            .SelectMany(venda => venda.Itens) // Pega todos os itens de todas as vendas em uma única lista
            .GroupBy(item => item.Produto) // Agrupa os itens pelo Produto (requer que Produto tenha sido Include no repositório)
            .Select(grupo => new ProductRevenueDto // Para cada grupo de itens de um produto...
            {
                ProductName = grupo.Key?.Nome ?? "Produto Desconhecido", // Nome do produto (usa ?.Nome para segurança e default se Produto for null)
                TotalRevenue = grupo.Sum(item => item.Quantidade * item.PrecoUnitario) // Soma o valor total de todos os itens desse produto
            })
            .ToList(); // Converte o resultado para uma lista de ProductRevenueDto


        // PASSO 3: Montar o objeto de Resultado
        var result = new GetSalesAnalysisResult
        {
            TotalSalesCount = totalSalesCount,
            TotalRevenue = totalRevenue,
            RevenuePerProduct = revenuePerProductList // A lista calculada no passo 2
        };

        // PASSO 4: Retornar o Resultado
        return result;
    }
}