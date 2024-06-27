
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing product tags.
    /// </summary>
    public interface IProductTagService
    {
        /// <summary>
        /// Creates a new product tag.
        /// </summary>
        /// <param name="createProductTagDto">The data transfer object containing the information for the new product tag.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateProductTag(CreateProductTagDto createProductTagDto);

        /// <summary>
        /// Retrieves a product tag based on the provided request data.
        /// </summary>
        /// <param name="productTagRequestDto">The data transfer object containing the request information for the product tag.</param>
        /// <returns>A ProductTag object representing the retrieved product tag.</returns>
        Task<ProductTag> GetProductTag(ProductTagRequestDto productTagRequestDto);

        /// <summary>
        /// Updates an existing product tag.
        /// </summary>
        /// <param name="updateProductTagDto">The data transfer object containing the updated information for the product tag.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateProductTag(UpdateProductTagDto updateProductTagDto);

        /// <summary>
        /// Deletes a product tag based on the provided request data.
        /// </summary>
        /// <param name="deleteProductTagDto">The data transfer object containing the information for the product tag to be deleted.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteProductTag(DeleteProductTagDto deleteProductTagDto);

        /// <summary>
        /// Retrieves a list of product tags based on the provided request data.
        /// </summary>
        /// <param name="listProductTagRequestDto">The data transfer object containing the request information for the list of product tags.</param>
        /// <returns>A list of ProductTag objects representing the retrieved product tags.</returns>
        Task<List<ProductTag>> GetListProductTag(ListProductTagRequestDto listProductTagRequestDto);
    }
}
