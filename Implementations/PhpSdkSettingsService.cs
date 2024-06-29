
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Interfaces;
using ProjectName.Types;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class PhpSdkSettingsService : IPhpSdkSettingsService
    {
        private readonly IDbConnection _dbConnection;

        public PhpSdkSettingsService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreatePhpSdkSettings(CreatePhpSdkSettingsDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.IDSPortalEnabled) ||
                string.IsNullOrEmpty(request.TokenUrl) ||
                string.IsNullOrEmpty(request.AuthorizationUrl) ||
                string.IsNullOrEmpty(request.UserInfoUrl) ||
                string.IsNullOrEmpty(request.PortalUrl) ||
                string.IsNullOrEmpty(request.ClientId) ||
                string.IsNullOrEmpty(request.ClientSecret) ||
                string.IsNullOrEmpty(request.GrantType) ||
                string.IsNullOrEmpty(request.Scope))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create a new PhpSdkSettings object
            var phpSdkSettings = new PhpSdkSettings
            {
                Id = Guid.NewGuid(),
                IDSPortalEnabled = request.IDSPortalEnabled,
                TokenUrl = request.TokenUrl,
                AuthorizationUrl = request.AuthorizationUrl,
                UserInfoUrl = request.UserInfoUrl,
                PortalUrl = request.PortalUrl,
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                GrantType = request.GrantType,
                Scope = request.Scope,
                HeaderIBMClientId = request.HeaderIBMClientId,
                HeaderIBMClientSecret = request.HeaderIBMClientSecret,
                IDSConnectLogoutUrl = request.IDSConnectLogoutUrl,
                DisplayApplicationAnalytics = request.DisplayApplicationAnalytics,
                Development = request.Development,
                DevelopmentSandboxId = request.DevelopmentSandboxId,
                DevelopmentSumAggregatesPerEndpointUrl = request.DevelopmentSumAggregatesPerEndpointUrl,
                DevelopmentSumAggregatesPerEndpointPerDayUrl = request.DevelopmentSumAggregatesPerEndpointPerDayUrl,
                DevelopmentScopes = request.DevelopmentScopes,
                ProductionSumAggregatesPerEndpointUrl = request.ProductionSumAggregatesPerEndpointUrl,
                ProductionSumAggregatesPerEndpointPerDayUrl = request.ProductionSumAggregatesPerEndpointPerDayUrl,
                ProductionScopes = request.ProductionScopes,
                CertificateValidationUrl = request.CertificateValidationUrl,
                ProductionCertificateValidationUrl = request.ProductionCertificateValidationUrl,
                ReactUrl = request.ReactUrl,
                DrupalUrl = request.DrupalUrl,
                DrupalUrlAdmin = request.DrupalUrlAdmin,
                EmailAddress = request.EmailAddress,
                MaxAppsPerUser = request.MaxAppsPerUser,
                DisableSnippetGeneration = request.DisableSnippetGeneration,
                DisableDocumentationGeneration = request.DisableDocumentationGeneration,
                JsonApiRoles = request.JsonApiRoles,
                Version = 1,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                CreatorId = request.CreatorId,
                ChangedUser = request.CreatorId
            };

            // Step 3: Save the newly created PhpSdkSettings object to the database
            const string sql = @"
                INSERT INTO PhpSdkSettings (
                    Id, IDSPortalEnabled, TokenUrl, AuthorizationUrl, UserInfoUrl, PortalUrl, ClientId, ClientSecret, GrantType, Scope,
                    HeaderIBMClientId, HeaderIBMClientSecret, IDSConnectLogoutUrl, DisplayApplicationAnalytics, Development, DevelopmentSandboxId,
                    DevelopmentSumAggregatesPerEndpointUrl, DevelopmentSumAggregatesPerEndpointPerDayUrl, DevelopmentScopes, ProductionSumAggregatesPerEndpointUrl,
                    ProductionSumAggregatesPerEndpointPerDayUrl, ProductionScopes, CertificateValidationUrl, ProductionCertificateValidationUrl, ReactUrl,
                    DrupalUrl, DrupalUrlAdmin, EmailAddress, MaxAppsPerUser, DisableSnippetGeneration, DisableDocumentationGeneration, JsonApiRoles,
                    Version, Created, Changed, CreatorId, ChangedUser
                )
                VALUES (
                    @Id, @IDSPortalEnabled, @TokenUrl, @AuthorizationUrl, @UserInfoUrl, @PortalUrl, @ClientId, @ClientSecret, @GrantType, @Scope,
                    @HeaderIBMClientId, @HeaderIBMClientSecret, @IDSConnectLogoutUrl, @DisplayApplicationAnalytics, @Development, @DevelopmentSandboxId,
                    @DevelopmentSumAggregatesPerEndpointUrl, @DevelopmentSumAggregatesPerEndpointPerDayUrl, @DevelopmentScopes, @ProductionSumAggregatesPerEndpointUrl,
                    @ProductionSumAggregatesPerEndpointPerDayUrl, @ProductionScopes, @CertificateValidationUrl, @ProductionCertificateValidationUrl, @ReactUrl,
                    @DrupalUrl, @DrupalUrlAdmin, @EmailAddress, @MaxAppsPerUser, @DisableSnippetGeneration, @DisableDocumentationGeneration, @JsonApiRoles,
                    @Version, @Created, @Changed, @CreatorId, @ChangedUser
                );
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, phpSdkSettings);
                return phpSdkSettings.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
