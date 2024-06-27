
using System;

namespace ProjectName.Types
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public Guid Document { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
    }
}
    