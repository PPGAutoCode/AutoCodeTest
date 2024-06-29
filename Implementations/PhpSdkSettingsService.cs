
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
            // Step 1: Validation
            if (string.IsNullOrEmpty(request.IDSPortalEnabled) || string.IsNullOrEmpty(request.TokenUrl) ||
                string.IsNullOrEmpty(request.AuthorizationUrl) || string.IsNullOrEmpty(request.UserInfoUrl) ||
                string.IsNullOrEmpty(request.PortalUrl) || string.IsNullOrEmpty(request.ClientId) ||
                string.IsNullOrEmpty(request.ClientSecret) || string.IsNullOrEmpty(request.GrantType) ||
                string.IsNullOrEmpty(request.Scope))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Create PhpSdkSettings object
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

            // Step 3: Save to database
            const string sql = @"
                INSERT INTO PhpSdkSettings (
                    Id, IDSPortalEnabled, TokenUrl, AuthorizationUrl, UserInfoUrl, PortalUrl, ClientId, ClientSecret, GrantType, Scope,
                    HeaderIBMClientId, HeaderIBMClientSecret, IDSConnectLogoutUrl, DisplayApplicationAnalytics, Development, DevelopmentSandboxId,
                    DevelopmentSumAggregatesPerEndpointUrl, DevelopmentSumAggregatesPerEndpointPerDayUrl, DevelopmentScopes, ProductionSumAggregatesPerEndpointUrl,
                    ProductionSumAggregatesPerEndpointPerDayUrl, ProductionScopes, CertificateValidationUrl, ProductionCertificateValidationUrl, ReactUrl,
                    DrupalUrl, DrupalUrlAdmin, EmailAddress, MaxAppsPerUser, DisableSnippetGeneration, DisableDocumentationGeneration, JsonApiRoles,
                    Version, Created, Changed, CreatorId, ChangedUser
                ) VALUES (
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

        public async Task<PhpSdkSettings> GetPhpSdkSettings(PhpSdkSettingsRequestDto request)
        {
            // Step 1: Validation
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Retrieve from database
            const string sql = "SELECT * FROM PhpSdkSettings WHERE Id = @Id;";

            try
            {
                var result = await _dbConnection.QuerySingleOrDefaultAsync<PhpSdkSettings>(sql, new { Id = request.Id });
                if (result == null)
                {
                    throw new TechnicalException("DP-404", "Technical Error");
                }
                return result;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<string> UpdatePhpSdkSettings(UpdatePhpSdkSettingsDto request)
        {
            // Step 1: Validation
            if (string.IsNullOrEmpty(request.IDSPortalEnabled) || string.IsNullOrEmpty(request.TokenUrl) ||
                string.IsNullOrEmpty(request.AuthorizationUrl) || string.IsNullOrEmpty(request.UserInfoUrl) ||
                string.IsNullOrEmpty(request.PortalUrl) || string.IsNullOrEmpty(request.ClientId) ||
                string.IsNullOrEmpty(request.ClientSecret) || string.IsNullOrEmpty(request.GrantType) ||
                string.IsNullOrEmpty(request.Scope))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Retrieve existing PhpSdkSettings
            var existingSettings = await GetPhpSdkSettings(new PhpSdkSettingsRequestDto { Id = request.Id });
            if (existingSettings == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Update the retrieved object
            existingSettings.IDSPortalEnabled = request.IDSPortalEnabled;
            existingSettings.TokenUrl = request.TokenUrl;
            existingSettings.AuthorizationUrl = request.AuthorizationUrl;
            existingSettings.UserInfoUrl = request.UserInfoUrl;
            existingSettings.PortalUrl = request.PortalUrl;
            existingSettings.ClientId = request.ClientId;
            existingSettings.ClientSecret = request.ClientSecret;
            existingSettings.GrantType = request.GrantType;
            existingSettings.Scope = request.Scope;
            existingSettings.HeaderIBMClientId = request.HeaderIBMClientId;
            existingSettings.HeaderIBMClientSecret = request.HeaderIBMClientSecret;
            existingSettings.IDSConnectLogoutUrl = request.IDSConnectLogoutUrl;
            existingSettings.DisplayApplicationAnalytics = request.DisplayApplicationAnalytics;
            existingSettings.Development = request.Development;
            existingSettings.DevelopmentSandboxId = request.DevelopmentSandboxId;
            existingSettings.DevelopmentSumAggregatesPerEndpointUrl = request.DevelopmentSumAggregatesPerEndpointUrl;
            existingSettings.DevelopmentSumAggregatesPerEndpointPerDayUrl = request.DevelopmentSumAggregatesPerEndpointPerDayUrl;
            existingSettings.DevelopmentScopes = request.DevelopmentScopes;
            existingSettings.ProductionSumAggregatesPerEndpointUrl = request.ProductionSumAggregatesPerEndpointUrl;
            existingSettings.ProductionSumAggregatesPerEndpointPerDayUrl = request.ProductionSumAggregatesPerEndpointPerDayUrl;
            existingSettings.ProductionScopes = request.ProductionScopes;
            existingSettings.CertificateValidationUrl = request.CertificateValidationUrl;
            existingSettings.ProductionCertificateValidationUrl = request.ProductionCertificateValidationUrl;
            existingSettings.ReactUrl = request.ReactUrl;
            existingSettings.DrupalUrl = request.DrupalUrl;
            existingSettings.DrupalUrlAdmin = request.DrupalUrlAdmin;
            existingSettings.EmailAddress = request.EmailAddress;
            existingSettings.MaxAppsPerUser = request.MaxAppsPerUser;
            existingSettings.DisableSnippetGeneration = request.DisableSnippetGeneration;
            existingSettings.DisableDocumentationGeneration = request.DisableDocumentationGeneration;
            existingSettings.JsonApiRoles = request.JsonApiRoles;
            existingSettings.Changed = DateTime.UtcNow;
            existingSettings.ChangedUser = request.ChangedUser;

            // Step 4: Save to database
            const string sql = @"
                UPDATE PhpSdkSettings SET
                    IDSPortalEnabled = @IDSPortalEnabled, TokenUrl = @TokenUrl, AuthorizationUrl = @AuthorizationUrl, UserInfoUrl = @UserInfoUrl,
                    PortalUrl = @PortalUrl, ClientId = @ClientId, ClientSecret = @ClientSecret, GrantType = @GrantType, Scope = @Scope,
                    HeaderIBMClientId = @HeaderIBMClientId, HeaderIBMClientSecret = @HeaderIBMClientSecret, IDSConnectLogoutUrl = @IDSConnectLogoutUrl,
                    DisplayApplicationAnalytics = @DisplayApplicationAnalytics, Development = @Development, DevelopmentSandboxId = @DevelopmentSandboxId,
                    DevelopmentSumAggregatesPerEndpointUrl = @DevelopmentSumAggregatesPerEndpointUrl, DevelopmentSumAggregatesPerEndpointPerDayUrl = @DevelopmentSumAggregatesPerEndpointPerDayUrl,
                    DevelopmentScopes = @DevelopmentScopes, ProductionSumAggregatesPerEndpointUrl = @ProductionSumAggregatesPerEndpointUrl,
                    ProductionSumAggregatesPerEndpointPerDayUrl = @ProductionSumAggregatesPerEndpointPerDayUrl, ProductionScopes = @ProductionScopes,
                    CertificateValidationUrl = @CertificateValidationUrl, ProductionCertificateValidationUrl = @ProductionCertificateValidationUrl,
                    ReactUrl = @ReactUrl, DrupalUrl = @DrupalUrl, DrupalUrlAdmin = @DrupalUrlAdmin, EmailAddress = @EmailAddress,
                    MaxAppsPerUser = @MaxAppsPerUser, DisableSnippetGeneration = @DisableSnippetGeneration, DisableDocumentationGeneration = @DisableDocumentationGeneration,
                    JsonApiRoles = @JsonApiRoles, Changed = @Changed, ChangedUser = @ChangedUser
                WHERE Id = @Id;
            ";

            try
            {
                await _dbConnection.ExecuteAsync(sql, existingSettings);
                return existingSettings.Id.ToString();
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeletePhpSdkSettings(DeletePhpSdkSettingsDto request)
        {
            // Step 1: Validation
            if (request.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 2: Retrieve existing PhpSdkSettings
            var existingSettings = await GetPhpSdkSettings(new PhpSdkSettingsRequestDto { Id = request.Id });
            if (existingSettings == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            // Step 3: Delete from database
            const string sql = "DELETE FROM PhpSdkSettings WHERE Id = @Id;";

            try
            {
                await _dbConnection.ExecuteAsync(sql, new { Id = request.Id });
                return true;
            }
            catch (Exception)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
