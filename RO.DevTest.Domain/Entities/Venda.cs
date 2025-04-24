using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Domain.Entities
{
    public class Venda : BaseEntity
    {
        public Guid ClienteId { get; set; } // Chave estrangeira para Cliente
        public Cliente Cliente { get; set; } = null!; // Propriedade de navegação para Cliente

        public DateTime DataVenda { get; set; } = DateTime.UtcNow; // Quando foi realizada a venda

        public List<ItemVenda> Itens { get; set; } = new(); // Lista de itens vendidos
    }
}
