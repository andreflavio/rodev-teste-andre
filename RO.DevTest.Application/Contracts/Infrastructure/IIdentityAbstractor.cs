using Microsoft.AspNetCore.Identity;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Contracts.Infrastructure
{
    /// <summary>
    /// This is an abstraction of the Identity library, creating methods that will interact with
    /// it to create and update users.
    /// </summary>
    public interface IIdentityAbstractor
    {
        /// <summary>
        /// Finds a <see cref="User"/> through its ID asynchronously.
        /// </summary>
        /// <param name="userId">The <see cref="User"/>'s ID.</param>
        /// <returns>The <see cref="User"/> if found, <see cref="null"/> otherwise.</returns>
        Task<User?> FindByIdAsync(Guid userId);

        /// <summary>
        /// Finds a <see cref="User"/> through its email asynchronously.
        /// </summary>
        /// <param name="email">The <see cref="User"/>'s email.</param>
        /// <returns>The <see cref="User"/> if found, <see cref="null"/> otherwise.</returns>
        Task<User?> FindUserByEmailAsync(string email);

        /// <summary>
        /// Gets the names of the roles that a <see cref="User"/> has asynchronously.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to get the roles.</param>
        /// <returns>A <see cref="IList{T}"/> with the names of the roles.</returns>
        Task<IList<string>> GetUserRolesAsync(User user);

        /// <summary>
        /// Signs in a <see cref="User"/> asynchronously in a non-persistent way.
        /// The <see cref="User"/>'s account is not locked if failed.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to sign in.</param>
        /// <param name="password">The <see cref="User"/>'s password.</param>
        /// <returns>A <see cref="SignInResult"/>.</returns>
        Task<SignInResult> PasswordSignInAsync(User user, string password);

        /// <summary>
        /// Creates a <see cref="User"/> asynchronously and returns the <see cref="IdentityResult"/> of it.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to be added.</param>
        /// <param name="password">The plain text of the password to be used to hash it.</param>
        /// <returns>The <see cref="IdentityResult"/>.</returns>
        Task<IdentityResult> CreateUserAsync(User user, string password);

        /// <summary>
        /// Adds a <see cref="User"/> to a role asynchronously.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to add to the role.</param>
        /// <param name="role">The role to assign.</param>
        /// <returns>The <see cref="IdentityResult"/>.</returns>
        Task<IdentityResult> AddToRoleAsync(User user, UserRoles role);

        /// <summary>
        /// Deletes a <see cref="User"/> from the database.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to be deleted.</param>
        /// <returns>A <see cref="Task{IdentityResult}"/>.</returns>
        Task<IdentityResult> DeleteUserAsync(User user);

        /// <summary>
        /// Updates a <see cref="User"/> in the database.
        /// </summary>
        /// <param name="userToUpdate">The <see cref="User"/> to be updated.</param>
        /// <returns>A <see cref="Task{IdentityResult}"/>.</returns>
        Task<IdentityResult> UpdateUserAsync(User userToUpdate);

        /// <summary>
        /// Removes a <see cref="User"/> from a role.
        /// </summary>
        /// <param name="userToUpdate">The <see cref="User"/> to remove from the role.</param>
        /// <param name="originalRole">The role to remove.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task RemoveFromRoleAsync(User userToUpdate, object originalRole);

        /// <summary>
        /// Deprecated method for deleting a user (replaced by DeleteUserAsync).
        /// </summary>
        Task<IdentityResult> DeleteUser(User user);
    }
}