
namespace ProjectName.Types
{
    using System;

    public class UserType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public Guid CreatorId { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
