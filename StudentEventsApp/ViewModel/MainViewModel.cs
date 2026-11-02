using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Data;
using System.Windows.Input;
using StudentRegistryApp.Models;
using StudentRegistryApp.Repositories;

namespace StudentRegistryApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Repository<Student> repository = new Repository<Student>();

        private readonly StudentGroup group = new StudentGroup
        {
            Name = "Навчальна група"
        };
        private Student? selectedStudent;

        private string studentId = "";
        private string studentName = "";
        private string studentAge = "";
        private string studentMass = "";
        private string studentMarks = "";

        private string searchText = "";
        private string minimumMark = "";
        private string selectedSort = "Ім'я";
        private string message = "Програма готова до роботи.";

        private const string FileName = "students.json";
        public ObservableCollection<Student> Students
        {
            get { return group.Students; }
        }
        public ICollectionView StudentsView { get; }
        public ObservableCollection<string> SortOptions { get; } = new ObservableCollection<string>
            {
                "Ім'я",
                "Вік",
                "Маса",
                "Середній бал"
            };
        public Student? SelectedStudent
        {
            get { return selectedStudent; }
            set
            {
                selectedStudent = value;
                OnPropertyChanged();

                if (value != null)
                {
                    StudentId = value.Id.ToString();
                    StudentName = value.Name;
                    StudentAge = value.Age.ToString();
                    StudentMass = value.Mass.ToString();
                    StudentMarks = string.Join(", ", value.Marks);
                }
            }
        }
        public string StudentId
        {
            get { return studentId; }
            set
            {
                studentId = value;
                OnPropertyChanged();
            }
        }
        public string StudentName
        {
            get { return studentName; }
            set
            {
                studentName = value;
                OnPropertyChanged();
            }
        }
        public string StudentAge
        {
            get { return studentAge; }
            set
            {
                studentAge = value;
                OnPropertyChanged();
            }
        }
        public string StudentMass
        {
            get { return studentMass; }
            set
            {
                studentMass = value;
                OnPropertyChanged();
            }
        }
        public string StudentMarks
        {
            get { return studentMarks; }
            set
            {
                studentMarks = value;
                OnPropertyChanged();
            }
        }
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged();
                StudentsView.Refresh();
            }
        }
        public string MinimumMark
        {
            get { return minimumMark; }
            set
            {
                minimumMark = value;
                OnPropertyChanged();
                StudentsView.Refresh();
            }
        }
        public string SelectedSort
        {
            get { return selectedSort; }
            set
            {
                selectedSort = value;
                OnPropertyChanged();
                SortStudents();
            }
        }
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public MainViewModel()
        {
            StudentsView = CollectionViewSource.GetDefaultView(Students);

            StudentsView.Filter = FilterStudent;

            AddCommand = new RelayCommand(p => AddStudent());
            UpdateCommand = new RelayCommand(p => UpdateStudent());
            RemoveCommand = new RelayCommand(p => RemoveStudent());
            ClearCommand = new RelayCommand(p => ClearFields());
            SaveCommand = new RelayCommand(p => SaveStudents());
            LoadCommand = new RelayCommand(p => LoadStudents());

            SortStudents();
        }
        private bool ReadStudent(out Student student)
        {
            student = new Student();

            if (!int.TryParse(StudentId, out int id) || id <= 0)
            {
                Message = "Введіть правильний ID.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(StudentName))
            {
                Message = "Введіть ім'я студента.";
                return false;
            }

            if (!int.TryParse(StudentAge, out int age) || age <= 0)
            {
                Message = "Введіть правильний вік.";
                return false;
            }

            string massText = StudentMass.Replace(',', '.');

            if (!double.TryParse(massText, NumberStyles.Float, CultureInfo.InvariantCulture, out double mass) || mass <= 0)
            {
                Message = "Введіть правильну масу.";
                return false;
            }

            List<int> marks = new List<int>();

            string[] parts = StudentMarks.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                if (!int.TryParse(part.Trim(), out int mark) || mark < 0 || mark > 100)
                {
                    Message = "Оцінки повинні бути від 0 до 100.";
                    return false;
                }

                marks.Add(mark);
            }

            student = new Student
            {
                Id = id,
                Name = StudentName.Trim(),
                Age = age,
                Mass = mass,
                Marks = marks
            };
            return true;
        }
        private void AddStudent()
        {
            if (!ReadStudent(out Student student))
            {
                return;
            }

            if (repository.Find(s => s.Id == student.Id) != null)
            {
                Message = "Студент із таким ID вже існує.";
                return;
            }

            repository.Add(student);
            Students.Add(student);

            StudentsView.Refresh();

            Message = "Студента успішно додано.";
            ClearFields(false);
        }
        private void UpdateStudent()
        {
            if (SelectedStudent == null)
            {
                Message = "Оберіть студента для редагування.";
                return;
            }

            if (!ReadStudent(out Student student))
            {
                return;
            }

            Student oldStudent = SelectedStudent;

            if (repository.Find(s => s.Id == student.Id && s != oldStudent) != null)
            {
                Message = "Такий ID вже використовується.";
                return;
            }

            int index = Students.IndexOf(oldStudent);
            repository.Update(oldStudent, student);
            Students[index] = student;

            StudentsView.Refresh();

            Message = "Дані студента оновлено.";
            ClearFields(false);
        }
        private void RemoveStudent()
        {
            if (SelectedStudent == null)
            {
                Message = "Оберіть студента для видалення.";
                return;
            }

            Student student = SelectedStudent;

            repository.Remove(student);
            Students.Remove(student);

            Message = "Студента видалено.";
            ClearFields(false);
        }
        private void ClearFields(bool changeMessage = true)
        {
            SelectedStudent = null;

            StudentId = "";
            StudentName = "";
            StudentAge = "";
            StudentMass = "";
            StudentMarks = "";

            if (changeMessage)
            {
                Message = "Поля очищено.";
            }
        }
        private bool FilterStudent(object item)
        {
            if (item is not Student student)
            {
                return false;
            }

            bool matchesName = student.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

            if (!matchesName)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(MinimumMark))
            {
                return true;
            }

            string text = MinimumMark.Replace(',', '.');

            if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double minimum))
            {
                return true;
            }

            return student.AverageMark >= minimum;
        }
        private void SortStudents()
        {
            if (StudentsView == null)
            {
                return;
            }

            StudentsView.SortDescriptions.Clear();

            string property = SelectedSort switch
            {
                "Вік" => nameof(Student.Age),
                "Маса" => nameof(Student.Mass),
                "Середній бал" => nameof(Student.AverageMark),
                _ => nameof(Student.Name)
            };

            StudentsView.SortDescriptions.Add(new SortDescription(property, ListSortDirection.Ascending));
            StudentsView.Refresh();
        }

        private void SaveStudents()
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                string json = JsonSerializer.Serialize(Students.ToList(), options);

                File.WriteAllText(FileName, json);

                Message = "Дані успішно збережено у JSON.";
            }
            catch (Exception ex)
            {
                Message = "Помилка збереження: " + ex.Message;
            }
        }
        private void LoadStudents()
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    Message = "Файл students.json не знайдено.";
                    return;
                }

                string json = File.ReadAllText(FileName);

                if (string.IsNullOrWhiteSpace(json))
                {
                    Message = "Файл порожній.";
                    return;
                }

                List<Student>? loaded = JsonSerializer.Deserialize<List<Student>>(json);

                if (loaded == null)
                {
                    Message = "Не вдалося прочитати дані.";
                    return;
                }

                repository.GetAll().ToList().ForEach(s => repository.Remove(s));
                Students.Clear();

                foreach (Student student in loaded)
                {
                    if (student == null || student.Id <= 0 || string.IsNullOrWhiteSpace(student.Name) || student.Age <= 0 || student.Mass <= 0 || student.Marks == null || student.Marks.Any(m => m < 0 || m > 100) || repository.Find(s => s.Id == student.Id) != null)
                    {
                        throw new InvalidDataException(
                            "Файл містить некоректні дані студента.");
                    }

                    repository.Add(student);
                    Students.Add(student);
                }

                StudentsView.Refresh();

                Message = "Дані успішно завантажено.";
            }
            catch (Exception ex)
            {
                Message = "Помилка завантаження: " + ex.Message;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}