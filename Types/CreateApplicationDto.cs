
// File: CreateApplicationDto.cs
namespace ProjectName.Types
{
    public class CreateApplicationDto
    {
        public string AllowedGrantTypes { get; set; }
        public string Name { get; set; }
        public string APICHostname { get; set; }
        public string CredentialsDev { get; set; }
        public string CredentialsProd { get; set; }
        public string Description { get; set; }
        public Guid? IdDev { get; set; }
        public Guid? IdProd { get; set; }
        public Guid? ImageId { get; set; }
        public string Status { get; set; }
        public Guid? ClientIdDev { get; set; }
        public Guid? ClientIdProd { get; set; }
        public string ClientSecret { get; set; }
        public string ClientUriDev { get; set; }
        public string ClientUriProd { get; set; }
        public Guid EnvironmentId { get; set; }
        public List<Guid> Subscriptions { get; set; }
        public List<string> AppTags { get; set; }
    }
}
