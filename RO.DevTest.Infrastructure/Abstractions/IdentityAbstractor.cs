// Caminho: C:\Users\André\teste\RO.DevTest3\RO.DevTest.Infrastructure\Abstractions\IdentityAbstractor.cs

using Microsoft.AspNetCore.Identity;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities; // Verifique se esta é a sua entidade User usada com UserManager
using RO.DevTest.Domain.Enums; // Necessário para UserRoles
using System; // Necessário para Guid, Task, NotImplementedException (para os métodos que ainda faltam)
using System.Collections.Generic; // Necessário para IList

namespace RO.DevTest.Infrastructure.Abstractions;

/// <summary>
/// This is a abstraction of the Identity library, creating methods that will interact with
/// it to create and update users
/// </summary>
public class IdentityAbstractor : IIdentityAbstractor
{
    private readonly UserManager<User> _userManager; // Assumindo que sua entidade User é o tipo usado pelo UserManager
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityAbstractor(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public async Task<User?> FindUserByEmailAsync(string email) => await _userManager.FindByEmailAsync(email);

    public async Task<User?> FindUserByIdAsync(string userId) => await _userManager.FindByIdAsync(userId);

    public async Task<IList<string>> GetUserRolesAsync(User user) => await _userManager.GetRolesAsync(user);

    public async Task<IdentityResult> CreateUserAsync(User partnerUser, string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException($"{nameof(password)} cannot be null or empty", nameof(password));
        }

        if (string.IsNullOrEmpty(partnerUser.Email))
        {
            throw new ArgumentException($"{nameof(User.Email)} cannot be null or empty", nameof(partnerUser));
        }

        return await _userManager.CreateAsync(partnerUser, password);
    }

    public async Task<SignInResult> PasswordSignInAsync(User user, string password)
        => await _signInManager.PasswordSignInAsync(user, password, false, false);

    public async Task<IdentityResult> DeleteUserAsync(User userToDelete) => await _userManager.DeleteAsync(userToDelete);

    public async Task<IdentityResult> AddToRoleAsync(User user, UserRoles role)
    {
        // Converte o enum UserRoles para string (nome do papel)
        string roleName = role.ToString();

        // Verifica se o papel existe e cria se não existir
        if (await _roleManager.RoleExistsAsync(roleName) is false)
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
        }

        // Adiciona o usuário ao papel
        return await _userManager.AddToRoleAsync(user, roleName);
    }

    // ===================================================================
    // >>> MÉTODOS QUE PRECISAM SER IMPLEMENTADOS <<<

    // Implementação para buscar usuário por GUID
    public async Task<User?> FindByIdAsync(Guid id)
    {
        // O UserManager.FindByIdAsync espera string, então convertemos o GUID para string
        return await _userManager.FindByIdAsync(id.ToString());
    }

    // Implementação para atualizar dados básicos do usuário
    public async Task<IdentityResult> UpdateUserAsync(User userToUpdate)
    {
        // O UserManager tem um método UpdateAsync para salvar as mudanças feitas na entidade User
        return await _userManager.UpdateAsync(userToUpdate);
    }

    // Implementação para remover um usuário de um papel
    // NOTA: Sua interface IIdentityAbstractor define o parâmetro 'originalRole' como 'object'.
    // Isso é incomum para papéis. Assumimos que você passará um valor do enum UserRoles
    // ou a string do nome do papel como 'object'. Vamos tentar converter para string.
    // >>> Considere fortemente mudar a assinatura na interface IIdentityAbstractor
    // para usar 'string roleName' ou 'UserRoles role' em vez de 'object'. <<<
    public async Task RemoveFromRoleAsync(User userToUpdate, object originalRole)
    {
        string? roleName = null;

        // Tenta converter o objeto para string (se for o nome do papel)
        if (originalRole is string strRole)
        {
            roleName = strRole;
        }
        // Tenta converter o objeto para o enum UserRoles e depois para string (se for o enum)
        else if (originalRole is UserRoles enumRole)
        {
            roleName = enumRole.ToString();
        }
        // Se o objeto não for nem string nem UserRoles, pode ser um erro, dependendo do que você espera passar.
        // Você pode adicionar um tratamento de erro mais robusto aqui se necessário.
        else
        {
            // Opcional: Tratar tipo inesperado, talvez lançar ArgumentException ou logar.
            // Por enquanto, apenas proceed se roleName for null, o que resultará em erro no RemoveFromRoleAsync
            // se o nome for null/empty, ou não fará nada se o UserManager lidar com null.
            // Para segurança, podemos lançar:
            // throw new ArgumentException("O parâmetro 'originalRole' deve ser do tipo string ou UserRoles.", nameof(originalRole));
        }


        if (!string.IsNullOrEmpty(roleName))
        {
            // Remove o usuário do papel. O método do UserManager retorna IdentityResult,
            // mas a assinatura na sua interface (Task) não espera um retorno.
            // Se a sua interface DEVE retornar Task<IdentityResult>, altere a assinatura aqui.
            // Se a interface DEVE retornar Task, você precisa lidar com o IdentityResult (ex: checar sucesso/falha).
            var result = await _userManager.RemoveFromRoleAsync(userToUpdate, roleName);

            // Como a sua interface atual RemoveFromRoleAsync retorna Task (void),
            // não podemos retornar o IdentityResult.
            // Se você precisa tratar falhas na remoção do papel, DEVERIA LANÇAR uma exceção aqui
            // se result.Succeeded for false e a sua interface retornar Task (void).
            // Se a sua interface *puder* retornar Task<IdentityResult>, mude a interface e a assinatura deste método.

            // Exemplo se a interface é Task (void) e você quer lançar erro em caso de falha:
            if (!result.Succeeded)
            {
                // Logar result.Errors ou converter para uma exceção apropriada
                // throw new InvalidOperationException($"Failed to remove role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                Console.WriteLine($"Aviso: Falha ao remover papel {roleName} para o usuário {userToUpdate.Id}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                // Decida se quer lançar um erro crítico ou apenas logar o aviso.
            }
        }
        else
        {
            // Opcional: Lidar com o caso onde roleName ficou nulo/vazio após tentar converter o objeto.
            // Isso pode acontecer se originalRole for um tipo inesperado.
            Console.WriteLine($"Aviso: Não foi possível determinar o nome do papel a partir do objeto fornecido para o usuário {userToUpdate.Id}. Tipo fornecido: {(originalRole != null ? originalRole.GetType().Name : "null")}");
        }
    }

    public Task<IdentityResult> DeleteUser(User user)
    {
        throw new NotImplementedException();
    }

    // <<< FIM DOS MÉTODOS QUE PRECISAVAM SER IMPLEMENTADOS >>>
    // ===================================================================

    // Se você tiver outros métodos na sua interface IIdentityAbstractor, implemente-os aqui também.
    // Ex: Um método para buscar papéis de um usuário (GetUserRolesAsync já existe), etc.
}