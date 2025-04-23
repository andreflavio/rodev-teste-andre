// Caminho esperado: RO.DevTest.Application\Features\User\Commands\DeleteUserCommand\DeleteUserCommand.cs

using MediatR;
using System; // Necessário para Guid

namespace RO.DevTest.Application.Features.User.Commands.DeleteUserCommand; // O namespace segue o padrão Feature.Entity.Operation.Command

public class DeleteUserCommand : IRequest<DeleteUserResult>
{
    // ID do usuário a ser deletado - ESSENCIAL
    public Guid Id { get; set; }
}

public class DeleteUserResult
{
    public bool Success { get; internal set; }
    public string? Message { get; internal set; }
}