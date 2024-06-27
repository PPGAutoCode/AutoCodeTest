
using System;

namespace ProjectName.Types
{
    public class UpdateSupportTicketStateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
