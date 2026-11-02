using System;
using System.Windows;
using StudentRegistryApp.ViewModels;

namespace StudentRegistryApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel(LedDisplay);
            DataContext = viewModel;

            GlyphEditor.Glyph = viewModel.EditorGlyph;

            GlyphEditor.GlyphChanged += EditorGlyphChanged;
            viewModel.GlyphImported += ViewModelGlyphImported;
        }
        private void EditorGlyphChanged(object? sender, EventArgs e)
        {
            viewModel.UpdateGlyphHex();
        }
        private void ViewModelGlyphImported(object? sender, EventArgs e)
        {
            GlyphEditor.Refresh();
        }
    }
}