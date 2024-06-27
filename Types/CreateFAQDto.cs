
namespace ProjectName.Types
{
    public class CreateFAQDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public Guid? FaqCategoryId { get; set; }
        public string Langcode { get; set; }
        public bool Status { get; set; }
        public string Order { get; set; }
    }
}
