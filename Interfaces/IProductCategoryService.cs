
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing product categories.
    /// </summary>
    public interface IProductCategoryService
    {
        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="createProductCategoryDto">The data transfer object containing the details of the product category to be created.</param>
        /// <returns>A string representing the unique identifier of the newly created product category.</returns>
        Task<string> CreateProductCategory(CreateProductCategoryDto createProductCategoryDto);

        /// <summary>
        /// Retrieves a product category by its unique identifier.
        /// </summary>
        /// <param name="productCategoryRequestDto">The data transfer object containing the details required to retrieve the product category.</param>
        /// <returns>A ProductCategory object representing the retrieved product category.</returns>
        Task<ProductCategory> GetProductCategory(ProductCategoryRequestDto productCategoryRequestDto);

        /// <summary>
        /// Updates an existing product category.
        /// </summary>
        /// <param name="updateProductCategoryDto">The data transfer object containing the updated details of the product category.</param>
        /// <returns>A string representing the unique identifier of the updated product category.</returns>
        Task<string> UpdateProductCategory(UpdateProductCategoryDto updateProductCategoryDto);

        /// <summary>
        /// Deletes a product category by its unique identifier.
        /// </summary>
        /// <param name="deleteProductCategoryDto">The data transfer object containing the details required to delete the product category.</param>
        /// <returns>A boolean indicating whether the product category was successfully deleted.</returns>
        Task<bool> DeleteProductCategory(DeleteProductCategoryDto deleteProductCategoryDto);

        /// <summary>
        /// Retrieves a list of product categories based on the provided criteria.
        /// </summary>
        /// <param name="listProductCategoryRequestDto">The data transfer object containing the criteria for retrieving the list of product categories.</param>
        /// <returns>A list of ProductCategory objects representing the retrieved product categories.</returns>
        Task<List<ProductCategory>> GetListProductCategory(ListProductCategoryRequestDto listProductCategoryRequestDto);
    }
}
