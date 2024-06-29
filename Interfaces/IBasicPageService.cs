
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
        /// <param name="createBasicPageDto">The data transfer object containing the information to create a basic page.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        Task<string> CreateBasicPage(CreateBasicPageDto createBasicPageDto);

        /// <summary>
        /// Retrieves a basic page by the given request data.
        /// </summary>
        /// <param name="basicPageRequestDto">The data transfer object containing the request information to retrieve a basic page.</param>
        /// <returns>The basic page object.</returns>
        Task<BasicPage> GetBasicPage(BasicPageRequestDto basicPageRequestDto);

        /// <summary>
        /// Updates an existing basic page.
        /// </summary>
        /// <param name="updateBasicPageDto">The data transfer object containing the information to update a basic page.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        Task<string> UpdateBasicPage(UpdateBasicPageDto updateBasicPageDto);

        /// <summary>
        /// Deletes a basic page by the given request data.
        /// </summary>
        /// <param name="deleteBasicPageDto">The data transfer object containing the information to delete a basic page.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        Task<string> DeleteBasicPage(DeleteBasicPageDto deleteBasicPageDto);

        /// <summary>
        /// Retrieves a list of basic pages by the given request data.
        /// </summary>
        /// <param name="listBasicPageRequestDto">The data transfer object containing the request information to retrieve a list of basic pages.</param>
        /// <returns>A list of basic page objects.</returns>
        Task<List<BasicPage>> GetListBasicPage(ListBasicPageRequestDto listBasicPageRequestDto);
    }
}
