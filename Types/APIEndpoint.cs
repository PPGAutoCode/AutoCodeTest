
// File: APIEndpoint.cs
namespace ProjectName.Types
{
    public class APIEndpoint
    {
        public string Id { get; set; }
        public string? ApiContext { get; set; }
        public string? ApiReferenceId { get; set; }
        public string? ApiResource { get; set; }
        public string? ApiScope { get; set; }
        public string? ApiScopeProduction { get; set; }
        public string? ApiSecurity { get; set; }
        public string? ApiTags { get; set; }
        public bool Deprecated { get; set; }
        public string? Description { get; set; }
        public string? Documentation { get; set; }
        public string? EndpointUrls { get; set; }
        public string? EnvironmentId { get; set; }
        public string? Swagger { get; set; }
        public string? Tour { get; set; }
        public DateTime? Updated { get; set; }
        public string? Version { get; set; }
    }
}
