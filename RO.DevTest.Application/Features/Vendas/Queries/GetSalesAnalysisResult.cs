// No arquivo GetSalesAnalysisResult.cs (sugestão de pasta: RO.DevTest.Application/Features/Vendas/Queries/SalesAnalysis)

using System.Collections.Generic; // Necessário para List

namespace RO.DevTest.Application.Features.Vendas.Queries.SalesAnalysis;
// Ajuste o namespace

// Resultado da análise de vendas por período
public class GetSalesAnalysisResult
{
    public int TotalSalesCount { get; set; } // Quantidade total de vendas no período
    public decimal TotalRevenue { get; set; } // Renda total gerada por todas as vendas no período
    public List<ProductRevenueDto> RevenuePerProduct { get; set; } = new List<ProductRevenueDto>(); // Lista com a renda gerada por cada produto
}