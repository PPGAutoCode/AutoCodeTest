
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
        /// Creates a new organization.
        /// </summary>
        /// <param name="createOrganizationDto">Data transfer object containing the details of the organization to be created.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateOrganization(CreateOrganizationDto createOrganizationDto);

        /// <summary>
        /// Retrieves an organization based on the provided request details.
        /// </summary>
        /// <param name="organizationRequestDto">Data transfer object containing the request details to fetch an organization.</param>
        /// <returns>An Organization object representing the fetched organization.</returns>
        Task<Organization> GetOrganization(OrganizationRequestDto organizationRequestDto);

        /// <summary>
        /// Updates an existing organization.
        /// </summary>
        /// <param name="updateOrganizationDto">Data transfer object containing the details of the organization to be updated.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateOrganization(UpdateOrganizationDto updateOrganizationDto);

        /// <summary>
        /// Deletes an organization based on the provided details.
        /// </summary>
        /// <param name="deleteOrganizationDto">Data transfer object containing the details of the organization to be deleted.</param>
        /// <returns>A boolean indicating the success or failure of the deletion operation.</returns>
        Task<bool> DeleteOrganization(DeleteOrganizationDto deleteOrganizationDto);

        /// <summary>
        /// Retrieves a list of organizations based on the provided request details.
        /// </summary>
        /// <param name="listOrganizationRequestDto">Data transfer object containing the request details to fetch a list of organizations.</param>
        /// <returns>A list of Organization objects representing the fetched organizations.</returns>
        Task<List<Organization>> GetListOrganization(ListOrganizationRequestDto listOrganizationRequestDto);
    }
}
