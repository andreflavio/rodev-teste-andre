using FluentValidation;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(cpau => cpau.UserName)
            .NotEmpty()
            .WithMessage("O campo UserName é obrigatório.");

        RuleFor(cpau => cpau.Name)
            .NotEmpty()
            .WithMessage("O campo Name é obrigatório.");

        RuleFor(cpau => cpau.Email)
            .NotEmpty()
            .WithMessage("O campo Email é obrigatório.")
            .EmailAddress()
            .WithMessage("O campo Email deve ser um endereço de email válido.");

        RuleFor(cpau => cpau.Password)
            .NotEmpty()
            .WithMessage("O campo Password é obrigatório.")
            .MinimumLength(6)
            .WithMessage("O campo Password deve ter pelo menos 6 caracteres.");

        RuleFor(cpau => cpau.PasswordConfirmation)
            .NotEmpty()
            .WithMessage("O campo PasswordConfirmation é obrigatório.")
            .Equal(cpau => cpau.Password)
            .WithMessage("Os campos Password e PasswordConfirmation devem ser iguais.");
    }
}