
namespace ProjectName.Types
{
    public class UpdateStoryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? ShortDescription { get; set; }
        public int StoryNumber { get; set; }
        public Guid? CompletedCondition { get; set; }
        public List<Guid> Steps { get; set; }
    }
}
