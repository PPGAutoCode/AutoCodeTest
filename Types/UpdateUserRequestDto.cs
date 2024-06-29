
namespace ProjectName.Types
{
    public class UpdateUserRequestDto
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public bool Status { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public bool ContactSettings { get; set; }
        public string SiteLanguage { get; set; }
        public string LocaleSettings { get; set; }
        public List<Guid> UserRoles { get; set; }
        public Guid? ImageId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string IBMUId { get; set; }
        public int MaxNumApps { get; set; }
        public List<string> CompletedStories { get; set; }
        public List<Guid> UserType { get; set; }
        public Guid? UserQuestionnaire { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastAccess { get; set; }
    }
}
