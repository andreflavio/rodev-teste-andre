namespace RO.DevTest.Application.Features.Clientes.CreateClienteCommand
{
    public class CreateClienteResult
    {
        public bool Success { get; set; }
        public Guid ClienteId { get; set; } // ID agora é Guid
        public string ErrorMessage { get; set; }

        // Construtor para sucesso
        public CreateClienteResult(bool success, Guid clienteId) // ID agora é Guid
        {
            Success = success;
            ClienteId = clienteId;
            ErrorMessage = string.Empty;
        }

        // Construtor para falha
        public CreateClienteResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
            ClienteId = Guid.Empty; // Valor padrão para Guid em caso de falha
        }
    }
}