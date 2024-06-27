
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for the Product Domain Service, providing methods to manage product domains.
    /// </summary>
    public interface IProductDomainService
    {
        /// <summary>
        /// Creates a new product domain based on the provided data.
        /// </summary>
        /// <param name="createProductDomainDto">Data transfer object containing the information needed to create a product domain.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateProductDomain(CreateProductDomainDto createProductDomainDto);

        /// <summary>
        /// Retrieves a product domain based on the provided request data.
        /// </summary>
        /// <param name="productDomainRequestDto">Data transfer object containing the request information to fetch a product domain.</param>
        /// <returns>A ProductDomain object representing the retrieved product domain.</returns>
        Task<ProductDomain> GetProductDomain(ProductDomainRequestDto productDomainRequestDto);

        /// <summary>
        /// Updates an existing product domain based on the provided data.
        /// </summary>
        /// <param name="updateProductDomainDto">Data transfer object containing the information needed to update a product domain.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateProductDomain(UpdateProductDomainDto updateProductDomainDto);

        /// <summary>
        /// Deletes a product domain based on the provided data.
        /// </summary>
        /// <param name="deleteProductDomainDto">Data transfer object containing the information needed to delete a product domain.</param>
        /// <returns>A boolean indicating the success or failure of the deletion operation.</returns>
        Task<bool> DeleteProductDomain(DeleteProductDomainDto deleteProductDomainDto);

        /// <summary>
        /// Retrieves a list of product domains based on the provided request data.
        /// </summary>
        /// <param name="listProductDomainRequestDto">Data transfer object containing the request information to fetch a list of product domains.</param>
        /// <returns>A list of ProductDomain objects representing the retrieved product domains.</returns>
        Task<List<ProductDomain>> GetListProductDomain(ListProductDomainRequestDto listProductDomainRequestDto);
    }
}
