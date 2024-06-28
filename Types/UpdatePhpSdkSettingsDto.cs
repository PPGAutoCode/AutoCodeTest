
namespace ProjectName.Types
{
    public class UpdatePhpSdkSettingsDto
    {
        public string Id { get; set; }
        public string IDSPortalEnabled { get; set; }
        public string TokenUrl { get; set; }
        public string AuthorizationUrl { get; set; }
        public string UserInfoUrl { get; set; }
        public string PortalUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrantType { get; set; }
        public string Scope { get; set; }
        public string HeaderIBMClientId { get; set; }
        public string HeaderIBMClientSecret { get; set; }
        public string IDSConnectLogoutUrl { get; set; }
        public string DisplayApplicationAnalytics { get; set; }
        public string Development { get; set; }
        public string DevelopmentSandboxId { get; set; }
        public string DevelopmentSumAggregatesPerEndpointUrl { get; set; }
        public string DevelopmentSumAggregatesPerEndpointPerDayUrl { get; set; }
        public string DevelopmentScopes { get; set; }
        public string ProductionSumAggregatesPerEndpointUrl { get; set; }
        public string ProductionSumAggregatesPerEndpointPerDayUrl { get; set; }
        public string ProductionScopes { get; set; }
        public string CertificateValidationUrl { get; set; }
        public string ProductionCertificateValidationUrl { get; set; }
        public string ReactUrl { get; set; }
        public string DrupalUrl { get; set; }
        public string DrupalUrlAdmin { get; set; }
        public string EmailAddress { get; set; }
        public string MaxAppsPerUser { get; set; }
        public string DisableSnippetGeneration { get; set; }
        public string DisableDocumentationGeneration { get; set; }
        public string JsonApiRoles { get; set; }
    }
}
