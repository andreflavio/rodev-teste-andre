// No arquivo GetSalesAnalysisQuery.cs (sugestão de pasta: RO.DevTest.Application/Features/Vendas/Queries/SalesAnalysis)

using MediatR;
using System; // Necessário para DateTime

namespace RO.DevTest.Application.Features.Vendas.Queries.SalesAnalysis;
// Ajuste o namespace conforme a pasta que você criar

// A query para buscar a análise de vendas em um período
// Ela retornará um GetSalesAnalysisResult (que definiremos a seguir)
public class GetSalesAnalysisQuery : IRequest<GetSalesAnalysisResult>
{
    // Data de início do período (inclusive)
    public DateTime StartDate { get; set; }

    // Data de fim do período (inclusive)
    public DateTime EndDate { get; set; }

    // Opcional: Validação básica pode ser colocada em um Validator separado (FluentValidation)
}