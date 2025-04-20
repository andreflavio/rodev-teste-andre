// Caminho: C:\Users\André\teste\RO.DevTest3\RO.DevTest.WebApi\Controllers\UsersController.cs

using MediatR;
using Microsoft.AspNetCore.Http; // Necessário para StatusCodes e HttpContext
using Microsoft.AspNetCore.Http.Extensions; // Necessário para GetDisplayUrl (usado no CreateUser original)
using Microsoft.AspNetCore.Mvc; // Necessário para ControllerBase, ApiController, HttpPut, HttpPost, HttpGet, etc.
using NSwag.Annotations; // Necessário para OpenApiTags, ProducesResponseType
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand; // Necessário para CreateUserCommand e CreateUserResult
using RO.DevTest.Application.Features.User.Commands.GetAllUsersCommand; // Necessário para GetAllUsersCommand e GetAllUsersResult
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand; // Necessário para UpdateUserCommand e UpdateUserResult
using RO.DevTest.Domain.Exception; // Necessário para BadRequestException (usado no catch do UpdateUser)
using System; // Necessário para Guid e Exception
using System.Collections.Generic; // Necessário para IEnumerable<GetAllUsersResult>
using System.Threading.Tasks; // Necessário para Task
// Se estiver usando ILogger, adicione:
// using Microsoft.Extensions.Logging;


namespace RO.DevTest.WebApi.Controllers // <-- VERIFIQUE SE ESTE NAMESPACE ESTÁ CORRETO PARA SEU PROJETO WebAPI
{
    [ApiController] // Indica que este controller responde a requisições de API
    [Route("api/user")] // Define a rota base para este controller (ex: /api/user)
    [OpenApiTags("Users")] // Tag para o Swagger organizar a documentação
    // Controllers de API geralmente herdam de ControllerBase
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        // Se estiver usando ILogger, declare-o aqui:
        // private readonly ILogger<UsersController> _logger;

        // Construtor com injeção de dependência do IMediator (e ILogger, se estiver usando)
        public UsersController(IMediator mediator/*, ILogger<UsersController> logger*/)
        {
            _mediator = mediator;
            // Se estiver usando ILogger, inicialize-o:
            // _logger = logger;
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="request">Dados para criação do usuário.</param>
        /// <returns>Resultado da criação do usuário.</returns>
        [HttpPost] // Define este método como um endpoint HTTP POST
        [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status201Created)] // Documenta o retorno de sucesso (201 Created)
        [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status400BadRequest)] // Documenta o retorno de erro (400 Bad Request)
        // ** MANTIDO SEU MÉTODO CREATEUSER ORIGINAL **
        public async Task<IActionResult> CreateUser([FromQuery] CreateUserCommand request) // Recebe os dados da query string (ou [FromBody] se for do corpo)
        {
            // Envia o comando CreateUserCommand para o MediatR
            CreateUserResult response = await _mediator.Send(request);

            // Retorna uma resposta 201 Created com a localização do novo recurso (usando GetDisplayUrl como no seu original)
            return Created(HttpContext.Request.GetDisplayUrl(), response);
        }

        /// <summary>
        /// Busca todos os usuários ou filtra por nome e/ou username.
        /// </summary>
        /// <param name="name">Filtra por nome do usuário.</param>
        /// <param name="userName">Filtra por username do usuário.</param>
        /// <returns>Uma lista de usuários encontrados.</returns>
        [HttpGet] // Define este método como um endpoint HTTP GET
        [ProducesResponseType(typeof(IEnumerable<GetAllUsersResult>), StatusCodes.Status200OK)] // Documenta o retorno de sucesso (200 OK)
        // ** MANTIDO SEU MÉTODO GETALLUSERS ORIGINAL **
        public async Task<ActionResult<IEnumerable<GetAllUsersResult>>> GetAllUsers(
            [FromQuery] string? name, // Recebe o parâmetro 'name' da query string (ex: /api/user?name=...)
            [FromQuery] string? userName // Recebe o parâmetro 'userName' da query string (ex: /api/user?userName=...)
                                         // Adicione outros parâmetros [FromQuery] aqui se adicionou na Query/Repositório
        )
        {
            // Cria uma instância do comando/query da Application e popula com os parâmetros recebidos
            var query = new GetAllUsersCommand // Use a Query CORRETA da camada Application
            {
                Name = name, // Passa o valor recebido para a propriedade da Query
                UserName = userName // Passa o valor recebido para a propriedade da Query
                // Popule outros campos aqui
            };

            // Envia o comando/query (agora com filtros) para o MediatR
            // O MediatR encontrará o GetAllUsersCommandHandler e o executará
            var response = await _mediator.Send(query);

            // Retorna a lista de GetAllUsersResult com status 200 OK
            return Ok(response);
        }


        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="id">O ID (Guid) do usuário a ser atualizado.</param>
        /// <param name="command">Dados para atualização do usuário.</param>
        /// <returns>Resultado da atualização do usuário.</returns>
        [HttpPut("{id:guid}")] // Define este método como um endpoint HTTP PUT com o ID na rota
        [ProducesResponseType(typeof(UpdateUserResult), StatusCodes.Status200OK)] // Documenta retorno 200 OK
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Documenta erro 400 Bad Request
        // [ProducesResponseType(StatusCodes.Status404NotFound)] // Opcional: se o handler lançasse NotFoundException
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Documenta erro 500 Internal Server Error
        // ** MÉTODO UPDATEUSER ARRUMADO (com retorno no catch e uso do ex) **
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command) // Recebe ID da rota e Command do corpo
        {
            // Verifica se o ID da rota corresponde ao ID no corpo da requisição
            if (id != command.Id)
            {
                return BadRequest("O ID na rota não corresponde ao ID no corpo da requisição.");
            }

            try
            {
                // Envia o comando para o MediatR
                UpdateUserResult response = await _mediator.Send(command);

                // Retorna a resposta de sucesso (200 OK)
                return Ok(response);

                // Alternativa: retornar 204 NoContent se a atualização for bem-sucedida e não precisa retornar dados.
                // return NoContent();
            }
            catch (BadRequestException ex) // Captura exceções de requisição inválida do Handler
            {
                // Retorna status 400 Bad Request com a mensagem de erro
                return BadRequest(ex.Message);
            }
            catch (Exception ex) // Captura outras exceções inesperadas
            {
                // Use a variável 'ex' aqui para registrar a exceção (remove o aviso CS0168)
                Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.ToString()}"); // Fornece mais detalhes (StackTrace, etc.)

                // Se estiver usando ILogger injetado:
                // _logger.LogError(ex, "Ocorreu um erro inesperado ao atualizar o usuário com ID {UserId}.", id);

                // >>> RETORNO ADICIONADO AQUI PARA RESOLVER O ERRO CS0161 <<<
                // Retorna um status 500 para erros internos inesperados
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno inesperado ao processar a solicitação.");
            }
        }

        // ** NÃO COLOQUE DEFINIÇÕES DE COMANDOS/QUERIES DA APPLICATION AQUI DENTRO DA CLASSE DO CONTROLLER **
        // Certifique-se de que as classes UpdateUserCommand, UpdateUserResult,
        // UpdateUserCommandHandler, UpdateUserCommandValidator
        // ESTÃO APENAS nos seus respectivos arquivos separados na camada Application.
    }
    // ** NÃO COLOQUE DEFINIÇÕES DE COMANDOS/QUERIES DA APPLICATION AQUI FORA TAMBÉM **
}