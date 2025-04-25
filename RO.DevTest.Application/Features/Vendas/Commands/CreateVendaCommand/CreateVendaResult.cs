namespace RO.DevTest.Application.Features.Vendas.Commands
{
    // Resultado da criação da venda
    public class CreateVendaResult
    {
        public bool Success { get; set; }  // Indicador de sucesso
        public string Message { get; set; }  // Mensagem detalhando o resultado
        public Guid? VendaId { get; set; }  // ID da venda criada, se o sucesso ocorrer
    }
}
