
using System;

namespace ProjectName.Types
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public Guid ProductsId { get; set; }
        public Guid ApplicationsId { get; set; }
        public int? Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public Guid CreatorId { get; set; }
        public Guid ChangedUser { get; set; }
        public Application Application { get; set; }
        public Product Product { get; set; }
    }
}
