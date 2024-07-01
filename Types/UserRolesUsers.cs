
namespace ProjectName.Types
{
    public class UserRolesUsers
    {
        public Guid id { get; set; }
        public Guid UsersId { get; set; }
        public Guid UserRolesId { get; set; }
        public int? Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public Guid CreatorId { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
