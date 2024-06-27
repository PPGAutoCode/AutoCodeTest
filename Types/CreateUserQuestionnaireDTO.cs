
// File: CreateUserQuestionnaireDTO.cs
namespace ProjectName.Types
{
    public class CreateUserQuestionnaireDTO
    {
        public string Label { get; set; }
        public string HelpText { get; set; }
        public string ReferenceMethod { get; set; }
        public List<Guid> DefaultValue { get; set; }
    }
}
