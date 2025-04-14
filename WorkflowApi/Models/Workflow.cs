using System;
using System.Collections.Generic;

namespace WorkflowApi.Models
{
    public class Workflow
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 