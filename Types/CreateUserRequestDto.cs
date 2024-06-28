
namespace ProjectName.Types
{
    using System;
    using System.Collections.Generic;

    public class CreateUserRequestDto
    {
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
        public int? MaxNumApps { get; set; }
        public List<string> CompletedStories { get; set; }
        public Enum UserType { get; set; }
        public Guid? UserQuestionnaire { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastAccess { get; set; }
    }
}
