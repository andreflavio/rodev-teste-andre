
namespace RO.DevTest.Application.Features.Clientes.CreateClienteCommand
{
    public class CreateClienteResult
    {
        private bool v;
        private Guid guid;

        public bool Success { get; set; }
        public int ClienteId { get; set; }
        public string ErrorMessage { get; set; }

        // Construtor para sucesso
        public CreateClienteResult(bool success, int clienteId = 0)
        {
            Success = success;
            ClienteId = clienteId;
            ErrorMessage = string.Empty;
        }

        // Construtor para falha
        public CreateClienteResult(bool success, string errorMessage = "")
        {
            Success = success;
            ClienteId = 0;
            ErrorMessage = errorMessage;
        }

        public CreateClienteResult(bool v, Guid guid)
        {
            this.v = v;
            this.guid = guid;
        }
    }
}