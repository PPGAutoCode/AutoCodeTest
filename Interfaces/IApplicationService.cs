
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing application-related operations.
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Creates a new application based on the provided data.
        /// </summary>
        /// <param name="createApplicationDto">Data transfer object containing the details for the new application.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateApplication(CreateApplicationDto createApplicationDto);

        /// <summary>
        /// Retrieves an application based on the provided request data.
        /// </summary>
        /// <param name="applicationRequestDto">Data transfer object containing the request details for the application.</param>
        /// <returns>An Application object representing the retrieved application.</returns>
        Task<Application> GetApplication(ApplicationRequestDto applicationRequestDto);

        /// <summary>
        /// Updates an existing application based on the provided data.
        /// </summary>
        /// <param name="updateApplicationDto">Data transfer object containing the details for updating the application.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateApplication(UpdateApplicationDto updateApplicationDto);

        /// <summary>
        /// Deletes an application based on the provided data.
        /// </summary>
        /// <param name="deleteApplicationDto">Data transfer object containing the details for deleting the application.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteApplication(DeleteApplicationDto deleteApplicationDto);

        /// <summary>
        /// Retrieves a list of applications based on the provided request data.
        /// </summary>
        /// <param name="listApplicationRequestDto">Data transfer object containing the request details for the list of applications.</param>
        /// <returns>A list of Application objects representing the retrieved applications.</returns>
        Task<List<Application>> GetListApplication(ListApplicationRequestDto listApplicationRequestDto);
    }
}
