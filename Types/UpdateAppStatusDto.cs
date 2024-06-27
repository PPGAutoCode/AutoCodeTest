
using System;

namespace ProjectName.Types
{
    public class UpdateAppStatusDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid ChangedUser { get; set; }
    }
}
