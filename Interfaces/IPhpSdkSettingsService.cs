
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Service interface for managing PHP SDK settings.
    /// </summary>
    public interface IPhpSdkSettingsService
    {
        /// <summary>
        /// Creates a new PHP SDK settings entry.
        /// </summary>
        /// <param name="createPhpSdkSettingsDto">The data transfer object containing the settings to create.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreatePhpSdkSettings(CreatePhpSdkSettingsDto createPhpSdkSettingsDto);

        /// <summary>
        /// Retrieves PHP SDK settings based on the provided request.
        /// </summary>
        /// <param name="phpSdkSettingsRequestDto">The data transfer object containing the request parameters.</param>
        /// <returns>A PHP SDK settings object.</returns>
        Task<PhpSdkSettings> GetPhpSdkSettings(PhpSdkSettingsRequestDto phpSdkSettingsRequestDto);

        /// <summary>
        /// Updates an existing PHP SDK settings entry.
        /// </summary>
        /// <param name="updatePhpSdkSettingsDto">The data transfer object containing the settings to update.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdatePhpSdkSettings(UpdatePhpSdkSettingsDto updatePhpSdkSettingsDto);

        /// <summary>
        /// Deletes a PHP SDK settings entry.
        /// </summary>
        /// <param name="deletePhpSdkSettingsDto">The data transfer object containing the settings to delete.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeletePhpSdkSettings(DeletePhpSdkSettingsDto deletePhpSdkSettingsDto);
    }
}
