
namespace ProjectName.Types
{
    public class UpdateUserRoleDto
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string? HelpText { get; set; }
        public string ReferenceMethod { get; set; }
        public List<Guid> DefaultValue { get; set; }
    }
}
