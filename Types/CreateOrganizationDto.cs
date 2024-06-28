
namespace ProjectName.Types
{
    public class CreateOrganizationDto
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public Guid File { get; set; }
    }
}
