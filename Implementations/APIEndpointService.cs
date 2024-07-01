
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
            if (request.Name == null || request.Deprecated == null || request.Version == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Tags
            var existingTags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = request.ApiTags });
            if (existingTags.Count() != request.ApiTags.Count)
            {
                throw new TechnicalException("DP-404", "Technical Error");
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
                Deprecated = request.Deprecated.Value,
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
            var apiEndpointTags = request.ApiTags.Select(tagId => new APIEndpointTag { APIEndpointId = apiEndpoint.Id, ApiTagId = tagId }).ToList();
            apiEndpoint.ApiTags = apiEndpointTags;

            // Step 5: Database Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert the apiEndpoint object into the APIEndpoints database table
                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpoints (Id, Name, ApiContext, ApiReferenceId, ApiResource, ApiScope, ApiScopeProduction, ApiSecurity, Deprecated, Description, Documentation, EndpointUrls, EnvironmentId, ProviderId, Swagger, Tour, Updated, Version) VALUES (@Id, @Name, @ApiContext, @ApiReferenceId, @ApiResource, @ApiScope, @ApiScopeProduction, @ApiSecurity, @Deprecated, @Description, @Documentation, @EndpointUrls, @EnvironmentId, @ProviderId, @Swagger, @Tour, @Updated, @Version)", apiEndpoint, transaction);

                    // Insert the related APIEndpointTags into the respective database table
                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (APIEndpointId, ApiTagId) VALUES (@APIEndpointId, @ApiTagId)", apiEndpointTags, transaction);

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
                var tagIds = await _dbConnection.QueryAsync<Guid>("SELECT ApiTagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = apiEndpoint.Id });
                var tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = tagIds });

                if (tags.Count() != tagIds.Count())
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                apiEndpoint.ApiTags = tags.ToList();
            }

            // Step 4: Map and Return
            if (apiEndpoint == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return apiEndpoint;
        }

        public async Task<string> UpdateAPIEndpoint(UpdateAPIEndpointDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == null || request.ApiScope == null || request.Description == null || request.Version == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing API Endpoint
            var existingAPIEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            if (existingAPIEndpoint == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Validate Related Entities
            if (request.ApiTags != null)
            {
                var existingTags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = request.ApiTags });
                if (existingTags.Count() != request.ApiTags.Count)
                {
                    throw new BusinessException("DP-422", "Client Error");
                }
            }

            // Step 4: Update the APIEndpoint object with the provided changes
            existingAPIEndpoint.ApiScope = request.ApiScope;
            existingAPIEndpoint.Description = request.Description;
            existingAPIEndpoint.Version = (int.Parse(existingAPIEndpoint.Version) + 1).ToString();
            existingAPIEndpoint.Updated = DateTime.UtcNow;

            // Step 5: Update APIEndpointTags
            var existingTags = await _dbConnection.QueryAsync<APIEndpointTag>("SELECT * FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingAPIEndpoint.Id });
            var tagsToRemove = existingTags.Where(et => !request.ApiTags.Contains(et.ApiTagId)).ToList();
            var tagsToAdd = request.ApiTags.Where(rt => !existingTags.Select(et => et.ApiTagId).Contains(rt)).Select(rt => new APIEndpointTag { APIEndpointId = existingAPIEndpoint.Id, ApiTagId = rt }).ToList();

            // Step 6: Save Changes to Database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Update APIEndpoints Table
                    await _dbConnection.ExecuteAsync("UPDATE APIEndpoints SET ApiScope = @ApiScope, Description = @Description, Version = @Version, Updated = @Updated WHERE Id = @Id", existingAPIEndpoint, transaction);

                    // Remove the old tags from the database
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId AND ApiTagId IN @ApiTagIds", new { APIEndpointId = existingAPIEndpoint.Id, ApiTagIds = tagsToRemove.Select(t => t.ApiTagId) }, transaction);

                    // Insert the new tags into the database
                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (APIEndpointId, ApiTagId) VALUES (@APIEndpointId, @ApiTagId)", tagsToAdd, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "Technical Error");
                }
            }

            // Step 7: Return the updated API endpoint ID
            return existingAPIEndpoint.Id.ToString();
        }

        public async Task<bool> DeleteAPIEndpoint(DeleteAPIEndpointDto request)
        {
            // Step 1: Validate Input
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Existing API Endpoint
            var existingAPIEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            if (existingAPIEndpoint == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Fetch Related Tags
            var tagIds = await _dbConnection.QueryAsync<Guid>("SELECT ApiTagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingAPIEndpoint.Id });
            var tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = tagIds });

            if (tags.Count() != tagIds.Count())
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 4: Delete the APIEndpoint
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Remove the APIEndpoint object from the database
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpoints WHERE Id = @Id", new { Id = existingAPIEndpoint.Id }, transaction);

                    // Ensure that all related entries in the APIEndpointTags table are also appropriately handled (deleted)
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingAPIEndpoint.Id }, transaction);

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
            if (request.PageLimit == null || request.PageOffset == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch API Endpoints
            var apiEndpoints = await _dbConnection.QueryAsync<APIEndpoint>("SELECT * FROM APIEndpoints ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY", new { Offset = request.PageOffset, Limit = request.PageLimit });

            // Step 3: Pagination Check
            if (request.PageLimit == 0 && request.PageOffset == 0)
            {
                throw new TechnicalException("DP-400", "Technical Error");
            }

            // Step 4: Fetch Related Tags
            foreach (var apiEndpoint in apiEndpoints)
            {
                var tagIds = await _dbConnection.QueryAsync<Guid>("SELECT ApiTagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = apiEndpoint.Id });
                var tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = tagIds });

                if (tags.Count() != tagIds.Count())
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }

                apiEndpoint.ApiTags = tags.ToList();
            }

            // Step 5: Return the Response
            return apiEndpoints.ToList();
        }
    }
}
