using System.Windows;
using StudentRegistryApp.ViewModels;

namespace StudentRegistryApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}