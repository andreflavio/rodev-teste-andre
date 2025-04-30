using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Application.Features.User.Commands.DeleteUserCommand;
using RO.DevTest.Application.Features.User.Commands.GetAllUsersCommand;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;
using RO.DevTest.Domain.Exception;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public UsersController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost("admin")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateUserCommand command, [FromQuery] string masterPassword)
        {
            try
            {
                // Pega a senha master configurada no appsettings
                var configuredPassword = _configuration["AdminSettings:MasterPassword"];

                // Verifica se a senha mestre fornecida é a correta
                if (masterPassword != configuredPassword)
                {
                    return Forbid("Senha mestre incorreta.");
                }

                // Envia o comando para criar o usuário
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Retorna erro detalhado
                return StatusCode(500, $"Erro ao criar o usuário admin: {ex.Message}");
            }
        }

        // Criação de um novo usuário
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand request)
        {
            CreateUserResult response = await _mediator.Send(request);
            return Created(HttpContext.Request.GetDisplayUrl(), response);
        }

        // Busca todos os usuários ou filtra por nome e/ou username
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GetAllUsersResult>>> GetAllUsers([FromQuery] string? name, [FromQuery] string? userName)
        {
            var query = new GetAllUsersCommand
            {
                Name = name,
                UserName = userName
            };

            var response = await _mediator.Send(query);

            if (response == null || !response.Any())
            {
                return NotFound(new
                {
                    Message = "Nenhum usuário encontrado com os parâmetros informados.",
                    Status = "Not Found",
                    Data = response
                });
            }

            return Ok(new
            {
                Message = "Usuários encontrados com sucesso.",
                Status = "Success",
                Data = response
            });
        }

        // Atualiza um usuário existente
        [HttpPut("{id:guid}")]
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno inesperado ao processar a solicitação.");
            }
        }

        // Deleta um usuário existente pelo seu ID
        [HttpDelete("{id:guid}")]
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno inesperado ao processar a solicitação de deleção.");
            }
        }
    }
}