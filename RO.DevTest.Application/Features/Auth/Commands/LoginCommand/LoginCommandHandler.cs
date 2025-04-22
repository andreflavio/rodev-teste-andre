using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure; // Para IIdentityAbstractor
using RO.DevTest.Application.Contracts.infraes; // Para IJwtTokenGenerator
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IIdentityAbstractor _identityAbstractor;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(IIdentityAbstractor identityAbstractor, IJwtTokenGenerator jwtTokenGenerator)
        {
            _identityAbstractor = identityAbstractor;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar o usuário pelo Username (ou Email, dependendo da sua implementação)
            var user = await _identityAbstractor.FindUserByEmailAsync(request.Username);
            // Se você usa Username, utilize:
            // var user = await _identityAbstractor.FindUserByNameAsync(request.Username);

            if (user == null)
            {
                return new LoginResponse { Success = false, ErrorMessage = "Credenciais inválidas." };
            }

            // 2. Verificar a senha usando o PasswordSignInAsync
            var signInResult = await _identityAbstractor.PasswordSignInAsync(user, request.Password);

            if (signInResult.Succeeded)
            {
                // 3. Obter as roles do usuário
                var roles = await _identityAbstractor.GetUserRolesAsync(user);

                // 4. Gerar o token JWT
                var accessToken = _jwtTokenGenerator.GenerateToken(user, roles);

                // 5. Criar a resposta de login com o token
                return new LoginResponse
                {
                    Success = true,
                    AccessToken = accessToken,
                    ExpirationDate = System.DateTime.UtcNow.AddHours(8), // Defina a expiração do token
                    Roles = roles
                };
            }
            else
            {
                // 6. Autenticação falhou
                return new LoginResponse { Success = false, ErrorMessage = "Credenciais inválidas." };
            }
        }
    }
}