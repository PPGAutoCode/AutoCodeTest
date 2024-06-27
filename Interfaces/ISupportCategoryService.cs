
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing support categories.
    /// </summary>
    public interface ISupportCategoryService
    {
        /// <summary>
        /// Creates a new support category.
        /// </summary>
        /// <param name="createSupportCategoryDto">Data transfer object for creating a support category.</param>
        /// <returns>The ID of the created support category.</returns>
        Task<string> CreateSupportCategoryAsync(CreateSupportCategoryDto createSupportCategoryDto);

        /// <summary>
        /// Retrieves a support category by its request details.
        /// </summary>
        /// <param name="supportCategoryRequestDto">Data transfer object for requesting a support category.</param>
        /// <returns>The requested support category.</returns>
        Task<SupportCategory> GetSupportCategoryAsync(SupportCategoryRequestDto supportCategoryRequestDto);

        /// <summary>
        /// Updates an existing support category.
        /// </summary>
        /// <param name="updateSupportCategoryDto">Data transfer object for updating a support category.</param>
        /// <returns>The ID of the updated support category.</returns>
        Task<string> UpdateSupportCategoryAsync(UpdateSupportCategoryDto updateSupportCategoryDto);

        /// <summary>
        /// Deletes a support category.
        /// </summary>
        /// <param name="deleteSupportCategoryDto">Data transfer object for deleting a support category.</param>
        /// <returns>True if the support category was deleted successfully, otherwise false.</returns>
        Task<bool> DeleteSupportCategoryAsync(DeleteSupportCategoryDto deleteSupportCategoryDto);

        /// <summary>
        /// Retrieves a list of support categories based on the request details.
        /// </summary>
        /// <param name="listSupportCategoryRequestDto">Data transfer object for requesting a list of support categories.</param>
        /// <returns>A list of support categories.</returns>
        Task<List<SupportCategory>> GetListSupportCategoryAsync(ListSupportCategoryRequestDto listSupportCategoryRequestDto);
    }
}
