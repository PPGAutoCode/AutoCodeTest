
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing GoLive developer types.
    /// </summary>
    public interface IGoLiveDeveloperTypeService
    {
        /// <summary>
        /// Creates a new GoLive developer type.
        /// </summary>
        /// <param name="createGoLiveDeveloperTypeDto">The data transfer object containing the details of the GoLive developer type to create.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateGoLiveDeveloperTypeAsync(CreateGoLiveDeveloperTypeDto createGoLiveDeveloperTypeDto);

        /// <summary>
        /// Retrieves a GoLive developer type based on the provided request data.
        /// </summary>
        /// <param name="goLiveDeveloperTypeRequestDto">The data transfer object containing the request details to retrieve the GoLive developer type.</param>
        /// <returns>A GoLiveDeveloperType object representing the retrieved developer type.</returns>
        Task<GoLiveDeveloperType> GetGoLiveDeveloperTypeAsync(GoLiveDeveloperTypeRequestDto goLiveDeveloperTypeRequestDto);

        /// <summary>
        /// Updates an existing GoLive developer type.
        /// </summary>
        /// <param name="updateGoLiveDeveloperTypeDto">The data transfer object containing the details of the GoLive developer type to update.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateGoLiveDeveloperTypeAsync(UpdateGoLiveDeveloperTypeDto updateGoLiveDeveloperTypeDto);

        /// <summary>
        /// Deletes a GoLive developer type based on the provided request data.
        /// </summary>
        /// <param name="deleteGoLiveDeveloperTypeDto">The data transfer object containing the details of the GoLive developer type to delete.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteGoLiveDeveloperTypeAsync(DeleteGoLiveDeveloperTypeDto deleteGoLiveDeveloperTypeDto);

        /// <summary>
        /// Retrieves a list of GoLive developer types based on the provided request data.
        /// </summary>
        /// <param name="listGoLiveDeveloperTypeRequestDto">The data transfer object containing the request details to retrieve the list of GoLive developer types.</param>
        /// <returns>A list of GoLiveDeveloperType objects representing the retrieved developer types.</returns>
        Task<List<GoLiveDeveloperType>> GetListGoLiveDeveloperTypeAsync(ListGoLiveDeveloperTypeRequestDto listGoLiveDeveloperTypeRequestDto);
    }
}
