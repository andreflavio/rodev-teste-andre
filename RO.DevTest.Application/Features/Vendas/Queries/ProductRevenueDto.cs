// No arquivo ProductRevenueDto.cs (sugestão de pasta: RO.DevTest.Application/Features/Vendas/Queries/SalesAnalysis)

namespace RO.DevTest.Application.Features.Vendas.Queries.SalesAnalysis;
// Ajuste o namespace

// DTO para representar a renda por produto na análise de vendas
public class ProductRevenueDto
{
    public string ProductName { get; set; } = string.Empty; // Nome do produto
    public decimal TotalRevenue { get; set; } // Renda total gerada por este produto no período
}