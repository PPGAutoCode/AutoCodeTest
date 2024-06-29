
// File: Product.cs
namespace ProjectName.Types
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? ApicHostname { get; set; }
        public List<Advantage>? Advantages { get; set; }
        public List<Feature>? Features { get; set; }
        public List<RelatedProducts>? RelatedProducts { get; set; }
        public Comparison? Comparison { get; set; }
        public List<APIEndpoint>? APIEndpoint { get; set; }
        public List<ProductTag>? Tags { get; set; }
        public List<ProductCategories>? Categories { get; set; }
        public string? attachments { get; set; }
        public bool? Deprecated { get; set; }
        public bool? DisableDocumentation { get; set; }
        public string? EnvironmentId { get; set; }
        public string? HeaderImage { get; set; }
        public string? Label { get; set; }
        public bool? OverviewDisplay { get; set; }
        public string? Description { get; set; }
        public string? Domain { get; set; }
        public bool? Enabled { get; set; }
        public string Name { get; set; }
        public string? ImageId { get; set; }
        public bool? Visible { get; set; }
        public int? Weight { get; set; }
        public string? Langcode { get; set; }
        public bool? Sticky { get; set; }
        public bool? Status { get; set; }
        public bool? Promote { get; set; }
        public bool? CommercialProduct { get; set; }
        public int Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public Guid CreatorId { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
