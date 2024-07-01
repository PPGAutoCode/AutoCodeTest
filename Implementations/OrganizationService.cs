
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
            var organizationId = Guid.NewGuid();
            var fileId = Guid.NewGuid();

            // Step 5: Single SQL Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO File (Id, FileName, FileUrl, Timestamp) VALUES (@Id, @FileName, @FileUrl, @Timestamp)",
                        new { Id = fileId, FileName = "FileName", FileUrl = new byte[0], Timestamp = DateTime.UtcNow }, transaction);

                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO Organization (Id, Title, ImageId, Description, Status, FileId) VALUES (@Id, @Title, @ImageId, @Description, @Status, @FileId)",
                        new { Id = organizationId, request.Title, request.ImageId, request.Description, request.Status, FileId = fileId }, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 6: Return Value
            return organizationId.ToString();
        }

        public async Task<List<Organization>> GetListOrganization(ListOrganizationRequestDto request)
        {
            // Step 1: Request Validation
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Sorting and Pagination
            var sortField = string.IsNullOrEmpty(request.SortField) ? "Title" : request.SortField;
            var sortOrder = string.IsNullOrEmpty(request.SortOrder) ? "ASC" : request.SortOrder;

            // Step 3: Fetching Organization Data
            var query = $"SELECT * FROM Organization ORDER BY {sortField} {sortOrder} OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            var organizations = await _dbConnection.QueryAsync<Organization>(query, new { Offset = request.PageOffset, Limit = request.PageLimit });

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

            if (organization == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return organization;
        }

        public async Task<string> UpdateOrganization(UpdateOrganizationDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == Guid.Empty || string.IsNullOrEmpty(request.Title))
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
            existingOrganization.ImageId = request.ImageId;
            existingOrganization.Description = request.Description;
            existingOrganization.Status = request.Status;
            existingOrganization.FileId = request.File;

            // Step 4: Save Changes to Database
            await _dbConnection.ExecuteAsync(
                "UPDATE Organization SET Title = @Title, ImageId = @ImageId, Description = @Description, Status = @Status, FileId = @FileId WHERE Id = @Id",
                new { existingOrganization.Title, existingOrganization.ImageId, existingOrganization.Description, existingOrganization.Status, existingOrganization.FileId, existingOrganization.Id });

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

            return true;
        }
    }
}
