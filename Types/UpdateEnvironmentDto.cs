
// UpdateEnvironmentDto.cs
using System;

namespace ProjectName.Types
{
    public class UpdateEnvironmentDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
