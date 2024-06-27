
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing API endpoints.
    /// </summary>
    public interface IAPIEndpointService
    {
        /// <summary>
        /// Creates a new API endpoint.
        /// </summary>
        /// <param name="createAPIEndpointDTO">Data transfer object for creating an API endpoint.</param>
        /// <returns>The identifier of the created API endpoint.</returns>
        Task<string> CreateAPIEndpoint(CreateAPIEndpointDTO createAPIEndpointDTO);

        /// <summary>
        /// Retrieves an API endpoint by its request DTO.
        /// </summary>
        /// <param name="apiEndpointRequestDTO">Data transfer object for requesting an API endpoint.</param>
        /// <returns>The requested API endpoint.</returns>
        Task<APIEndpoint> GetAPIEndpoint(APIEndpointRequestDTO apiEndpointRequestDTO);

        /// <summary>
        /// Updates an existing API endpoint.
        /// </summary>
        /// <param name="updateAPIEndpointDTO">Data transfer object for updating an API endpoint.</param>
        /// <returns>The identifier of the updated API endpoint.</returns>
        Task<string> UpdateAPIEndpoint(UpdateAPIEndpointDTO updateAPIEndpointDTO);

        /// <summary>
        /// Deletes an API endpoint.
        /// </summary>
        /// <param name="deleteAPIEndpointDTO">Data transfer object for deleting an API endpoint.</param>
        /// <returns>True if the API endpoint was deleted successfully, otherwise false.</returns>
        Task<bool> DeleteAPIEndpoint(DeleteAPIEndpointDTO deleteAPIEndpointDTO);

        /// <summary>
        /// Retrieves a list of API endpoints based on the request DTO.
        /// </summary>
        /// <param name="listAPIEndpointRequestDTO">Data transfer object for requesting a list of API endpoints.</param>
        /// <returns>A list of API endpoints.</returns>
        Task<List<APIEndpoint>> GetListAPIEndpoint(ListAPIEndpointRequestDTO listAPIEndpointRequestDTO);
    }
}
