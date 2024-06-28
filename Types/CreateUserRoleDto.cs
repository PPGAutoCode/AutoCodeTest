
// File: CreateUserRoleDto.cs
namespace ProjectName.Types
{
    public class CreateUserRoleDto
    {
        public string Label { get; set; }
        public string HelpText { get; set; }
        public string? ReferenceMethod { get; set; }
        public List<Guid> DefaultValue { get; set; }
    }
}
