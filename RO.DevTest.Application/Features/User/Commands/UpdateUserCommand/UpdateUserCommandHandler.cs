// Caminho esperado: RO.DevTest.Application\Features\User\Commands\UpdateUserCommand\UpdateUserCommandHandler.cs

using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity; // Pode ser necessário se IIdentityAbstractor retornar IdentityResult ou tipos similares
using RO.DevTest.Application.Contracts.Infrastructure; // Para IIdentityAbstractor
using RO.DevTest.Domain.Exception; // Para BadRequestException, NotFoundException (se você criar)
using RO.DevTest.Domain.Enums; // Para UserRoles
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand; // Para o Command e Result
using RO.DevTest.Domain.Entities; // Para a entidade User
using System.Threading;
using System.Threading.Tasks;
using System; // Necessário para Exception

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand; // O namespace segue o padrão

/// <summary>
/// Command handler for updating an existing <see cref="Domain.Entities.User"/>
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IIdentityAbstractor _identityAbstractor;

    // Usando injeção de dependência via construtor tradicional para consistência com GetAllUsersCommandHandler
    public UpdateUserCommandHandler(IIdentityAbstractor identityAbstractor)
    {
        _identityAbstractor = identityAbstractor;
    }

    public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar o comando de entrada
        // *** Agora a classe UpdateUserCommandValidator DEVE estar em um arquivo separado e ser PUBLIC ***
        UpdateUserCommandValidator validator = new(); // Instanciação direta como no CreateUserCommandHandler
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            // Lança exceção em caso de falha na validação, seguindo o padrão do Create
            throw new BadRequestException(validationResult);
        }

        // 2. Buscar o usuário existente pelo ID usando o IdentityAbstractor
        // Assumindo que IIdentityAbstractor tem um método para buscar usuário por GUID ou string ID.
        // Se o ID do usuário é Guid no seu Domain.User, IIdentityAbstractor.FindByIdAsync deve suportar Guid
        // Se o ID do usuário é string, use request.Id.ToString()
        var userToUpdate = await _identityAbstractor.FindByIdAsync(request.Id); // <-- VERIFIQUE A ASSINATURA EXATA AQUI (Guid ou string)

        // 3. Lidar com o caso do usuário não ser encontrado
        if (userToUpdate == null)
        {
            // Lança BadRequestException para consistência, embora NotFoundException seria semanticamente mais correto se você a tiver
            throw new BadRequestException($"Usuário com ID {request.Id} não encontrado.");
        }

        // 4. Obter o papel atual do usuário antes de qualquer modificação
        // Assumindo que a entidade User carregada por IIdentityAbstractor tem a propriedade Role populada
        // Se IIdentityAbstractor não popula Role, você precisaria buscar os papéis do usuário explicitamente aqui
        var originalRole = userToUpdate.Role; // <-- VERIFIQUE SE ESTA PROPRIEDADE ESTÁ POPULADA

        // 5. Aplicar as mudanças básicas (Nome, Email, UserName) na entidade rastreada
        // Evitamos modificar a propriedade Role aqui se o IdentityAbstractor a gerencia separadamente
        userToUpdate.Name = request.Name;
        userToUpdate.Email = request.Email;
        userToUpdate.UserName = request.UserName; // Atualiza UserName se necessário

        // 6. Persistir as mudanças básicas do usuário usando o IdentityAbstractor
        // Assumindo que IIdentityAbstractor tem um método para atualizar o usuário que retorna IdentityResult
        var updateIdentityResult = await _identityAbstractor.UpdateUserAsync(userToUpdate); // <-- VERIFIQUE A ASSINATURA EXATA AQUI

        if (!updateIdentityResult.Succeeded)
        {
            // Lida com erros retornados pelo IdentityAbstractor (duplicidade de email/username, etc.)
            throw new BadRequestException(updateIdentityResult); // Passa o IdentityResult para a exceção
        }

        // 7. Lidar com a atualização do papel, se houver mudança
        if (originalRole != request.Role)
        {
            // Remover o papel antigo (se diferente do None/Default)
            if (originalRole != UserRoles.None) // Adapte a condição se tiver um valor default diferente
            {
                try
                {
                    // Chama o método, AGUARDA, mas não atribui o resultado a uma variável
                    await _identityAbstractor.RemoveFromRoleAsync(userToUpdate, originalRole); // <-- VERIFIQUE A ASSINATURA EXATA AQUI (UserRoles ou string)
                                                                                               // Se o método não lançou exceção, assumimos sucesso
                }
                catch (Exception ex)
                {
                    // Captura qualquer exceção lançada por RemoveFromRoleAsync em caso de falha
                    // Adapte o tratamento de erro ou o tipo de exceção capturada conforme o necessário
                    throw new BadRequestException($"Falha ao remover papel antigo '{originalRole}' para o usuário {userToUpdate.Id}. Detalhe: {ex.Message}");
                }
            }

            // Adicionar o novo papel (se diferente do None/Default)
            if (request.Role != UserRoles.None) // Adapte a condição
            {
                try
                {
                    // Chama o método, AGUARDA, mas não atribui o resultado a uma variável
                    await _identityAbstractor.AddToRoleAsync(userToUpdate, request.Role); // <-- VERIFIQUE A ASSINATURA EXATA AQUI (UserRoles ou string)
                    // Se o método não lançou exceção, assumimos sucesso
                }
                catch (Exception ex)
                {
                    // Captura qualquer exceção lançada por AddToRoleAsync em caso de falha
                    // Adapte o tratamento de erro ou o tipo de exceção capturada conforme o necessário
                    throw new BadRequestException($"Falha ao adicionar novo papel '{request.Role}' para o usuário {userToUpdate.Id}. Detalhe: {ex.Message}");
                }
            }
        }

        // 8. Retornar um resultado de sucesso
        return new UpdateUserResult
        {
            Success = true,
            Message = $"Usuário com ID {request.Id} atualizado com sucesso."
        };
    }
}