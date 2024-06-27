
namespace ProjectName.Types
{
    public class UpdateSupportTicketDTO
    {
        public Guid Id { get; set; }
        public Guid ReportedBy { get; set; }
        public Guid AssignedTo { get; set; }
        public string ContactDetails { get; set; }
        public string DateClosed { get; set; }
        public Guid EnvironmentImpacted { get; set; }
        public string NameOfReportingOrganization { get; set; }
        public string Priority { get; set; }
        public Guid Severity { get; set; }
        public string ShortDescription { get; set; }
        public string State { get; set; }
        public List<Guid> SupportCategories { get; set; }
        public List<Message> Messages { get; set; }
        public int Version { get; set; }
        public DateTime Changed { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
