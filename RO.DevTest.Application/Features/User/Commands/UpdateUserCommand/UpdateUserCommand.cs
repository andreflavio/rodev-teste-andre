// Caminho esperado: RO.DevTest.Application\Features\User\Commands\UpdateUserCommand\UpdateUserCommand.cs

using MediatR;
using RO.DevTest.Domain.Enums;
using System; // Necessário para Guid

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand; // O namespace segue o padrão Feature.Entity.Operation.Command

public class UpdateUserCommand : IRequest<UpdateUserResult>
{
    // ID do usuário a ser atualizado - ESSENCIAL para um comando de update
    public Guid Id { get; set; }

    // Campos que você permite atualizar.
    // Geralmente não inclui senha neste comando.
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRoles Role { get; set; } // Assumindo que o papel pode ser atualizado

    // Não precisamos de um método AssignTo aqui
    // pois a lógica de atualização aplicará essas mudanças
    // a uma entidade User *existente*, não criando uma nova.
}

public class UpdateUserResult
{
    public bool Success { get; internal set; }
    public string? Message { get; internal set; }
}