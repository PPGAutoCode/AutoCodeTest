
namespace ProjectName.Types
{
    public class UpdateChangeLogDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public Guid ProductId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ChangeLogVersion { get; set; }
        public int? Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public Guid CreatorId { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
