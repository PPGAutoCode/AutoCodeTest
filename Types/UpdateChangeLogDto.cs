
namespace ProjectName.Types
{
    public class UpdateChangeLogDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public string ProductId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ChangeLogVersion { get; set; }
        public int? Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public string CreatorId { get; set; }
        public string ChangedUser { get; set; }
    }
}
