
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Service interface for managing organization-related operations.
    /// </summary>
    public interface IOrganizationService
    {
        /// <summary>
        /// Creates a new organization based on the provided data.
        /// </summary>
        /// <param name="createOrganizationDto">Data transfer object containing the details for the new organization.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateOrganization(CreateOrganizationDto createOrganizationDto);

        /// <summary>
        /// Retrieves an organization based on the provided data.
        /// </summary>
        /// <param name="organizationDto">Data transfer object containing the details to identify the organization.</param>
        /// <returns>An Organization object representing the retrieved organization.</returns>
        Task<Organization> GetOrganization(OrganizationDto organizationDto);

        /// <summary>
        /// Updates an existing organization based on the provided data.
        /// </summary>
        /// <param name="updateOrganizationDto">Data transfer object containing the details for the update.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateOrganization(UpdateOrganizationDto updateOrganizationDto);

        /// <summary>
        /// Deletes an organization based on the provided data.
        /// </summary>
        /// <param name="deleteOrganizationDto">Data transfer object containing the details to identify the organization to delete.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteOrganization(DeleteOrganizationDto deleteOrganizationDto);

        /// <summary>
        /// Retrieves a list of organizations based on the provided request data.
        /// </summary>
        /// <param name="listOrganizationRequesteDto">Data transfer object containing the request details.</param>
        /// <returns>A list of Organization objects representing the retrieved organizations.</returns>
        Task<List<Organization>> GetListOrganization(ListOrganizationRequesteDto listOrganizationRequesteDto);
    }
}
