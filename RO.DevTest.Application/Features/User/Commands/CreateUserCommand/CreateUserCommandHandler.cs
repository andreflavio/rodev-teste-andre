using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand
{
    /// <summary>
    /// Command handler for the creation of <see cref="Domain.Entities.User"/>
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly IIdentityAbstractor _identityAbstractor;

        // Construtor correto para injeção de dependência
        public CreateUserCommandHandler(IIdentityAbstractor identityAbstractor)
        {
            _identityAbstractor = identityAbstractor ?? throw new ArgumentNullException(nameof(identityAbstractor));
        }

        public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Validar o request
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "O objeto request não pode ser nulo.");
            }

            // Validar o request com FluentValidation
            CreateUserCommandValidator validator = new();
            ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

            // Criar o novo usuário a partir do request
            Domain.Entities.User newUser = request.AssignTo();

            // Verificação se a criação do novo usuário falhou
            if (newUser == null)
            {
                throw new InvalidOperationException("Falha ao criar o usuário a partir da solicitação.");
            }

            // Criar o usuário na identidade
            IdentityResult userCreationResult = await _identityAbstractor.CreateUserAsync(newUser, request.Password);

            // Verifique se o resultado da criação é válido
            if (userCreationResult == null)
            {
                throw new InvalidOperationException("O resultado da criação do usuário não pode ser nulo.");
            }

            // Verifique se a criação do usuário foi bem-sucedida
            if (!userCreationResult.Succeeded)
            {
                throw new BadRequestException(userCreationResult);
            }

            // Adicionar o usuário ao papel (role)
            IdentityResult userRoleResult = await _identityAbstractor.AddToRoleAsync(newUser, request.Role);

            // Verifique se o resultado de adicionar o papel é válido
            if (userRoleResult == null)
            {
                throw new InvalidOperationException("O resultado de adicionar o papel ao usuário não pode ser nulo.");
            }

            if (!userRoleResult.Succeeded)
            {
                throw new BadRequestException(userRoleResult);
            }

            // Retornar o resultado com o novo usuário criado
            return new CreateUserResult(newUser);
        }
    }
}
