// Caminho esperado: RO.DevTest.Application\Contracts\Persistence\Repositories\IUserRepository.cs

using RO.DevTest.Domain.Entities; // Necessário para IEnumerable<User>
using System.Collections.Generic;
using System.Threading.Tasks;
// Remova using RO.DevTest.Application.Features.User.Commands.GetAllUsersCommand; se não for usado para mais nada

namespace RO.DevTest.Application.Contracts.Persistence.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        // A interface deve ter APENAS esta assinatura para GetAllAsync
        // Ela retorna entidades de domínio (User) e aceita parâmetros de filtro (string?)
        Task<IEnumerable<User>> GetAllAsync(string? name = null, string? userName = null);

        // ** REMOVA QUALQUER OUTRA DECLARAÇÃO DE GetAllAsync AQUI **
        // (Incluindo a linha com parâmetros 'object' ou retornando GetAllUsersResult)

    }
}