
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing application statuses.
    /// </summary>
    public interface IAppStatusService
    {
        /// <summary>
        /// Creates a new application status.
        /// </summary>
        /// <param name="createAppStatusDto">The data transfer object containing the details of the new application status.</param>
        /// <returns>A string representing the result of the operation.</returns>
        Task<string> CreateAppStatus(CreateAppStatusDto createAppStatusDto);

        /// <summary>
        /// Retrieves the application status based on the provided request.
        /// </summary>
        /// <param name="appStatusRequestDto">The data transfer object containing the request details.</param>
        /// <returns>An AppStatus object representing the application status.</returns>
        Task<AppStatus> GetAppStatus(AppStatusRequestDto appStatusRequestDto);

        /// <summary>
        /// Updates an existing application status.
        /// </summary>
        /// <param name="updateAppStatusDto">The data transfer object containing the details to update the application status.</param>
        /// <returns>A string representing the result of the operation.</returns>
        Task<string> UpdateAppStatus(UpdateAppStatusDto updateAppStatusDto);

        /// <summary>
        /// Deletes an application status.
        /// </summary>
        /// <param name="deleteAppStatusDto">The data transfer object containing the details to delete the application status.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> DeleteAppStatus(DeleteAppStatusDto deleteAppStatusDto);

        /// <summary>
        /// Retrieves a list of application statuses based on the provided request.
        /// </summary>
        /// <param name="listAppStatusRequestDto">The data transfer object containing the request details.</param>
        /// <returns>A list of AppStatus objects representing the application statuses.</returns>
        Task<List<AppStatus>> GetListAppStatus(ListAppStatusRequestDto listAppStatusRequestDto);
    }
}
