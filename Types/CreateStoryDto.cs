
namespace ProjectName.Types
{
    public class CreateStoryDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Number { get; set; }
        public Guid? CompletedCondition { get; set; }
        public List<Guid> Steps { get; set; }
    }
}
