using System.Collections.Generic;

namespace TodoApi.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsComplete { get; set; }

        public ICollection<Story> Stories { get; set; }
    }
}