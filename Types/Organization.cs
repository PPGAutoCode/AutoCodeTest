
namespace ProjectName.Types
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid? ImageId { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public Guid FileId { get; set; }
    }
}
