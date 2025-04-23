namespace RO.DevTest.Application.Features.Clientes.GetAllClientesCommand
{
    using MediatR; // Necessário para IRequest

    public class GetAllClientesQuery : IRequest<List<GetAllClientesResult>>
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public bool OrdemDecrescente { get; set; }
        public string OrdenarPor { get; set; } = "Nome"; // Pode ser outro campo, como "Email", "Id", etc.
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
