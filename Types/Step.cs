
// Step.cs
using System;

namespace ProjectName.Types
{
    public class Step
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Number { get; set; }
        public string TargetId { get; set; }
        public string NavLink { get; set; }
        public string Styles { get; set; }
        public bool? SpotlightClicks { get; set; }
    }
}
