using System;

namespace StudentRegistryApp.Models
{
    public class Glyph8x10
    {
        private readonly byte[] rows = new byte[10];
        public int Width
        {
            get { return 8; }
        }
        public int Height
        {
            get { return 10; }
        }
        public bool GetPixel(int x, int y)
        {
            CheckCoordinates(x, y);
            int bitIndex = 7 - x;
            int mask = 1 << bitIndex;

            return (rows[y] & mask) != 0;
        }
        public void SetPixel(int x, int y, bool isOn)
        {
            CheckCoordinates(x, y);
            int bitIndex = 7 - x;
            int mask = 1 << bitIndex;

            if (isOn)
            {
                rows[y] = (byte)(rows[y] | mask);
            }
            else
            {
                rows[y] = (byte)(rows[y] & ~mask);
            }
        }
        public void TogglePixel(int x, int y)
        {
            CheckCoordinates(x, y);
            int bitIndex = 7 - x;
            int mask = 1 << bitIndex;

            rows[y] = (byte)(rows[y] ^ mask);
        }
        public byte[] GetBytes()
        {
            return (byte[])rows.Clone();
        }
        public void SetBytes(byte[] newRows)
        {
            if (newRows == null || newRows.Length != Height)
            {
                throw new ArgumentException("Символ повинен містити рівно 10 байтів.");
            }
            Array.Copy(newRows, rows, Height);
        }
        private void CheckCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException("Координати пікселя виходять за межі символу.");
            }
        }
    }
}