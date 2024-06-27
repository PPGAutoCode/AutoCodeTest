
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing application tags.
    /// </summary>
    public interface IAppTagService
    {
        /// <summary>
        /// Creates a new application tag.
        /// </summary>
        /// <param name="createAppTagDto">The data transfer object containing the details of the tag to be created.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateAppTag(CreateAppTagDto createAppTagDto);

        /// <summary>
        /// Retrieves a specific application tag.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the request details to fetch the tag.</param>
        /// <returns>An AppTag object representing the retrieved tag.</returns>
        Task<AppTag> GetAppTag(AppTagRequestDto requestDto);

        /// <summary>
        /// Updates an existing application tag.
        /// </summary>
        /// <param name="updateAppTagDto">The data transfer object containing the details of the tag to be updated.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateAppTag(UpdateAppTagDto updateAppTagDto);

        /// <summary>
        /// Deletes a specific application tag.
        /// </summary>
        /// <param name="deleteAppTagDto">The data transfer object containing the details of the tag to be deleted.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteAppTag(DeleteAppTagDto deleteAppTagDto);

        /// <summary>
        /// Retrieves a list of application tags based on the provided request details.
        /// </summary>
        /// <param name="requestDto">The data transfer object containing the request details to fetch the list of tags.</param>
        /// <returns>A list of AppTag objects representing the retrieved tags.</returns>
        Task<List<AppTag>> GetListAppTag(ListAppTagRequestDto requestDto);
    }
}
