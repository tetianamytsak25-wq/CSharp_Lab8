using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace StudentRegistryApp.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public double Mass { get; set; }
        public List<int> Marks { get; set; } = new List<int>();

        [JsonIgnore]
        public string MarksText
        {
            get
            {
                return Marks == null ? "" : string.Join(", ", Marks);
            }
        }
        [JsonIgnore]
        public double AverageMark
        {
            get
            {
                if (Marks == null || Marks.Count == 0)
                {
                    return 0;
                }
                return Marks.Average();
            }
        }
    }
}