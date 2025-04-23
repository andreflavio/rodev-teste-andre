// Caminho esperado: RO.DevTest.Application\Features\User\Commands\GetAllUsersCommand\GetAllUsersCommand.cs

using MediatR;
using System.Collections.Generic;
//using RO.DevTest.Application.Features.User.Commands.GetAllUsersCommand; // Este using é para o namespace abaixo, geralmente não precisa dele aqui

namespace RO.DevTest.Application.Features.User.Commands.GetAllUsersCommand // <-- VERIFIQUE SE O NAMESPACE ESTÁ EXATO
{
    // Query para obter todos os usuários, agora com parâmetros opcionais de filtro
    // Implementa IRequest, retornando uma lista de GetAllUsersResult
    public class GetAllUsersCommand : IRequest<List<GetAllUsersResult>> // <-- VERIFIQUE SE O NOME DA CLASSE ESTÁ EXATO
    {
        // ** ESTAS DUAS LINHAS PRECISAM ESTAR AQUI DENTRO DA CLASSE **
        public string? Name { get; set; }
        public string? UserName { get; set; }
        // Adicione outros campos de filtro aqui, se necessário
    }
}