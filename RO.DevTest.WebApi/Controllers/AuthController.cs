using MediatR;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // <<< ADICIONE ESTE USING para usar [AllowAnonymous]

namespace RO.DevTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    // Não coloque [Authorize] aqui no topo da classe! Está correto assim.
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")] // O método de Login
        [AllowAnonymous] // <<< ADICIONE ESTA LINHA AQUI para permitir acesso público!
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.Success)
            {
                // Retorna 200 OK com a resposta JSON (incluindo o token)
                return Ok(response);
            }
            else
            {
                // Retorna 401 Unauthorized com a mensagem de erro (agora, a lógica do handler é alcançada)
                return Unauthorized(response.ErrorMessage);
            }
        }
        // Você pode adicionar outros endpoints de autenticação aqui, se necessário (ex: Register, RefreshToken),
        // aplicando [AllowAnonymous] ou [Authorize] conforme a necessidade.
    }
}