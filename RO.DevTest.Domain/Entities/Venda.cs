using RO.DevTest.Domain.Abstract;  // Adicionando a importação do namespace correto para BaseEntity
using RO.DevTest.Domain.Entities;  // Importa a classe Cliente, que está no mesmo namespace onde Venda está

namespace RO.DevTest.Domain.Entities
{
    public class Venda : BaseEntity  // Herda de BaseEntity, que está no namespace RO.DevTest.Domain.Abstract
    {
        public Guid ClienteId { get; set; }  // Chave estrangeira para Cliente
        public Cliente Cliente { get; set; } = null!;  // Propriedade de navegação para Cliente

        public DateTime DataVenda { get; set; } = DateTime.UtcNow;  // Quando foi realizada a venda

        public List<ItemVenda> Itens { get; set; } = new();  // Lista de itens vendidos

        // Propriedade Total calculada com base nos itens da venda
        public decimal Total
        {
            get
            {
                // Calcula o total somando os preços dos itens multiplicados pela quantidade
                return Itens.Sum(item => item.Quantidade * item.PrecoUnitario);
            }
        }
    }
}
