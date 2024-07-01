
namespace ProjectName.Types
{
    public class Image
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }
        public string AltText { get; set; }
    }
}
