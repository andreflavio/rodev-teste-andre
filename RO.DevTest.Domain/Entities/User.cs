// Caminho esperado: C:\Users\André\teste\RO.DevTest3\RO.DevTest.Domain\User.cs

using Microsoft.AspNetCore.Identity;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Domain.Entities;

/// <summary>
/// Represents a <see cref="IdentityUser"/> in the API
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// Name of the user
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role of the user (Admin or Customer)
    /// </summary>
    public UserRoles Role { get; set; } = UserRoles.Customer; // Agora esta propriedade será mapeada no banco

    public User() : base() { }
}
