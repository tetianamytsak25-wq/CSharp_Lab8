using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using StudentRegistryApp.Controls;
using StudentRegistryApp.Models;
using StudentRegistryApp.Services;

namespace StudentRegistryApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly LedTextScroller scroller;
        private string displayText = "HELLO WORLD!";
        private int speed = 100;
        private string hexInput = "00 00 00 00 00 00 00 00 00 00";
        public Glyph8x10 EditorGlyph { get; } = new Glyph8x10();
        public string DisplayText
        {
            get { return displayText; }
            set
            {
                displayText = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                scroller.Speed = value;
                OnPropertyChanged(nameof(Speed));
            }
        }
        public string HexInput
        {
            get { return hexInput; }
            set
            {
                hexInput = value;
                OnPropertyChanged(nameof(HexInput));
            }
        }
        public string HexBytes
        {
            get
            {
                return string.Join(" ", EditorGlyph.GetBytes().Select(value => value.ToString("X2")));
            }
        }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ImportHexCommand { get; }
        public ICommand ClearGlyphCommand { get; }
        public MainViewModel(LedMatrixControl display)
        {
            scroller = new LedTextScroller(display);

            StartCommand = new RelayCommand(StartScrolling);
            StopCommand = new RelayCommand(StopScrolling);
            ImportHexCommand = new RelayCommand(ImportHex);
            ClearGlyphCommand = new RelayCommand(ClearGlyph);
        }
        public void UpdateGlyphHex()
        {
            HexInput = HexBytes;
            OnPropertyChanged(nameof(HexBytes));
        }
        private void StartScrolling(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(DisplayText))
            {
                MessageBox.Show("Введіть текст для відображення.");
                return;
            }
            scroller.Start(DisplayText);
        }
        private void StopScrolling(object? parameter)
        {
            scroller.Stop();
        }
        private void ClearGlyph(object? parameter)
        {
            EditorGlyph.SetBytes(new byte[10]);
            UpdateGlyphHex();
        }
        private void ImportHex(object? parameter)
        {
            try
            {
                string[] parts = HexInput.Split(new[] { ' ', '\t', '\r', '\n', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 10)
                {
                    MessageBox.Show("Потрібно ввести рівно 10 HEX-байтів.");
                    return;
                }

                byte[] values = parts.Select(part => byte.Parse(part, NumberStyles.HexNumber, CultureInfo.InvariantCulture)).ToArray();

                EditorGlyph.SetBytes(values);
                OnPropertyChanged(nameof(HexBytes));
                HexInput = HexBytes;
                GlyphImported?.Invoke(this, EventArgs.Empty);
            }
            catch (FormatException)
            {
                MessageBox.Show("HEX-код має містити байти від 00 до FF.");
            }
            catch (OverflowException)
            {
                MessageBox.Show("Кожен HEX-байт має бути в межах 00–FF.");
            }
        }
        public event EventHandler? GlyphImported;
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}