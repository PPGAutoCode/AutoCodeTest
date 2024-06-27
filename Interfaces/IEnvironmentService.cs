
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Service interface for managing environment-related operations.
    /// </summary>
    public interface IEnvironmentService
    {
        /// <summary>
        /// Creates a new environment based on the provided data.
        /// </summary>
        /// <param name="createEnvironmentDto">Data transfer object containing the details for the new environment.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateEnvironment(CreateEnvironmentDto createEnvironmentDto);

        /// <summary>
        /// Retrieves an environment based on the provided request data.
        /// </summary>
        /// <param name="environmentRequestDto">Data transfer object containing the request details for the environment.</param>
        /// <returns>An Environment object representing the requested environment.</returns>
        Task<Environment> GetEnvironment(EnvironmentRequestDto environmentRequestDto);

        /// <summary>
        /// Updates an existing environment based on the provided data.
        /// </summary>
        /// <param name="updateEnvironmentDto">Data transfer object containing the details for updating the environment.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateEnvironment(UpdateEnvironmentDto updateEnvironmentDto);

        /// <summary>
        /// Deletes an environment based on the provided data.
        /// </summary>
        /// <param name="deleteEnvironmentDto">Data transfer object containing the details for deleting the environment.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteEnvironment(DeleteEnvironmentDto deleteEnvironmentDto);

        /// <summary>
        /// Retrieves a list of environments based on the provided request data.
        /// </summary>
        /// <param name="listEnvironmentRequestDto">Data transfer object containing the request details for the list of environments.</param>
        /// <returns>A list of Environment objects representing the requested environments.</returns>
        Task<List<Environment>> GetListEnvironment(ListEnvironmentRequestDto listEnvironmentRequestDto);
    }
}
