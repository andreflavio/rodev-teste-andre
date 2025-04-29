// Caminho: C:\Users\André\teste\RO.DevTest3\RO.DevTest.WebApi\Controllers\UsersController.cs

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations; // Certifique-se de que este using está presente se estiver usando NSwag
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Application.Features.User.Commands.DeleteUserCommand;
using RO.DevTest.Application.Features.User.Commands.GetAllUsersCommand;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;
using RO.DevTest.Domain.Exception; // Certifique-se de que este using está presente se estiver usando BadRequestException
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Este using é ESSENCIAL para [Authorize] e [AllowAnonymous]
using System.Linq; // Necessário para o método Any() usado em GetAllUsers

namespace RO.DevTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize] // <<< Este [Authorize] fica aqui para proteger os outros métodos do controlador >>>

    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        // private readonly ILogger<UsersController> _logger; // Descomente se usar ILogger

        public UsersController(IMediator mediator/*, ILogger<UsersController> logger*/)
        {
            _mediator = mediator;
            // _logger = logger; // Descomente se usar ILogger
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="request">Dados para criação do usuário.</param>
        /// <returns>Resultado da criação do usuário.</returns>
        [HttpPost]
        [AllowAnonymous] // <<< COLOQUE ESTA LINHA AQUI, ACIMA DO MÉTODO CreateUser >>>
        [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromQuery] CreateUserCommand request)
        {
            CreateUserResult response = await _mediator.Send(request);
            return Created(HttpContext.Request.GetDisplayUrl(), response);
        }

        /// <summary>
        /// Busca todos os usuários ou filtra por nome e/ou username.
        /// </summary>
        // ... outros atributos [HttpGet], [ProducesResponseType] ...
        [HttpGet]
        // <<< Este método GetAllUsers está protegido pelo [Authorize] da classe.
        // Você pode adicionar [Authorize(Roles = "Admin")] aqui se só Admins puderem listar todos.
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GetAllUsersResult>>> GetAllUsers(
            [FromQuery] string? name,
            [FromQuery] string? userName)
        {
            var query = new GetAllUsersCommand
            {
                Name = name,
                UserName = userName
            };

            var response = await _mediator.Send(query);

            if (response == null || !response.Any())
            {
                // Nenhum usuário encontrado
                return NotFound(new
                {
                    Message = "Nenhum usuário encontrado com os parâmetros informados.",
                    Status = "Not Found",
                    Data = response
                });
            }

            // Usuários encontrados com sucesso
            return Ok(new
            {
                Message = "Usuários encontrados com sucesso.",
                Status = "Success",
                Data = response
            });
        }


        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        // ... outros atributos [HttpPut], [ProducesResponseType] ...
        [HttpPut("{id:guid}")]
        // <<< Este método UpdateUser está protegido pelo [Authorize] da classe.
        // Você pode adicionar [Authorize(Roles = "Admin")] aqui se só Admins puderem atualizar.
        [ProducesResponseType(typeof(UpdateUserResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("O ID na rota não corresponde ao ID no corpo da requisição.");
            }

            try
            {
                UpdateUserResult response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.ToString()}");
                // _logger.LogError(ex, "Ocorreu um erro inesperado ao atualizar o usuário com ID {UserId}.", id); // Descomente se usar ILogger
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno inesperado ao processar a solicitação.");
            }
        }

        /// <summary>
        /// Deleta um usuário existente pelo seu ID.
        /// </summary>
        // ... outros atributos [HttpDelete], [ProducesResponseType] ...
        [HttpDelete("{id:guid}")]
        // <<< Este método DeleteUser está protegido pelo [Authorize] da classe.
        // Você pode adicionar [Authorize(Roles = "Admin")] aqui se só Admins puderem deletar.
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var command = new DeleteUserCommand { Id = id };
                DeleteUserResult response = await _mediator.Send(command);

                if (response.Success)
                {
                    return NoContent();
                }

                return BadRequest(response.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro inesperado ao deletar o usuário com ID {id}: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.ToString()}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno inesperado ao processar a solicitação de deleção.");
            }
        }
    }
}