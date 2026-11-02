using System;
using System.Windows;
using System.Windows.Media;
using StudentRegistryApp.Models;

namespace StudentRegistryApp.Controls
{
    public class LedMatrixControl : FrameworkElement
    {
        private const int Columns = 64;
        private const int Rows = 10;
        private const int PixelSize = 9;
        private readonly bool[,] pixels = new bool[Rows, Columns];
        public void Clear()
        {
            Array.Clear(pixels, 0, pixels.Length);
            InvalidateVisual();
        }
        public void SetPixel(int x, int y, bool isOn)
        {
            if (x < 0 || x >= Columns || y < 0 || y >= Rows)
            {
                throw new ArgumentOutOfRangeException("Координати виходять за межі LED-екрана.");
            }

            pixels[y, x] = isOn;
            InvalidateVisual();
        }
        public void ShowGlyph(Glyph8x10 glyph, int startX)
        {
            for (int y = 0; y < glyph.Height; y++)
            {
                for (int x = 0; x < glyph.Width; x++)
                {
                    int screenX = startX + x;

                    if (screenX >= 0 && screenX < Columns)
                    {
                        pixels[y, screenX] = glyph.GetPixel(x, y);
                    }
                }
            }
            InvalidateVisual();
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(Columns * PixelSize, Rows * PixelSize);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(25, 31, 42)), null, new Rect(0, 0, Columns * PixelSize, Rows * PixelSize));

            Brush onBrush = new SolidColorBrush(Color.FromRgb(77, 220, 130));
            Brush offBrush = new SolidColorBrush(Color.FromRgb(48, 58, 68));

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    Brush brush = pixels[y, x] ? onBrush : offBrush;
                    drawingContext.DrawRectangle(brush, null, new Rect(x * PixelSize + 1, y * PixelSize + 1, PixelSize - 2, PixelSize - 2));
                }
            }
        }
    }
}