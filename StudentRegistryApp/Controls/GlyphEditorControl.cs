using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using StudentRegistryApp.Models;

namespace StudentRegistryApp.Controls
{
    public class GlyphEditorControl : FrameworkElement
    {
        private const int PixelSize = 14;
        public Glyph8x10 Glyph { get; set; } = new Glyph8x10();
        public event EventHandler? GlyphChanged;
        public GlyphEditorControl()
        {
            Width = 8 * PixelSize;
            Height = 10 * PixelSize;
            Cursor = Cursors.Hand;
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(8 * PixelSize, 10 * PixelSize);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(25, 32, 43)), null, new Rect(0, 0, 8 * PixelSize, 10 * PixelSize));

            Brush onBrush = new SolidColorBrush(Color.FromRgb(56, 184, 120));
            Brush offBrush = new SolidColorBrush(Color.FromRgb(65, 77, 90));

            Pen gridPen = new Pen(new SolidColorBrush(Color.FromRgb(35, 45, 57)), 1);

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Brush brush = Glyph.GetPixel(x, y) ? onBrush : offBrush;

                    Rect pixelRect = new Rect(x * PixelSize + 2, y * PixelSize + 2, PixelSize - 4, PixelSize - 4);
                    drawingContext.DrawRectangle(brush, gridPen, pixelRect);
                }
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Point position = e.GetPosition(this);

            int x = (int)(position.X / PixelSize);
            int y = (int)(position.Y / PixelSize);

            if (x >= 0 && x < 8 && y >= 0 && y < 10)
            {
                Glyph.TogglePixel(x, y);
                InvalidateVisual();
                GlyphChanged?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
            }
        }
        public void Refresh()
        {
            InvalidateVisual();
        }
    }
}