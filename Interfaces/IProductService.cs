
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing product-related operations.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Creates a new product based on the provided data.
        /// </summary>
        /// <param name="createProductDto">Data transfer object containing the details of the product to be created.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateProduct(CreateProductDto createProductDto);

        /// <summary>
        /// Retrieves a product based on the provided request data.
        /// </summary>
        /// <param name="productRequestDto">Data transfer object containing the request details to fetch a product.</param>
        /// <returns>A Product object matching the request.</returns>
        Task<Product> GetProduct(ProductRequestDto productRequestDto);

        /// <summary>
        /// Updates an existing product based on the provided data.
        /// </summary>
        /// <param name="updateProductDto">Data transfer object containing the details of the product to be updated.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateProduct(UpdateProductDto updateProductDto);

        /// <summary>
        /// Deletes a product based on the provided data.
        /// </summary>
        /// <param name="deleteProductDto">Data transfer object containing the details of the product to be deleted.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteProduct(DeleteProductDto deleteProductDto);

        /// <summary>
        /// Retrieves a list of products based on the provided request data.
        /// </summary>
        /// <param name="listProductRequestDto">Data transfer object containing the request details to fetch a list of products.</param>
        /// <returns>A list of Product objects matching the request.</returns>
        Task<List<Product>> GetListProduct(ListProductRequestDto listProductRequestDto);
    }
}
