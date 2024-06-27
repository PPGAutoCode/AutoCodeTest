
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing basic page operations.
    /// </summary>
    public interface IBasicPageService
    {
        /// <summary>
        /// Creates a new basic page.
        /// </summary>
        /// <param name="createBasicPageDto">Data transfer object for creating a basic page.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateBasicPage(CreateBasicPageDto createBasicPageDto);

        /// <summary>
        /// Retrieves a basic page based on the provided request data.
        /// </summary>
        /// <param name="basicPageRequestDto">Data transfer object for requesting a basic page.</param>
        /// <returns>A BasicPage object representing the retrieved page.</returns>
        Task<BasicPage> GetBasicPage(BasicPageRequestDto basicPageRequestDto);

        /// <summary>
        /// Updates an existing basic page.
        /// </summary>
        /// <param name="updateBasicPageDto">Data transfer object for updating a basic page.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateBasicPage(UpdateBasicPageDto updateBasicPageDto);

        /// <summary>
        /// Deletes a basic page based on the provided request data.
        /// </summary>
        /// <param name="deleteBasicPageDto">Data transfer object for deleting a basic page.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteBasicPage(DeleteBasicPageDto deleteBasicPageDto);

        /// <summary>
        /// Retrieves a list of basic pages based on the provided request data.
        /// </summary>
        /// <param name="listBasicPageRequestDto">Data transfer object for requesting a list of basic pages.</param>
        /// <returns>A list of BasicPage objects representing the retrieved pages.</returns>
        Task<List<BasicPage>> GetListBasicPage(ListBasicPageRequestDto listBasicPageRequestDto);
    }
}
