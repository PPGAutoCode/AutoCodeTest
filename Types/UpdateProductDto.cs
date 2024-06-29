
namespace ProjectName.Types
{
    public class UpdateProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<Guid>? Advantage { get; set; }
        public List<Guid>? Features { get; set; }
        public List<Guid>? RelatedProducts { get; set; }
        public List<Guid>? APIEndpoint { get; set; }
        public List<string>? Tags { get; set; }
        public List<Guid>? Subscribers { get; set; }
        public Guid? Comparison { get; set; }
        public Guid? CategoryId { get; set; }
        public string Version { get; set; }
        public bool? Enabled { get; set; }
        public string? ApicHostname { get; set; }
        public string? Attachments { get; set; }
        public string? EnvironmentId { get; set; }
        public string? HeaderImage { get; set; }
        public string? Label { get; set; }
        public bool? OverviewDisplay { get; set; }
        public string? Domain { get; set; }
        public string? Image { get; set; }
        public int? Weight { get; set; }
        public string? Langcode { get; set; }
        public bool? Sticky { get; set; }
        public bool? Status { get; set; }
        public bool? Promote { get; set; }
        public bool? CommercialProduct { get; set; }
    }
}
