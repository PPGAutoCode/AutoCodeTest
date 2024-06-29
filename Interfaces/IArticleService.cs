
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
        /// <param name="createArticleDto">Data transfer object containing the information needed to create an article.</param>
        /// <returns>A string representing the identifier of the newly created article.</returns>
        Task<string> CreateArticle(CreateArticleDto createArticleDto);

        /// <summary>
        /// Retrieves an article based on the provided request data.
        /// </summary>
        /// <param name="articleRequestDto">Data transfer object containing the information needed to request an article.</param>
        /// <returns>An Article object representing the requested article.</returns>
        Task<Article> GetArticle(ArticleRequestDto articleRequestDto);

        /// <summary>
        /// Updates an existing article based on the provided data.
        /// </summary>
        /// <param name="updateArticleDto">Data transfer object containing the information needed to update an article.</param>
        /// <returns>A string representing the identifier of the updated article.</returns>
        Task<string> UpdateArticle(UpdateArticleDto updateArticleDto);

        /// <summary>
        /// Deletes an article based on the provided data.
        /// </summary>
        /// <param name="deleteArticleDto">Data transfer object containing the information needed to delete an article.</param>
        /// <returns>A boolean indicating whether the article was successfully deleted.</returns>
        Task<bool> DeleteArticle(DeleteArticleDto deleteArticleDto);
    }
}
