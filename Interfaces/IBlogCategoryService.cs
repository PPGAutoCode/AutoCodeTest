
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing blog categories.
    /// </summary>
    public interface IBlogCategoryService
    {
        /// <summary>
        /// Creates a new blog category.
        /// </summary>
        /// <param name="createBlogCategoryDto">Data transfer object for creating a blog category.</param>
        /// <returns>The ID of the created blog category.</returns>
        Task<string> CreateBlogCategory(CreateBlogCategoryDto createBlogCategoryDto);

        /// <summary>
        /// Retrieves a blog category by the given request parameters.
        /// </summary>
        /// <param name="blogCategoryRequestDto">Data transfer object for requesting a blog category.</param>
        /// <returns>The requested blog category.</returns>
        Task<BlogCategory> GetBlogCategory(BlogCategoryRequestDto blogCategoryRequestDto);

        /// <summary>
        /// Updates an existing blog category.
        /// </summary>
        /// <param name="updateBlogCategoryDto">Data transfer object for updating a blog category.</param>
        /// <returns>A message indicating the result of the update operation.</returns>
        Task<string> UpdateBlogCategory(UpdateBlogCategoryDto updateBlogCategoryDto);

        /// <summary>
        /// Deletes a blog category.
        /// </summary>
        /// <param name="deleteBlogCategoryDto">Data transfer object for deleting a blog category.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteBlogCategory(DeleteBlogCategoryDto deleteBlogCategoryDto);

        /// <summary>
        /// Retrieves a list of blog categories based on the given request parameters.
        /// </summary>
        /// <param name="listBlogCategoryRequestDto">Data transfer object for requesting a list of blog categories.</param>
        /// <returns>A list of blog categories.</returns>
        Task<List<BlogCategory>> GetListBlogCategory(ListBlogCategoryRequestDto listBlogCategoryRequestDto);
    }
}
