
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing organization-related operations.
    /// </summary>
    public interface IOrganizationService
    {
        /// <summary>
        /// Creates a new organization based on the provided data.
        /// </summary>
        /// <param name="createOrganizationDTO">Data transfer object containing the details for the new organization.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateOrganization(CreateOrganizationDTO createOrganizationDTO);

        /// <summary>
        /// Retrieves an organization based on the provided data.
        /// </summary>
        /// <param name="organizationDTO">Data transfer object containing the details to identify the organization.</param>
        /// <returns>An Organization object representing the found organization.</returns>
        Task<Organization> GetOrganization(OrganizationDTO organizationDTO);

        /// <summary>
        /// Updates an existing organization based on the provided data.
        /// </summary>
        /// <param name="updateOrganizationDTO">Data transfer object containing the details for the update.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateOrganization(UpdateOrganizationDTO updateOrganizationDTO);

        /// <summary>
        /// Deletes an organization based on the provided data.
        /// </summary>
        /// <param name="deleteOrganizationDTO">Data transfer object containing the details to identify the organization to delete.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteOrganization(DeleteOrganizationDTO deleteOrganizationDTO);

        /// <summary>
        /// Retrieves a list of organizations based on the provided request data.
        /// </summary>
        /// <param name="getListOrganizationRequesteDTO">Data transfer object containing the request details.</param>
        /// <returns>A list of Organization objects representing the found organizations.</returns>
        Task<List<Organization>> GetListOrganization(GetListOrganizationRequesteDTO getListOrganizationRequesteDTO);
    }
}
