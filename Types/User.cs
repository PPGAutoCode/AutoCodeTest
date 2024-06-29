
namespace ProjectName.Types
{
    public class User
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public bool Status { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public bool? ContactSettings { get; set; }
        public string SiteLanguage { get; set; }
        public string LocaleSettings { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public string Image { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string IBM_UId { get; set; }
        public int? MaxNumApps { get; set; }
        public List<string> CompletedStories { get; set; }
        public Guid? UserType { get; set; }
        public Guid? UserQuestionnaire { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastAccess { get; set; }
    }
}
