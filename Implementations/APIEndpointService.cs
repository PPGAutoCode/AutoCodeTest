
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
            if (string.IsNullOrEmpty(request.Name) || request.Deprecated == null || string.IsNullOrEmpty(request.Version))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Fetch Tags
            var existingTags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Name IN @Names", new { Names = request.ApiTags });
            if (existingTags.Count() != request.ApiTags.Count)
            {
                throw new BusinessException("DP-404", "Technical Error");
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
            var apiEndpointTags = existingTags.Select(tag => new APIEndpointTag
            {
                Id = Guid.NewGuid(),
                APIEndpointId = apiEndpoint.Id,
                APITagId = tag.Id
            }).ToList();

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

            // Step 7: Return APIEndpointId
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
            var existingAPIEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            if (existingAPIEndpoint == null)
            {
                throw new BusinessException("DP-404", "Technical Error");
            }

            // Step 3: Validate Related Entities
            if (request.ApiTags != null)
            {
                var existingTags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Name IN @Names", new { Names = request.ApiTags });
                if (existingTags.Count() != request.ApiTags.Count)
                {
                    throw new BusinessException("DP-422", "Client Error");
                }
            }

            // Step 4: Update the APIEndpoint object with the provided changes
            existingAPIEndpoint.Name = request.Name;
            existingAPIEndpoint.ApiContext = request.ApiContext;
            existingAPIEndpoint.ApiReferenceId = request.ApiReferenceId;
            existingAPIEndpoint.ApiResource = request.ApiResource;
            existingAPIEndpoint.ApiScope = request.ApiScope;
            existingAPIEndpoint.ApiScopeProduction = request.ApiScopeProduction;
            existingAPIEndpoint.ApiSecurity = request.ApiSecurity;
            existingAPIEndpoint.Deprecated = request.Deprecated;
            existingAPIEndpoint.Description = request.Description;
            existingAPIEndpoint.Documentation = request.Documentation;
            existingAPIEndpoint.EndpointUrls = request.EndpointUrls;
            existingAPIEndpoint.EnvironmentId = request.EnvironmentId;
            existingAPIEndpoint.ProviderId = request.ProviderId;
            existingAPIEndpoint.Swagger = request.Swagger;
            existingAPIEndpoint.Tour = request.Tour;
            existingAPIEndpoint.Updated = DateTime.UtcNow;
            existingAPIEndpoint.Version = request.Version;

            // Step 5: Update APIEndpointTags
            var existingTags = await _dbConnection.QueryAsync<APIEndpointTag>("SELECT * FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingAPIEndpoint.Id });
            var existingTagIds = existingTags.Select(t => t.APITagId).ToList();

            var newTags = request.ApiTags?.Select(tag => new APIEndpointTag
            {
                Id = Guid.NewGuid(),
                APIEndpointId = existingAPIEndpoint.Id,
                APITagId = tag
            }).ToList();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try:
                    # Update APIEndpoints Table
                    await _dbConnection.ExecuteAsync("UPDATE APIEndpoints SET Name = @Name, ApiContext = @ApiContext, ApiReferenceId = @ApiReferenceId, ApiResource = @ApiResource, ApiScope = @ApiScope, ApiScopeProduction = @ApiScopeProduction, ApiSecurity = @ApiSecurity, Deprecated = @Deprecated, Description = @Description, Documentation = @Documentation, EndpointUrls = @EndpointUrls, EnvironmentId = @EnvironmentId, ProviderId = @ProviderId, Swagger = @Swagger, Tour = @Tour, Updated = @Updated, Version = @Version WHERE Id = @Id", existingAPIEndpoint, transaction)

                    # Remove the old tags from the database
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingAPIEndpoint.Id }, transaction)

                    # Insert the new tags into the database
                    if (newTags != None):
                        await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (Id, APIEndpointId, APITagId) VALUES (@Id, @APIEndpointId, @APITagId)", newTags, transaction)

                    transaction.Commit()
                except Exception:
                    transaction.Rollback()
                    raise TechnicalException("DP-500", "Technical Error")

            return existingAPIEndpoint.Id.ToString()

        public async Task<bool> DeleteAPIEndpoint(DeleteAPIEndpointDto request)
        {
            # Step 1: Validate Input
            if (request.Id == None):
                raise BusinessException("DP-422", "Client Error")

            # Step 2: Fetch Existing API Endpoint
            existingAPIEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id })
            if (existingAPIEndpoint == None):
                raise BusinessException("DP-404", "Technical Error")

            # Step 3: Fetch Related Tags
            tagIds = await _dbConnection.QueryAsync<Guid>("SELECT APITagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingAPIEndpoint.Id })
            tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = tagIds })

            if (tags.Count() != tagIds.Count()):
                raise BusinessException("DP-404", "Technical Error")

            # Step 4: Delete the APIEndpoint
            using (transaction = _dbConnection.BeginTransaction()):
                try:
                    # Remove the APIEndpoint object from the database
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpoints WHERE Id = @Id", new { Id = existingAPIEndpoint.Id }, transaction)

                    # Ensure that all related entries in the APIEndpointTags table are also appropriately handled (deleted)
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingAPIEndpoint.Id }, transaction)

                    transaction.Commit()
                except Exception:
                    transaction.Rollback()
                    raise TechnicalException("DP-500", "Technical Error")

            return True

        public async Task<List<APIEndpoint>> GetListAPIEndpoint(ListAPIEndpointRequestDto request)
        {
            # Step 1: Validate Input
            if (request.PageLimit <= 0 or request.PageOffset < 0):
                raise BusinessException("DP-422", "Client Error")

            # Step 2: Fetch API Endpoints
            apiEndpoints = await _dbConnection.QueryAsync<APIEndpoint>("SELECT * FROM APIEndpoints ORDER BY @SortField @SortOrder LIMIT @PageLimit OFFSET @PageOffset", new { SortField = request.SortField, SortOrder = request.SortOrder, PageLimit = request.PageLimit, PageOffset = request.PageOffset })

            # Step 3: Pagination Check
            if (request.PageLimit == 0 and request.PageOffset == 0):
                raise BusinessException("DP-400", "Technical Error")

            # Step 4: Fetch Related Tags
            apiEndpointIds = apiEndpoints.Select(ae => ae.Id).ToList()
            tagIds = await _dbConnection.QueryAsync<Guid>("SELECT APITagId FROM APIEndpointTags WHERE APIEndpointId IN @APIEndpointIds", new { APIEndpointIds = apiEndpointIds })
            tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = tagIds })

            if (tags.Count() != tagIds.Count()):
                raise BusinessException("DP-404", "Technical Error")

            # Step 5: Response Preparation
            for apiEndpoint in apiEndpoints:
                apiEndpoint.ApiTags = tags.Where(t => tagIds.Contains(t.Id)).ToList()

            return apiEndpoints.ToList()
    }
}
