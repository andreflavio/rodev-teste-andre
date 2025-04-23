// Caminho esperado: RO.DevTest.Application\Features\User\Commands\UpdateUserCommand\UpdateUserCommandValidator.cs

using FluentValidation;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand; // Para referenciar o UpdateUserCommand
using RO.DevTest.Domain.Enums; // Para referenciar UserRoles
using System; // Necessário para Enum.IsDefined

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand; // O namespace segue o padrão Feature.Entity.Operation.Command

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        // Regra 1: O ID do usuário a ser atualizado não pode ser vazio
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório para a atualização.");

        // Regra 2: O Nome do usuário não pode ser vazio e ter um tamanho máximo
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("O campo Nome precisa ser preenchido.")
            .MaximumLength(100) // Exemplo de tamanho máximo, ajuste se necessário
            .WithMessage("O campo Nome não pode exceder 100 caracteres.");

        // Regra 3: O UserName do usuário não pode ser vazio e ter um tamanho máximo
        RuleFor(command => command.UserName)
            .NotEmpty()
            .WithMessage("O campo UserName precisa ser preenchido.")
            .MaximumLength(50) // Exemplo de tamanho máximo, ajuste se necessário
            .WithMessage("O campo UserName não pode exceder 50 caracteres.");

        // Regra 4: O Email precisa ser preenchido e ser um email válido
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("O campo E-mail precisa ser preenchido.")
            .EmailAddress()
            .WithMessage("O campo E-mail precisa ser um e-mail válido.");

        // Regra 5: O Papel (Role) precisa ser um valor válido do enum UserRoles
        RuleFor(command => command.Role)
            .Must(role => Enum.IsDefined(typeof(UserRoles), role))
            .WithMessage("O Papel (Role) do usuário é inválido.");

        // NOTA: Não incluímos validações para Senha ou Confirmação de Senha
        // pois elas não fazem parte do UpdateUserCommand.
    }
}