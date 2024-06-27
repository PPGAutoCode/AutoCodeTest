
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.ControllersExceptions;
using ProjectName.Interfaces;
using ProjectName.Types;

namespace ProjectName.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IDbConnection _dbConnection;

        public OrganizationService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateOrganization(CreateOrganizationDTO createOrganizationDTO)
        {
            // Step 1: Request Validation
            if (string.IsNullOrEmpty(createOrganizationDTO.Title) || createOrganizationDTO.Document == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Document Handling
            if (string.IsNullOrEmpty(createOrganizationDTO.Document.Id) || string.IsNullOrEmpty(createOrganizationDTO.Document.Label) ||
                string.IsNullOrEmpty(createOrganizationDTO.Document.AllowedFileExtensions))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 3: Creating the Organization
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Title = createOrganizationDTO.Title,
                Image = createOrganizationDTO.Image,
                Description = createOrganizationDTO.Description,
                Status = createOrganizationDTO.Status,
                Document = createOrganizationDTO.Document,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            // Step 4: In a single SQL transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    const string insertOrganizationQuery = @"
                        INSERT INTO Organization (Id, Title, Image, Description, Status, Document, Created, Changed)
                        VALUES (@Id, @Title, @Image, @Description, @Status, @Document, @Created, @Changed)";
                    await _dbConnection.ExecuteAsync(insertOrganizationQuery, organization, transaction);

                    const string insertDocumentQuery = @"
                        INSERT INTO Document (Id, Label, Helptext, RequiredField, AllowedFileExtensions, FileDirectory, MaxUploadSize, EnableDescriptionField)
                        VALUES (@Id, @Label, @Helptext, @RequiredField, @AllowedFileExtensions, @FileDirectory, @MaxUploadSize, @EnableDescriptionField)";
                    await _dbConnection.ExecuteAsync(insertDocumentQuery, organization.Document, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 5: Return Organization.Id from database
            return organization.Id.ToString();
        }

        public async Task<List<Organization>> GetListOrganization(GetListOrganizationRequestDTO requestDTO)
        {
            // Step 1: Request Validation
            if (requestDTO.PageLimit <= 0 || requestDTO.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Sorting and Pagination
            var sortField = string.IsNullOrEmpty(requestDTO.SortField) ? "Title" : requestDTO.SortField;
            var sortOrder = string.IsNullOrEmpty(requestDTO.SortOrder) ? "ASC" : requestDTO.SortOrder;

            // Step 3: Fetching Organization Data
            var query = $@"
                SELECT * FROM Organization
                ORDER BY {sortField} {sortOrder}
                OFFSET {requestDTO.PageOffset} ROWS
                FETCH NEXT {requestDTO.PageLimit} ROWS ONLY";

            var organizations = await _dbConnection.QueryAsync<Organization>(query);

            // Step 4: Return the list of Organizations
            return organizations.ToList();
        }

        public async Task<Organization> GetOrganization(OrganizationDTO organizationDTO)
        {
            // Step 1: Validate OrganizationDTO fields
            if (organizationDTO.Id == Guid.Empty && string.IsNullOrEmpty(organizationDTO.Title))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch organization from database by the requested field
            var query = @"
                SELECT * FROM Organization
                WHERE Id = @Id OR Title = @Title";

            var organization = await _dbConnection.QueryFirstOrDefaultAsync<Organization>(query, new { Id = organizationDTO.Id, Title = organizationDTO.Title });

            // Step 3: Return the Organization or throw exception if not found
            if (organization == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return organization;
        }

        public async Task<string> UpdateOrganization(UpdateOrganizationDTO updateOrganizationDTO)
        {
            // Step 1: Validate Necessary Parameters
            if (updateOrganizationDTO.Id == Guid.Empty || string.IsNullOrEmpty(updateOrganizationDTO.Title))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing Organization
            var existingOrganization = await GetOrganization(new OrganizationDTO { Id = updateOrganizationDTO.Id });
            if (existingOrganization == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update Organization Object
            existingOrganization.Title = updateOrganizationDTO.Title;
            existingOrganization.Image = updateOrganizationDTO.Image;
            existingOrganization.Description = updateOrganizationDTO.Description;
            existingOrganization.Status = updateOrganizationDTO.Status;
            existingOrganization.Document = updateOrganizationDTO.Document;
            existingOrganization.Changed = DateTime.UtcNow;

            // Step 4: Save Changes to Database
            const string updateQuery = @"
                UPDATE Organization
                SET Title = @Title, Image = @Image, Description = @Description, Status = @Status, Document = @Document, Changed = @Changed
                WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(updateQuery, existingOrganization);

            // Step 5: Return the updated Organization ID
            return existingOrganization.Id.ToString();
        }

        public async Task<bool> DeleteOrganization(DeleteOrganizationDTO deleteOrganizationDTO)
        {
            // Step 1: Validate that the request.payload.id contains the necessary parameter (Id)
            if (deleteOrganizationDTO.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Get the existing Organization object from the database based on the provided userID
            var existingOrganization = await GetOrganization(new OrganizationDTO { Id = deleteOrganizationDTO.Id });
            if (existingOrganization == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete the Organization object from the database
            const string deleteQuery = @"
                DELETE FROM Organization
                WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(deleteQuery, new { Id = deleteOrganizationDTO.Id });

            // Step 4: Return true if the transaction is successful
            return true;
        }
    }
}
