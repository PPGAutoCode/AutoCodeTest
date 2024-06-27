
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing user-related operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user based on the provided request data.
        /// </summary>
        /// <param name="request">The data needed to create a user.</param>
        /// <returns>A string indicating the result of the user creation process.</returns>
        Task<string> CreateUser(CreateUserRequestDTO request);

        /// <summary>
        /// Retrieves a user based on the provided request data.
        /// </summary>
        /// <param name="request">The data needed to identify the user.</param>
        /// <returns>The user object if found.</returns>
        Task<User> GetUser(UserRequestDTO request);

        /// <summary>
        /// Updates an existing user based on the provided request data.
        /// </summary>
        /// <param name="request">The data needed to update the user.</param>
        /// <returns>A string indicating the result of the user update process.</returns>
        Task<string> UpdateUser(UpdateUserRequestDTO request);

        /// <summary>
        /// Deletes a user based on the provided request data.
        /// </summary>
        /// <param name="request">The data needed to identify the user to be deleted.</param>
        /// <returns>A boolean indicating whether the user was successfully deleted.</returns>
        Task<bool> DeleteUser(DeleteUserRequestDTO request);

        /// <summary>
        /// Retrieves a list of users based on the provided request data.
        /// </summary>
        /// <param name="request">The data needed to filter the list of users.</param>
        /// <returns>A list of user objects.</returns>
        Task<List<User>> GetListUser(ListUserRequestDTO request);
    }
}
