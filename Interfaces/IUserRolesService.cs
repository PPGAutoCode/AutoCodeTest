
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
        /// <param name="createUserRoleDTO">Data transfer object containing the information needed to create a user role.</param>
        /// <returns>A string representing the result of the operation.</returns>
        Task<string> CreateUserRole(CreateUserRoleDTO createUserRoleDTO);

        /// <summary>
        /// Updates an existing user role.
        /// </summary>
        /// <param name="updateUserRoleDTO">Data transfer object containing the information needed to update a user role.</param>
        /// <returns>A string representing the result of the operation.</returns>
        Task<string> UpdateUserRole(UpdateUserRoleDTO updateUserRoleDTO);

        /// <summary>
        /// Deletes a user role.
        /// </summary>
        /// <param name="deleteUserRoleDTO">Data transfer object containing the information needed to delete a user role.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> DeleteUserRole(DeleteUserRoleDTO deleteUserRoleDTO);

        /// <summary>
        /// Retrieves a list of user roles based on the provided request.
        /// </summary>
        /// <param name="userRolesRequestDTO">Data transfer object containing the request parameters.</param>
        /// <returns>A list of user roles.</returns>
        Task<List<UserRoles>> GetListUserRoles(UserRolesRequestDTO userRolesRequestDTO);

        /// <summary>
        /// Retrieves a single user role based on the provided request.
        /// </summary>
        /// <param name="userRolesRequestDTO">Data transfer object containing the request parameters.</param>
        /// <returns>A user role object.</returns>
        Task<UserRoles> GetUserRole(UserRolesRequestDTO userRolesRequestDTO);
    }
}
