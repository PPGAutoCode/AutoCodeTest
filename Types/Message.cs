
// Message.cs
using System;

namespace ProjectName.Types
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid SupportTicketId { get; set; }
        public string Body { get; set; }
    }
}
