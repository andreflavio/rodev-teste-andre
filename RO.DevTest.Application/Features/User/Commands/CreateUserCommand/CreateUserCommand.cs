using MediatR;
using RO.DevTest.Domain.Enums;
using Entities = RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand
{
    public class CreateUserCommand : IRequest<CreateUserResult>
    {
        public string UserName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PasswordConfirmation { get; set; } = null!;
        public UserRoles Role { get; set; } = UserRoles.Customer;  // O papel pode ser alterado dependendo da lógica

        // A senha mestre será passada diretamente
        public string? MasterPassword { get; set; }

        public Entities.User AssignTo()
        {
            return new Entities.User
            {
                UserName = UserName,
                Email = Email,
                Name = Name,
                Role = Role // Atribui o papel passado no comando
            };
        }
    }
}
