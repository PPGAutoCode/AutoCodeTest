
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

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
            ValidateCreateAPIEndpointDto(request);

            // Step 2: Fetch Tags
            var tags = await FetchTagsAsync(request.ApiTags);

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
                ApiTags = tags,
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
            var apiEndpointTags = request.ApiTags.Select(tagId => new APIEndpointTag
            {
                APIEndpointId = apiEndpoint.Id,
                TagId = tagId
            }).ToList();

            // Step 5: Database Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert the apiEndpoint object into the APIEndpoints database table
                    const string insertAPIEndpointQuery = @"
                        INSERT INTO APIEndpoints (Id, Name, ApiContext, ApiReferenceId, ApiResource, ApiScope, ApiScopeProduction, ApiSecurity, Deprecated, Description, Documentation, EndpointUrls, EnvironmentId, ProviderId, Swagger, Tour, Updated, Version)
                        VALUES (@Id, @Name, @ApiContext, @ApiReferenceId, @ApiResource, @ApiScope, @ApiScopeProduction, @ApiSecurity, @Deprecated, @Description, @Documentation, @EndpointUrls, @EnvironmentId, @ProviderId, @Swagger, @Tour, @Updated, @Version)";
                    await _dbConnection.ExecuteAsync(insertAPIEndpointQuery, apiEndpoint, transaction);

                    // Insert the related APIEndpointTags into the respective database table
                    const string insertAPIEndpointTagsQuery = @"
                        INSERT INTO APIEndpointTags (APIEndpointId, TagId)
                        VALUES (@APIEndpointId, @TagId)";
                    await _dbConnection.ExecuteAsync(insertAPIEndpointTagsQuery, apiEndpointTags, transaction);

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

        private void ValidateCreateAPIEndpointDto(CreateAPIEndpointDto request)
        {
            if (string.IsNullOrEmpty(request.Name) || request.Deprecated == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }
        }

        private async Task<List<ApiTag>> FetchTagsAsync(List<Guid> tagIds)
        {
            const string fetchTagsQuery = @"
                SELECT * FROM ApiTags WHERE Id IN @tagIds";
            var tags = await _dbConnection.QueryAsync<ApiTag>(fetchTagsQuery, new { tagIds });

            if (tags.Count() != tagIds.Count)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return tags.ToList();
        }
    }
}
