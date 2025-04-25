using MediatR;
using System.Collections.Generic;

namespace RO.DevTest.Application.Features.Clientes.GetAllClientesCommand
{
    public class GetAllClientesQuery : IRequest<List<GetAllClientesResult>>
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string OrdenarPor { get; set; } = "Nome";
        public bool OrdemDecrescente { get; set; } = false;
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
