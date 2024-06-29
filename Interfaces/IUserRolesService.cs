
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing user roles.
    /// </summary>
    public interface IUserRolesService
    {
        /// <summary>
        /// Creates a new user role.
        /// </summary>
        /// <param name="createUserRoleDto">The data transfer object containing the details of the user role to be created.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateUserRole(CreateUserRoleDto createUserRoleDto);

        /// <summary>
        /// Updates an existing user role.
        /// </summary>
        /// <param name="updateUserRoleDto">The data transfer object containing the details of the user role to be updated.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateUserRole(UpdateUserRoleDto updateUserRoleDto);

        /// <summary>
        /// Deletes a user role.
        /// </summary>
        /// <param name="deleteUserRoleDto">The data transfer object containing the details of the user role to be deleted.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteUserRole(DeleteUserRoleDto deleteUserRoleDto);

        /// <summary>
        /// Retrieves a list of user roles based on the provided request data.
        /// </summary>
        /// <param name="userRolesRequestDto">The data transfer object containing the request parameters.</param>
        /// <returns>A list of user roles.</returns>
        Task<List<UserRole>> GetListUserRoles(UserRolesRequestDto userRolesRequestDto);

        /// <summary>
        /// Retrieves a single user role based on the provided request data.
        /// </summary>
        /// <param name="userRolesRequestDto">The data transfer object containing the request parameters.</param>
        /// <returns>A single user role.</returns>
        Task<UserRole> GetUserRole(UserRolesRequestDto userRolesRequestDto);
    }
}
