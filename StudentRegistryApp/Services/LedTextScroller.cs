using System;
using System.Windows.Threading;
using StudentRegistryApp.Controls;
using StudentRegistryApp.Models;

namespace StudentRegistryApp.Services
{
    public class LedTextScroller
    {
        private readonly DispatcherTimer timer;
        private readonly LedMatrixControl display;
        private readonly GlyphFont font = new GlyphFont();
        private string text = "";
        private int offset;
        public int Speed
        {
            get { return (int)timer.Interval.TotalMilliseconds; }
            set
            {
                if (value < 20)
                {
                    value = 20;
                }
                timer.Interval = TimeSpan.FromMilliseconds(value);
            }
        }
        public bool IsRunning
        {
            get { return timer.IsEnabled; }
        }
        public LedTextScroller(LedMatrixControl display)
        {
            this.display = display;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += MoveText;
        }
        public void Start(string newText)
        {
            text = newText.ToUpperInvariant();
            offset = 64;
            timer.Start();
        }
        public void Stop()
        {
            timer.Stop();
            display.Clear();
        }
        private void MoveText(object? sender, EventArgs e)
        {
            display.Clear();
            int x = offset;

            foreach (char symbol in text)
            {
                Glyph8x10 glyph = font.GetGlyph(symbol);
                display.ShowGlyph(glyph, x);
                x += 8;
            }

            offset--;

            if (x < 0)
            {
                offset = 64;
            }
        }
    }
}