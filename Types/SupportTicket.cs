
namespace ProjectName.Types
{
    public class SupportTicket
    {
        public Guid Id { get; set; }
        public Guid ReportedBy { get; set; }
        public Guid AssignedTo { get; set; }
        public string ContactDetails { get; set; }
        public string DateClosed { get; set; }
        public AppEnvironment AppEnvironmentImpacted { get; set; }
        public string NameOfReportingOrganization { get; set; }
        public string Priority { get; set; }
        public Severity SeverityId { get; set; }
        public string ShortDescription { get; set; }
        public string State { get; set; }
        public List<SupportCategory> SupportCategories { get; set; }
        public List<Message> Message { get; set; }
        public int Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public Guid CreatorId { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
