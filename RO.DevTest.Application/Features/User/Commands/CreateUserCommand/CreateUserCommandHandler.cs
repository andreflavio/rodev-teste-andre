using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using RO.DevTest.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IIdentityAbstractor _identityAbstractor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IIdentityAbstractor identityAbstractor,
            IConfiguration configuration,
            ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
            _identityAbstractor = identityAbstractor ?? throw new ArgumentNullException(nameof(identityAbstractor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("[DEBUG] Role recebido no request: {Role} (Valor numérico: {RoleValue})",
                    request.Role, (int)request.Role);

                if (string.IsNullOrWhiteSpace(request.UserName))
                    throw new ArgumentException("O campo UserName do request é obrigatório.", nameof(request));
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentException("O campo Name do request é obrigatório.", nameof(request));
                if (string.IsNullOrWhiteSpace(request.Email))
                    throw new ArgumentException("O campo Email do request é obrigatório.", nameof(request));
                if (string.IsNullOrWhiteSpace(request.Password))
                    throw new ArgumentException("O campo Password do request é obrigatório.", nameof(request));
                if (request.Password != request.PasswordConfirmation)
                    throw new ArgumentException("Os campos Password e PasswordConfirmation do request não coincidem.", nameof(request));

                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                    throw new ArgumentException("O campo Email do request já está em uso.", nameof(request.Email));

                if (request.Role == UserRoles.Admin)
                {
                    var configuredPassword = _configuration["AdminSettings:MasterPassword"];
                    if (string.IsNullOrWhiteSpace(request.MasterPassword) || request.MasterPassword != configuredPassword)
                    {
                        throw new ArgumentException("Senha mestre incorreta.", nameof(request.MasterPassword));
                    }
                }

                var user = request.AssignTo();
                user.Id = Guid.NewGuid().ToString();
                user.NormalizedEmail = request.Email.ToUpper();
                user.NormalizedUserName = request.UserName.ToUpper();

                _logger.LogInformation("[DEBUG] Role após AssignTo: {Role}", user.Role);

                // Use IIdentityAbstractor to create the user with ASP.NET Identity's password hashing
                var createResult = await _identityAbstractor.CreateUserAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create user: {Errors}", errors);
                    throw new Exception($"Erro ao criar usuário: {errors}");
                }

                // Assign the role
                var roleResult = await _identityAbstractor.AddToRoleAsync(user, request.Role);
                if (!roleResult.Succeeded)
                {
                    var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to assign role to user: {Errors}", roleErrors);
                    throw new Exception($"Erro ao atribuir papel ao usuário: {roleErrors}");
                }

                _logger.LogInformation("[DEBUG] Role após salvar: {Role}", user.Role);

                var roles = new List<string> { request.Role.ToString() };
                var token = _jwtTokenGenerator.GenerateToken(user, roles);

                return new CreateUserResult(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating user with email: {Email}", request.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email: {Email}", request.Email);
                throw new Exception("Erro ao criar o usuário. Tente novamente mais tarde.", ex);
            }
        }
    }
}