
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

        public async Task<string> CreateOrganization(CreateOrganizationDto request)
        {
            // Step 1: Request Validation
            if (string.IsNullOrEmpty(request.Title) || request.Status == null || request.File == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Verify Title Uniqueness
            var existingOrganization = await _dbConnection.QueryFirstOrDefaultAsync<Organization>(
                "SELECT * FROM Organization WHERE Title = @Title", new { request.Title });
            if (existingOrganization != null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 3: File Handling
            // Assuming File validation logic here

            // Step 4: Creating the Organization
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                ImageId = request.Image,
                Description = request.Description,
                Status = request.Status,
                FileId = request.File,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow
            };

            // Step 5: Single SQL Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO Organization (Id, Title, ImageId, Description, Status, FileId, Created, Changed) " +
                        "VALUES (@Id, @Title, @ImageId, @Description, @Status, @FileId, @Created, @Changed)",
                        organization, transaction);

                    // Assuming File insertion logic here

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 6: Return Value
            return organization.Id.ToString();
        }

        public async Task<List<Organization>> GetListOrganization(ListOrganizationRequestDto request)
        {
            // Step 1: Request Validation
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Sorting and Pagination
            var query = "SELECT * FROM Organization";
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                query += $" ORDER BY {request.SortField} {request.SortOrder}";
            }
            query += $" OFFSET {request.PageOffset} ROWS FETCH NEXT {request.PageLimit} ROWS ONLY";

            // Step 3: Fetching Organization Data
            var organizations = await _dbConnection.QueryAsync<Organization>(query);

            // Step 4: Return Value
            return organizations.ToList();
        }

        public async Task<Organization> GetOrganization(OrganizationRequestDto request)
        {
            // Step 1: Validation
            if (request.Id == null && string.IsNullOrEmpty(request.Title))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Organization
            var organization = await _dbConnection.QueryFirstOrDefaultAsync<Organization>(
                "SELECT * FROM Organization WHERE Id = @Id OR Title = @Title", new { request.Id, request.Title });

            // Step 3: Return Value
            if (organization == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return organization;
        }

        public async Task<string> UpdateOrganization(UpdateOrganizationDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Title) || request.Status == null || request.File == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing Organization
            var existingOrganization = await _dbConnection.QueryFirstOrDefaultAsync<Organization>(
                "SELECT * FROM Organization WHERE Id = @Id", new { request.Id });
            if (existingOrganization == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update Organization Object
            existingOrganization.Title = request.Title;
            existingOrganization.Status = request.Status;
            existingOrganization.ImageId = request.Image;
            existingOrganization.Description = request.Description;
            existingOrganization.FileId = request.File;
            existingOrganization.Changed = DateTime.UtcNow;

            // Step 4: Save Changes to Database
            await _dbConnection.ExecuteAsync(
                "UPDATE Organization SET Title = @Title, Status = @Status, ImageId = @ImageId, Description = @Description, FileId = @FileId, Changed = @Changed WHERE Id = @Id",
                existingOrganization);

            // Step 5: Return Value
            return existingOrganization.Id.ToString();
        }

        public async Task<bool> DeleteOrganization(DeleteOrganizationDto request)
        {
            // Step 1: Validate Request
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing Organization
            var existingOrganization = await _dbConnection.QueryFirstOrDefaultAsync<Organization>(
                "SELECT * FROM Organization WHERE Id = @Id", new { request.Id });
            if (existingOrganization == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete Organization
            await _dbConnection.ExecuteAsync("DELETE FROM Organization WHERE Id = @Id", new { request.Id });

            // Step 4: Return Value
            return true;
        }
    }
}
