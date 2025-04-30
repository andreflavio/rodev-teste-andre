using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IIdentityAbstractor _identityAbstractor;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IIdentityAbstractor identityAbstractor,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<LoginCommandHandler> logger)
        {
            _identityAbstractor = identityAbstractor ?? throw new ArgumentNullException(nameof(identityAbstractor));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Login attempt with empty username or password");
                    return new LoginResponse
                    {
                        Success = false,
                        ErrorMessage = "Usuário não encontrado ou credenciais inválidas."
                    };
                }

                _logger.LogInformation("Attempting login for username: {Username}", request.Username);

                var user = await _identityAbstractor.FindUserByEmailAsync(request.Username);
                if (user == null)
                {
                    _logger.LogWarning("User not found for username: {Username}", request.Username);
                    return new LoginResponse
                    {
                        Success = false,
                        ErrorMessage = "Usuário não encontrado ou credenciais inválidas."
                    };
                }

                _logger.LogInformation("User found with ID: {UserId}", user.Id);

                var result = await _identityAbstractor.PasswordSignInAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Password validation failed for username: {Username}", request.Username);
                    return new LoginResponse
                    {
                        Success = false,
                        ErrorMessage = "Usuário não encontrado ou credenciais inválidas."
                    };
                }

                _logger.LogInformation("Password validated successfully for username: {Username}", request.Username);

                var roles = await _identityAbstractor.GetUserRolesAsync(user);
                if (roles == null || !roles.Any())
                {
                    _logger.LogWarning("No roles found for username: {Username}", request.Username);
                    return new LoginResponse
                    {
                        Success = false,
                        ErrorMessage = "Usuário não possui papéis configurados."
                    };
                }

                _logger.LogInformation("Roles retrieved for username: {Username}: {Roles}", request.Username, string.Join(", ", roles));

                var token = _jwtTokenGenerator.GenerateToken(user, roles);
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to generate JWT token for username: {Username}", request.Username);
                    return new LoginResponse
                    {
                        Success = false,
                        ErrorMessage = "Erro ao gerar token de autenticação."
                    };
                }

                _logger.LogInformation("JWT token generated successfully for username: {Username}", request.Username);

                return new LoginResponse
                {
                    Success = true,
                    AccessToken = token,
                    ExpirationDate = DateTime.UtcNow.AddHours(8),
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", request.Username);
                return new LoginResponse
                {
                    Success = false,
                    ErrorMessage = "Erro interno durante o login. Tente novamente mais tarde."
                };
            }
        }
    }
}