
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Service interface for managing allowed grant types.
    /// </summary>
    public interface IAllowedGrantTypeService
    {
        /// <summary>
        /// Creates a new allowed grant type.
        /// </summary>
        /// <param name="createAllowedGrantTypeDto">The data transfer object containing the information for the new allowed grant type.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateAllowedGrantType(CreateAllowedGrantTypeDto createAllowedGrantTypeDto);

        /// <summary>
        /// Retrieves an allowed grant type based on the provided request data.
        /// </summary>
        /// <param name="allowedGrantTypeRequestDto">The data transfer object containing the request information.</param>
        /// <returns>An instance of AllowedGrantType representing the retrieved grant type.</returns>
        Task<AllowedGrantType> GetAllowedGrantType(AllowedGrantTypeRequestDto allowedGrantTypeRequestDto);

        /// <summary>
        /// Updates an existing allowed grant type.
        /// </summary>
        /// <param name="updateAllowedGrantTypeDto">The data transfer object containing the updated information for the allowed grant type.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateAllowedGrantType(UpdateAllowedGrantTypeDto updateAllowedGrantTypeDto);

        /// <summary>
        /// Deletes an allowed grant type based on the provided request data.
        /// </summary>
        /// <param name="deleteAllowedGrantTypeDto">The data transfer object containing the information for the grant type to be deleted.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteAllowedGrantType(DeleteAllowedGrantTypeDto deleteAllowedGrantTypeDto);

        /// <summary>
        /// Retrieves a list of allowed grant types based on the provided request data.
        /// </summary>
        /// <param name="listAllowedGrantTypeRequestDto">The data transfer object containing the request information.</param>
        /// <returns>A list of AllowedGrantType instances representing the retrieved grant types.</returns>
        Task<List<AllowedGrantType>> GetListAllowedGrantType(ListAllowedGrantTypeRequestDto listAllowedGrantTypeRequestDto);
    }
}
