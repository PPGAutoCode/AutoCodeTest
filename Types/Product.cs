
namespace ProjectName.Types
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ApicHostname { get; set; }
        public List<Advantage> Advantages { get; set; }
        public List<Feature> Features { get; set; }
        public List<RelatedProduct> RelatedProducts { get; set; }
        public ProductComparison ProductComparison { get; set; }
        public List<APIEndpoint> APIEndpoints { get; set; }
        public List<ProductTag> ProductTags { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<ProductSubscriber> ProductSubscribers { get; set; }
        public List<ProductDomain> ProductDomainId { get; set; }
        public Guid AttachmentsId { get; set; }
        public Guid AppEnviroment { get; set; }
        public bool Deprecated { get; set; }
        public bool DisableDocumentation { get; set; }
        public string HeaderImage { get; set; }
        public string Label { get; set; }
        public bool OverviewDisplay { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public Guid ImageId { get; set; }
        public bool Visible { get; set; }
        public int Weight { get; set; }
        public string Langcode { get; set; }
        public bool Sticky { get; set; }
        public bool Status { get; set; }
        public bool Promote { get; set; }
        public bool CommercialProduct { get; set; }
        public string ProductVersion { get; set; }
        public int Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public Guid CreatorId { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
