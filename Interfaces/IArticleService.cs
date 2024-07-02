
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing article-related operations.
    /// </summary>
    public interface IArticleService
    {
        /// <summary>
        /// Creates a new article based on the provided data.
        /// </summary>
        /// <param name="createArticleDto">The data transfer object containing the article information to be created.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateArticle(CreateArticleDto createArticleDto);

        /// <summary>
        /// Retrieves an article based on the provided request data.
        /// </summary>
        /// <param name="articleRequestDto">The data transfer object containing the request information to retrieve the article.</param>
        /// <returns>An Article object representing the retrieved article.</returns>
        Task<Article> GetArticle(ArticleRequestDto articleRequestDto);

        /// <summary>
        /// Updates an existing article based on the provided data.
        /// </summary>
        /// <param name="updateArticleDto">The data transfer object containing the updated article information.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateArticle(UpdateArticleDto updateArticleDto);

        /// <summary>
        /// Deletes an article based on the provided data.
        /// </summary>
        /// <param name="deleteArticleDto">The data transfer object containing the information to delete the article.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteArticle(DeleteArticleDto deleteArticleDto);
    }
}
