
using System.Collections.Generic;
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
        /// Creates a new article.
        /// </summary>
        /// <param name="createArticleDto">The data transfer object containing the details of the article to be created.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateArticle(CreateArticleDto createArticleDto);

        /// <summary>
        /// Retrieves an article based on the provided request details.
        /// </summary>
        /// <param name="articleRequestDto">The data transfer object containing the request details for the article.</param>
        /// <returns>An Article object representing the retrieved article.</returns>
        Task<Article> GetArticle(ArticleRequestDto articleRequestDto);

        /// <summary>
        /// Updates an existing article.
        /// </summary>
        /// <param name="updateArticleDto">The data transfer object containing the details of the article to be updated.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateArticle(UpdateArticleDto updateArticleDto);

        /// <summary>
        /// Deletes an article based on the provided details.
        /// </summary>
        /// <param name="deleteArticleDto">The data transfer object containing the details of the article to be deleted.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteArticle(DeleteArticleDto deleteArticleDto);

        /// <summary>
        /// Retrieves a list of articles based on the provided request details.
        /// </summary>
        /// <param name="listArticleRequestDto">The data transfer object containing the request details for the list of articles.</param>
        /// <returns>A list of Article objects representing the retrieved articles.</returns>
        Task<List<Article>> GetListArticle(ListArticleRequestDto listArticleRequestDto);
    }
}
