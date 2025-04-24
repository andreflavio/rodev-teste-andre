using RO.DevTest.Domain.Abstract;  // Certificando-se de que está referenciando o BaseEntity
using System;

namespace RO.DevTest.Domain.Entities
{
    public class ItemVenda : BaseEntity
    {
        public Guid ProdutoId { get; set; }  // Alterado para Guid para alinhar com a chave primária de Produto
        public Produto Produto { get; set; } = null!;

        public Guid VendaId { get; set; }  // Chave estrangeira para Venda
        public Venda Venda { get; set; } = null!;

        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
