// Caminho esperado: C:\Users\André\teste\RO.DevTest3\RO.DevTest.WebApi\Controllers\UsersController.cs

using MediatR;
using Microsoft.AspNetCore.Http; // Necessário para StatusCodes e HttpContext
using Microsoft.AspNetCore.Http.Extensions; // Necessário para GetDisplayUrl
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations; // Necessário para OpenApiTags, ProducesResponseType
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand; // Necessário para CreateUserCommand e CreateUserResult
using RO.DevTest.Application.Features.User.Commands.GetAllUsersCommand; // Necessário para GetAllUsersCommand e GetAllUsersResult
using System.Collections.Generic; // Necessário para IEnumerable<GetAllUsersResult>
using System.Threading.Tasks; // Necessário para Task

namespace RO.DevTest.WebApi.Controllers // <-- VERIFIQUE SE ESTE NAMESPACE ESTÁ CORRETO PARA SEU PROJETO WEbAPI
{
    [ApiController] // ** BOA PRÁTICA para controllers de API **
    [Route("api/user")] // Define a rota base para este controller
    [OpenApiTags("Users")] // Tag para o Swagger organizar
    // Controllers de API geralmente herdam de ControllerBase, mas Controller também funciona
    public class UsersController : ControllerBase // Você pode usar ControllerBase ou Controller
    {
        private readonly IMediator _mediator;

        // Construtor com injeção de dependência do IMediator (essencial para enviar comandos/queries)
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="request">Dados para criação do usuário.</param>
        /// <returns>Resultado da criação do usuário.</returns>
        [HttpPost] // Define este método como um endpoint HTTP POST
        [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status201Created)] // Documenta o retorno de sucesso (201)
        [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status400BadRequest)] // Documenta o retorno de erro (400)
        public async Task<IActionResult> CreateUser([FromQuery] CreateUserCommand request) // Recebe os dados do corpo da requisição (ou [FromQuery] se preferir via query string)
        {
            // Envia o comando CreateUserCommand para o MediatR
            CreateUserResult response = await _mediator.Send(request);

            // Retorna uma resposta 201 Created com a localização do novo recurso (opcional, mas boa prática)
            // string uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}/{response.Id}"; // Exemplo de URI se o GET por ID existisse
            // return Created(uri, response);
            // Ou simplesmente Created com a URL da requisição atual
            return Created(HttpContext.Request.GetDisplayUrl(), response);
        }

        /// <summary>
        /// Busca todos os usuários ou filtra por nome e/ou username.
        /// </summary>
        /// <param name="name">Filtra por nome do usuário.</param>
        /// <param name="userName">Filtra por username do usuário.</param>
        /// <returns>Uma lista de usuários encontrados.</returns>
        [HttpGet] // Define este método como um endpoint HTTP GET
        [ProducesResponseType(typeof(IEnumerable<GetAllUsersResult>), StatusCodes.Status200OK)] // Documenta o retorno de sucesso (200)
        public async Task<ActionResult<IEnumerable<GetAllUsersResult>>> GetAllUsers(
            [FromQuery] string? name, // Recebe o parâmetro 'name' da query string (ex: /api/user?name=...)
            [FromQuery] string? userName // Recebe o parâmetro 'userName' da query string (ex: /api/user?userName=...)
                                         // Adicione outros parâmetros [FromQuery] aqui se adicionou na Query/Repositório
        )
        {
            // Cria uma instância do comando/query da Application e popula com os parâmetros recebidos
            var query = new GetAllUsersCommand // Use a Query CORRETA da camada Application
            {
                Name = name,       // Passa o valor recebido para a propriedade da Query
                UserName = userName // Passa o valor recebido para a propriedade da Query
                // Popule outros campos aqui
            };

            // Envia o comando/query (agora com filtros) para o MediatR
            // O MediatR encontrará o GetAllUsersCommandHandler e o executará
            var response = await _mediator.Send(query);

            // Retorna a lista de GetAllUsersResult com status 200 OK
            return Ok(response);
        }

        // Adicione outros métodos de ação (PUT, DELETE, etc.) aqui
    }

    // ** NÃO COLOQUE DEFINIÇÕES DE COMANDOS/QUERIES DA APPLICATION AQUI **
    // Remova qualquer classe como 'internal class GetAllUsersQuery : IRequest<object>' se ela estiver aqui.
}