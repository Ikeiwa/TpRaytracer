using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1
{
    class Texture
    {
        public enum FilterType
        {
            Nearest,
            Billinear
        }

        public enum WrapMode
        {
            Clamp,
            Repeat
        }

        private HDRColor[,] pixels;
        public Vector2 size { get; private set; }
        public FilterType filter = FilterType.Billinear;
        public WrapMode wrap = WrapMode.Repeat;

        public Texture(string filename)
        {
            Bitmap srcImage = new Bitmap(filename);
            size = new Vector2(srcImage.Width, srcImage.Height);

            pixels = new HDRColor[srcImage.Width, srcImage.Height];

            for (int x = 0; x < srcImage.Width; x++)
            {
                for (int y = 0; y < srcImage.Width; y++)
                {
                    pixels[x, y] = new HDRColor(srcImage.GetPixel(x, y));
                }
            }
        }

        private static HDRColor Bilinear(float tx, float ty, HDRColor c00, HDRColor c10, HDRColor c01, HDRColor c11)
        {
            return (1 - tx) * (1 - ty) * c00 + 
                   tx * (1 - ty) * c10 + 
                   (1 - tx) * ty * c01 + 
                   tx * ty * c11; 
        }

        public HDRColor Sample(Vector2 coords)
        {
            return Sample(coords.X, coords.Y);
        }

        public HDRColor Sample(float x, float y)
        {
            switch (wrap)
            {
                case WrapMode.Clamp:
                    break;
                case WrapMode.Repeat:
                    x = x - (float)Math.Floor(x);
                    y = y - (float)Math.Floor(y);
                    break;
            }

            x = x.Clamp(0, 1);
            y = y.Clamp(0, 1);

            Vector2 pixelPos = new Vector2(x * size.X, y * size.Y);
            

            switch (filter)
            {
                case FilterType.Nearest:
                    return pixels[(int)Math.Round(pixelPos.X), (int)Math.Round(pixelPos.Y)];
                case FilterType.Billinear:
                    int px = (int) pixelPos.X;
                    int py = (int) pixelPos.Y;

                    int px1 = Math.Min(px+1, (int) size.X);
                    int py1 = Math.Min(py+1, (int) size.Y);

                    HDRColor c00 = pixels[px, py];
                    HDRColor c10 = pixels[px1, py];
                    HDRColor c01 = pixels[px, py1];
                    HDRColor c11 = pixels[px1, py1];

                    return Bilinear(pixelPos.X - px, pixelPos.Y - py, c00, c10, c01, c11);
                default: return new HDRColor(0, 0, 0);
            }
        }
    }
}
