using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Domain.Entities
{
    public class Cliente : BaseEntity
    {
        public string Nome { get; set; } = string.Empty; // Nome do cliente (obrigatório)
        public string Email { get; set; } = string.Empty; // Email do cliente (obrigatório, talvez único)
        public string? Telefone { get; set; } // Telefone de contato (opcional)
        public string? Endereco { get; set; } // Endereço do cliente (opcional)
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow; // Data em que o cliente foi cadastrado
        // Outras propriedades relevantes podem ser adicionadas aqui
    }
}