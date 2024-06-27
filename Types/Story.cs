
// Story.cs
using System;
using System.Collections.Generic;

namespace ProjectName.Types
{
    public class Story
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public Guid? CompletedCondition { get; set; }
        public List<Step> Steps { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
    }
}
