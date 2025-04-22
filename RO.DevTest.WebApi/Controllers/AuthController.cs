using MediatR;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using System.Threading.Tasks;

namespace RO.DevTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(new { Token = result.AccessToken }); // Use AccessToken
            }
            else
            {
                // Você precisará adicionar uma propriedade ErrorMessage ao seu LoginResponse
                // para que isso funcione. Por enquanto, podemos retornar uma mensagem genérica.
                return Unauthorized("Falha na autenticação");
            }
        }
    }
}