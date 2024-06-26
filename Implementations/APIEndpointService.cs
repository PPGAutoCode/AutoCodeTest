
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
    public class APIEndpointService : IAPIEndpointService
    {
        private readonly IDbConnection _dbConnection;

        public APIEndpointService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAPIEndpoint(CreateAPIEndpointDto request)
        {
            // Step 1: Validate Fields
            if (string.IsNullOrEmpty(request.Name) || request.Deprecated == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Tags
            if (request.ApiTags != null && request.ApiTags.Any())
            {
                var existingTags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = request.ApiTags });
                if (existingTags.Count() != request.ApiTags.Count)
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }
            }

            // Step 3: Create APIEndpoint Object
            var apiEndpoint = new APIEndpoint
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ApiContext = request.ApiContext,
                ApiReferenceId = request.ApiReferenceId,
                ApiResource = request.ApiResource,
                ApiScope = request.ApiScope,
                ApiScopeProduction = request.ApiScopeProduction,
                ApiSecurity = request.ApiSecurity,
                Deprecated = request.Deprecated,
                Description = request.Description,
                Documentation = request.Documentation,
                EndpointUrls = request.EndpointUrls,
                EnvironmentId = request.EnvironmentId,
                ProviderId = request.ProviderId,
                Swagger = request.Swagger,
                Tour = request.Tour,
                Updated = DateTime.UtcNow,
                Version = request.Version
            };

            // Step 4: Create APIEndpointTags List
            var apiEndpointTags = new List<APIEndpointTag>();
            if (request.ApiTags != null)
            {
                foreach (var tagId in request.ApiTags)
                {
                    apiEndpointTags.Add(new APIEndpointTag { Id = Guid.NewGuid(), APIEndpointId = apiEndpoint.Id, APITagId = tagId });
                }
            }

            // Step 5: Database Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert the apiEndpoint object into the APIEndpoints database table
                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpoints (Id, Name, ApiContext, ApiReferenceId, ApiResource, ApiScope, ApiScopeProduction, ApiSecurity, Deprecated, Description, Documentation, EndpointUrls, EnvironmentId, ProviderId, Swagger, Tour, Updated, Version) VALUES (@Id, @Name, @ApiContext, @ApiReferenceId, @ApiResource, @ApiScope, @ApiScopeProduction, @ApiSecurity, @Deprecated, @Description, @Documentation, @EndpointUrls, @EnvironmentId, @ProviderId, @Swagger, @Tour, @Updated, @Version)", apiEndpoint, transaction);

                    // Insert the related APIEndpointTags into the respective database table
                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (Id, APIEndpointId, APITagId) VALUES (@Id, @APIEndpointId, @APITagId)", apiEndpointTags, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 6: Return APIEndpointId
            return apiEndpoint.Id.ToString();
        }

        public async Task<APIEndpoint> GetAPIEndpoint(APIEndpointRequestDto request)
        {
            // Step 1: Validate Input
            if (request.Id == null && string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            APIEndpoint apiEndpoint;

            // Step 2: Fetch API Endpoint
            if (request.Id != null)
            {
                apiEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            }
            else
            {
                apiEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Name = @Name", new { Name = request.Name });
            }

            // Step 3: Fetch Associated Tags
            if (apiEndpoint != null)
            {
                var tagIds = await _dbConnection.QueryAsync<Guid>("SELECT APITagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = apiEndpoint.Id });
                var tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = tagIds });

                if (tags.Count() != tagIds.Count())
                {
                    throw new BusinessException("DP-404", "Technical Error");
                }

                // Step 4: Map and Return
                apiEndpoint.ApiTags = tags.ToList();
            }
            else
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            return apiEndpoint;
        }

        public async Task<string> UpdateAPIEndpoint(UpdateAPIEndpointDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == null || string.IsNullOrEmpty(request.Name) || request.Deprecated == null || string.IsNullOrEmpty(request.Version))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing API Endpoint
            var existingEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            if (existingEndpoint == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Validate Related Entities
            if (request.ApiTags != null && request.ApiTags.Any())
            {
                var existingTags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = request.ApiTags });
                if (existingTags.Count() != request.ApiTags.Count)
                {
                    throw new BusinessException("DP-422", "Client Error");
                }
            }

            // Step 4: Update the APIEndpoint object with the provided changes
            existingEndpoint.Name = request.Name;
            existingEndpoint.ApiContext = request.ApiContext;
            existingEndpoint.ApiReferenceId = request.ApiReferenceId;
            existingEndpoint.ApiResource = request.ApiResource;
            existingEndpoint.ApiScope = request.ApiScope;
            existingEndpoint.ApiScopeProduction = request.ApiScopeProduction;
            existingEndpoint.ApiSecurity = request.ApiSecurity;
            existingEndpoint.Deprecated = request.Deprecated;
            existingEndpoint.Description = request.Description;
            existingEndpoint.Documentation = request.Documentation;
            existingEndpoint.EndpointUrls = request.EndpointUrls;
            existingEndpoint.EnvironmentId = request.EnvironmentId;
            existingEndpoint.ProviderId = request.ProviderId;
            existingEndpoint.Swagger = request.Swagger;
            existingEndpoint.Tour = request.Tour;
            existingEndpoint.Updated = DateTime.UtcNow;
            existingEndpoint.Version = request.Version;

            // Step 5: Update APIEndpointTags
            var existingTags = await _dbConnection.QueryAsync<APIEndpointTag>("SELECT * FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingEndpoint.Id });
            var tagsToRemove = existingTags.Where(et => !request.ApiTags.Contains(et.APITagId)).ToList();
            var tagsToAdd = request.ApiTags.Where(rt => !existingTags.Select(et => et.APITagId).Contains(rt)).ToList();

            // Step 6: Save Changes to Database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Update APIEndpoints Table
                    await _dbConnection.ExecuteAsync("UPDATE APIEndpoints SET Name = @Name, ApiContext = @ApiContext, ApiReferenceId = @ApiReferenceId, ApiResource = @ApiResource, ApiScope = @ApiScope, ApiScopeProduction = @ApiScopeProduction, ApiSecurity = @ApiSecurity, Deprecated = @Deprecated, Description = @Description, Documentation = @Documentation, EndpointUrls = @EndpointUrls, EnvironmentId = @EnvironmentId, ProviderId = @ProviderId, Swagger = @Swagger, Tour = @Tour, Updated = @Updated, Version = @Version WHERE Id = @Id", existingEndpoint, transaction);

                    // Remove the old tags from the database
                    if (tagsToRemove.Any())
                    {
                        await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE Id IN @Ids", new { Ids = tagsToRemove.Select(t => t.Id) }, transaction);
                    }

                    // Insert the new tags into the database
                    var newTags = tagsToAdd.Select(tagId => new APIEndpointTag { Id = Guid.NewGuid(), APIEndpointId = existingEndpoint.Id, APITagId = tagId }).ToList();
                    if (newTags.Any())
                    {
                        await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (Id, APIEndpointId, APITagId) VALUES (@Id, @APIEndpointId, @APITagId)", newTags, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 7: Return the updated API endpoint ID
            return existingEndpoint.Id.ToString();
        }

        public async Task<bool> DeleteAPIEndpoint(DeleteAPIEndpointDto request)
        {
            // Step 1: Validate Input
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing API Endpoint
            var existingEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            if (existingEndpoint == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Fetch Related Tags
            var tagIds = await _dbConnection.QueryAsync<Guid>("SELECT APITagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingEndpoint.Id });
            var tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = tagIds });

            if (tags.Count() != tagIds.Count())
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 4: Delete the APIEndpoint
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Remove the APIEndpoint object from the database
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpoints WHERE Id = @Id", new { Id = existingEndpoint.Id }, transaction);

                    // Ensure that all related entries in the APIEndpointTags table are also appropriately handled (deleted)
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingEndpoint.Id }, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 5: Return the Response
            return true;
        }

        public async Task<List<APIEndpoint>> GetListAPIEndpoint(ListAPIEndpointRequestDto request)
        {
            // Step 1: Validate Input
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch API Endpoints
            var apiEndpoints = await _dbConnection.QueryAsync<APIEndpoint>("SELECT * FROM APIEndpoints ORDER BY @SortField @SortOrder OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY", new { SortField = request.SortField, SortOrder = request.SortOrder, PageOffset = request.PageOffset, PageLimit = request.PageLimit });

            // Step 3: Pagination Check
            if (request.PageLimit == 0 && request.PageOffset == 0)
            {
                throw new BusinessException("DP-400", "Technical Error");
            }

            // Step 4: Fetch Related Tags
            var apiEndpointIds = apiEndpoints.Select(ae => ae.Id).ToList();
            var tagIds = await _dbConnection.QueryAsync<Guid>("SELECT APITagId FROM APIEndpointTags WHERE APIEndpointId IN @APIEndpointIds", new { APIEndpointIds = apiEndpointIds });
            var tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = tagIds });

            if (tags.Count() != tagIds.Count())
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 5: Response Preparation
            foreach (var apiEndpoint in apiEndpoints)
            {
                apiEndpoint.ApiTags = tags.Where(t => tagIds.Contains(t.Id)).ToList();
            }

            // Step 6: Return the Response
            return apiEndpoints.ToList();
        }
    }
}
