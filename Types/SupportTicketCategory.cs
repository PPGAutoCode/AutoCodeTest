
namespace ProjectName.Types
{
    public class SupportTicketCategory
    {
        public Guid Id { get; set; }
        public Guid SupportTicketId { get; set; }
        public Guid SupportCategoryId { get; set; }
        public string? Name { get; set; }
    }
}
