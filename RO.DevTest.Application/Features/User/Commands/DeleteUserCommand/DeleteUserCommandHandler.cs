// Caminho esperado: RO.DevTest.Application\Features\User\Commands\DeleteUserCommand\DeleteUserCommandHandler.cs

using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure; // Para IIdentityAbstractor
using RO.DevTest.Domain.Exception; // Para BadRequestException, NotFoundException
using System.Threading;
using System.Threading.Tasks;
using System; // Necessário para Exception

namespace RO.DevTest.Application.Features.User.Commands.DeleteUserCommand; // O namespace segue o padrão

/// <summary>
/// Command handler for deleting an existing <see cref="Domain.Entities.User"/>
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserResult>
{
    private readonly IIdentityAbstractor _identityAbstractor;

    public DeleteUserCommandHandler(IIdentityAbstractor identityAbstractor)
    {
        _identityAbstractor = identityAbstractor;
    }

    public async Task<DeleteUserResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar o usuário existente pelo ID usando o IdentityAbstractor
        var userToDelete = await _identityAbstractor.FindByIdAsync(request.Id);

        // 2. Lidar com o caso do usuário não ser encontrado
        if (userToDelete == null)
        {
            // Lança BadRequestException para consistência, embora NotFoundException seria semanticamente mais correto
            throw new BadRequestException($"Usuário com ID {request.Id} não encontrado.");
        }

        // 3. Tentar deletar o usuário usando o IdentityAbstractor
        var deleteResult = await _identityAbstractor.DeleteUserAsync(userToDelete); // <-- Assumindo que este método existe em IIdentityAbstractor

        // 4. Verificar se a deleção foi bem-sucedida
        if (deleteResult.Succeeded)
        {
            return new DeleteUserResult
            {
                Success = true,
                Message = $"Usuário com ID {request.Id} deletado com sucesso."
            };
        }
        else
        {
            // Lidar com erros de deleção retornados pelo IdentityAbstractor
            throw new BadRequestException(deleteResult);
        }
    }
}