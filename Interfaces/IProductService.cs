
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
        /// Asynchronously creates a new product.
        /// </summary>
        /// <param name="createProductDto">The data transfer object containing the details of the product to be created.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the created product.</returns>
        Task<string> CreateProduct(CreateProductDto createProductDto);
    }
}
