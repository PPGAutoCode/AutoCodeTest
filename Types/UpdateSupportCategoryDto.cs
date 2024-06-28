
// UpdateSupportCategoryDto.cs
using System;

namespace ProjectName.Types
{
    public class UpdateSupportCategoryDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
