using RO.DevTest.Domain.Abstract;  // Adicionando o using correto para o BaseEntity
using System;

namespace RO.DevTest.Domain.Entities
{
    public class Produto : BaseEntity
    {
        public string Nome { get; set; } = string.Empty;  // Nome do produto
        public decimal Preco { get; set; }  // Preço do produto
        public int Estoque { get; set; }  // Quantidade em estoque
        public required string Descricao { get; set; }

        // Outras propriedades relevantes para o produto podem ser adicionadas aqui
    }
}
