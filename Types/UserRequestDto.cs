
namespace ProjectName.Types
{
    using System;

    public class UserRequestDto
    {
        public Guid? Id { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
