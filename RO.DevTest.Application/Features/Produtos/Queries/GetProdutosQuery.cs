using MediatR;
using RO.DevTest.Domain.Entities;
using System.Collections.Generic;

namespace RO.DevTest.Application.Features.Produtos.Queries
{
    // Define a consulta para buscar produtos
    public class GetProdutosQuery : IRequest<IEnumerable<Produto>>
    {
        public string Nome { get; set; }
        public decimal? PrecoMin { get; set; }
        public decimal? PrecoMax { get; set; }
        public int Page { get; set; } = 1; // Padrão: página 1
        public int PageSize { get; set; } = 10; // Padrão: 10 itens por página
        public OrdenarPorEnum? OrdenarPor { get; set; } // Usando o enum OrdenarPorEnum
        public DirecaoEnum? Direcao { get; set; } // Usando o enum DirecaoEnum

        // Construtor padrão sem parâmetros
        public GetProdutosQuery() { }

        // Construtor com parâmetros (se necessário)
        public GetProdutosQuery(string nome, decimal? precoMin, decimal? precoMax, int page, int pageSize, OrdenarPorEnum? ordenarPor, DirecaoEnum? direcao)
        {
            Nome = nome;
            PrecoMin = precoMin;
            PrecoMax = precoMax;
            Page = page;
            PageSize = pageSize;
            OrdenarPor = ordenarPor;
            Direcao = direcao;
        }
    }

    // Enum para direção de ordenação (ascendente ou descendente)
    public enum DirecaoEnum
    {
        Ascendente,   // Para ordenação crescente
        Descendente   // Para ordenação decrescente
    }

    // Enum para opções de ordenação (por nome ou por preço)
    public enum OrdenarPorEnum
    {
        Nome,   // Ordenar por nome
        Preco   // Ordenar por preço
    }
}
