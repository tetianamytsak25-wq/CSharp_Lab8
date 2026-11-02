using System.Collections.ObjectModel;

namespace StudentRegistryApp.Models
{
    public class StudentGroup
    {
        public string Name { get; set; } = "";
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();
    }
}