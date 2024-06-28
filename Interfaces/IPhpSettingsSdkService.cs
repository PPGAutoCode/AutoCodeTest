
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Service interface for managing PHP settings SDK operations.
    /// </summary>
    public interface IPhpSettingsSdkService
    {
        /// <summary>
        /// Creates a new PHP settings SDK configuration.
        /// </summary>
        /// <param name="createPhpSettingsSdkDto">Data transfer object containing the details for creating a new PHP settings SDK.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreatePhpSettingsSdk(CreatePhpSettingsSdkDto createPhpSettingsSdkDto);

        /// <summary>
        /// Retrieves the PHP settings SDK configuration based on the provided request details.
        /// </summary>
        /// <param name="phpSettingsSdkRequestDto">Data transfer object containing the request details for retrieving the PHP settings SDK.</param>
        /// <returns>A PhpSettingsSdk object representing the retrieved configuration.</returns>
        Task<PhpSettingsSdk> GetPhpSettingsSdk(PhpSettingsSdkRequestDto phpSettingsSdkRequestDto);

        /// <summary>
        /// Updates an existing PHP settings SDK configuration.
        /// </summary>
        /// <param name="updatePhpSettingsSdkDto">Data transfer object containing the details for updating the PHP settings SDK.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdatePhpSettingsSdk(UpdatePhpSettingsSdkDto updatePhpSettingsSdkDto);

        /// <summary>
        /// Deletes a PHP settings SDK configuration.
        /// </summary>
        /// <param name="deletePhpSettingsSdkDto">Data transfer object containing the details for deleting the PHP settings SDK.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeletePhpSettingsSdk(DeletePhpSettingsSdkDto deletePhpSettingsSdkDto);
    }
}
