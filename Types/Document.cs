
using System;

namespace ProjectName.Types
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string Helptext { get; set; }
        public bool RequiredField { get; set; }
        public string AllowedFileExtensions { get; set; }
        public string FileDirectory { get; set; }
        public int? MaxUploadSize { get; set; }
        public bool EnableDescriptionField { get; set; }
    }
}
    