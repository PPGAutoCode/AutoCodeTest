
namespace ProjectName.Types
{
    using System;

    public class UserRole
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string HelpText { get; set; }
        public string ReferenceMethod { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
    }
}
