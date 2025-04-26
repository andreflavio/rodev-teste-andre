
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
            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response); // Retorna todo o objeto de resposta (incluindo o token)
            }
            else
            {
                return Unauthorized(response.ErrorMessage); // Retorna Unauthorized com a mensagem de erro
            }
        }
    }
}