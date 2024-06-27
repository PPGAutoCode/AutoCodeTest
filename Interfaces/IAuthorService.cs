
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing author-related operations.
    /// </summary>
    public interface IAuthorService
    {
        /// <summary>
        /// Creates a new author.
        /// </summary>
        /// <param name="createAuthorDto">The data transfer object containing the details of the author to be created.</param>
        /// <returns>A string representing the unique identifier of the newly created author.</returns>
        Task<string> CreateAuthor(CreateAuthorDto createAuthorDto);

        /// <summary>
        /// Retrieves an author based on the provided request details.
        /// </summary>
        /// <param name="authorRequestDto">The data transfer object containing the request details to fetch the author.</param>
        /// <returns>An Author object representing the fetched author.</returns>
        Task<Author> GetAuthor(AuthorRequestDto authorRequestDto);

        /// <summary>
        /// Updates an existing author.
        /// </summary>
        /// <param name="updateAuthorDto">The data transfer object containing the details of the author to be updated.</param>
        /// <returns>A string representing the unique identifier of the updated author.</returns>
        Task<string> UpdateAuthor(UpdateAuthorDto updateAuthorDto);

        /// <summary>
        /// Deletes an author based on the provided details.
        /// </summary>
        /// <param name="deleteAuthorDto">The data transfer object containing the details of the author to be deleted.</param>
        /// <returns>A boolean indicating whether the author was successfully deleted.</returns>
        Task<bool> DeleteAuthor(DeleteAuthorDto deleteAuthorDto);

        /// <summary>
        /// Retrieves a list of authors based on the provided request details.
        /// </summary>
        /// <param name="listAuthorRequestDto">The data transfer object containing the request details to fetch the list of authors.</param>
        /// <returns>A list of Author objects representing the fetched authors.</returns>
        Task<List<Author>> GetListAuthor(ListAuthorRequestDto listAuthorRequestDto);
    }
}
