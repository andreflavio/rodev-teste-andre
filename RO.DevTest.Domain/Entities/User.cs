// Caminho esperado: C:\Users\André\teste\RO.DevTest3\RO.DevTest.Domain\User.cs

using Microsoft.AspNetCore.Identity;
using RO.DevTest.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema; // <-- ADICIONE ESTE USING

namespace RO.DevTest.Domain.Entities;

/// <summary>
/// Represents a <see cref="IdentityUser"/> int the API
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// Name of the user
    /// </summary>
    public string Name { get; set; } = string.Empty;

    // Adicione [NotMapped] aqui para dizer ao Entity Framework para ignorar esta propriedade no banco de dados
    [NotMapped] // <-- ADICIONE ESTA LINHA
    public UserRoles Role { get; set; } // <-- Sua propriedade Role existente

    public User() : base() { }

    // Se você tiver outros construtores ou métodos na sua entidade User, mantenha-os aqui.
}