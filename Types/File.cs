
using System;

namespace ProjectName.Types
{
    public class File
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileUrl { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
    