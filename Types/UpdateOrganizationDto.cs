
namespace ProjectName.Types
{
    public class UpdateOrganizationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid? ImageId { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public Guid File { get; set; }
    }
}
