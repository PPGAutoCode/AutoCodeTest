
// File: UpdateFileDto.cs
namespace ProjectName.Types
{
    public class UpdateFileDto
    {
        public Guid? Id { get; set; }
        public string? FileName { get; set; }
        public byte[] FileUrl { get; set; }
    }
}
