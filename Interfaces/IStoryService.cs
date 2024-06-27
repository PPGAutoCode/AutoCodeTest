
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing story-related operations.
    /// </summary>
    public interface IStoryService
    {
        /// <summary>
        /// Creates a new story based on the provided data.
        /// </summary>
        /// <param name="createStoryDto">Data transfer object containing the details for the new story.</param>
        /// <returns>A string representing the unique identifier of the created story.</returns>
        Task<string> CreateStory(CreateStoryDto createStoryDto);

        /// <summary>
        /// Retrieves a story based on the provided request data.
        /// </summary>
        /// <param name="requestStoryDto">Data transfer object containing the criteria for retrieving the story.</param>
        /// <returns>A Story object representing the retrieved story.</returns>
        Task<Story> GetStory(RequestStoryDto requestStoryDto);

        /// <summary>
        /// Updates an existing story based on the provided data.
        /// </summary>
        /// <param name="updateStoryDto">Data transfer object containing the updated details for the story.</param>
        /// <returns>A string representing the unique identifier of the updated story.</returns>
        Task<string> UpdateStory(UpdateStoryDto updateStoryDto);

        /// <summary>
        /// Deletes a story based on the provided data.
        /// </summary>
        /// <param name="deleteStoryDto">Data transfer object containing the details for the story to be deleted.</param>
        /// <returns>A boolean indicating whether the story was successfully deleted.</returns>
        Task<bool> DeleteStory(DeleteStoryDto deleteStoryDto);

        /// <summary>
        /// Retrieves a list of stories based on the provided request data.
        /// </summary>
        /// <param name="listStoryRequestDto">Data transfer object containing the criteria for retrieving the list of stories.</param>
        /// <returns>A list of Story objects representing the retrieved stories.</returns>
        Task<List<Story>> GetListStory(ListStoryRequestDto listStoryRequestDto);
    }
}
