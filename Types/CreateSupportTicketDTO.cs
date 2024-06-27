
namespace ProjectName.Types
{
    public class CreateSupportTicketDTO
    {
        public Guid ReportedBy { get; set; }
        public Guid AssignedTo { get; set; }
        public string ContactDetails { get; set; }
        public Guid FileId { get; set; }
        public Guid EnvironmentImpacted { get; set; }
        public string NameOfReportingOrganization { get; set; }
        public string Priority { get; set; }
        public Guid SeverityId { get; set; }
        public string ShortDescription { get; set; }
        public string State { get; set; }
        public Message Message { get; set; }
        public List<Guid> SupportCategories { get; set; }
    }
}
