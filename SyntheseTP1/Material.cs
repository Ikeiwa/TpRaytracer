using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1
{
    class HDRColor
    {
        public double R = 0;
        public double G = 0;
        public double B = 0;

        public HDRColor(double R,double G,double B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public override string ToString()
        {
            return "R:" + R + " G:" + G + " B:" + B;
        }

        public Color ToColor()
        {

            return Color.FromArgb(255,
                                  (int)MathOps.Clamp(Math.Pow(R, 1 / 2.2) * 255, 0, 255),
                                  (int)MathOps.Clamp(Math.Pow(G, 1 / 2.2) * 255, 0, 255),
                                  (int)MathOps.Clamp(Math.Pow(B, 1 / 2.2) * 255, 0, 255));
        }

        public static HDRColor GetAverage(List<HDRColor> colors)
        {
            HDRColor result = new HDRColor(0,0,0);
            foreach(HDRColor color in colors)
            {
                result += color;
            }
            return result / colors.Count;
        }

        public override bool Equals(object obj) => Equals(obj as HDRColor);

        public bool Equal(HDRColor other)
        {
            return R == other.R && G == other.G && B == other.B;
        }

        public static HDRColor operator *(HDRColor color, float factor)
        {
            return new HDRColor(color.R * factor, color.G * factor, color.B * factor);
        }

        public static HDRColor operator *(HDRColor colorA, HDRColor colorB)
        {
            return new HDRColor(colorA.R * colorB.R, colorA.G * colorB.G, colorA.B * colorB.B);
        }

        public static HDRColor operator +(HDRColor colorA, float val)
        {
            return new HDRColor(colorA.R + val, colorA.G + val, colorA.B + val);
        }

        public static HDRColor operator +(HDRColor colorA, HDRColor colorB)
        {
            return new HDRColor(colorA.R + colorB.R, colorA.G + colorB.G, colorA.B + colorB.B);
        }

        public static HDRColor operator /(HDRColor color, float factor)
        {
            return new HDRColor(color.R / factor, color.G / factor, color.B / factor);
        }

        public static HDRColor Lerp(HDRColor A,HDRColor B, float t)
        {
            return new HDRColor(
                MathEx.Lerp(A.R,B.R,t),
                MathEx.Lerp(A.G,B.G,t),
                MathEx.Lerp(A.B,B.B,t)
            );
        }
    }

    public enum MaterialType
    {
        Diffuse,
        Mirror,
        Glass
    }

    class Material
    {
        public HDRColor color;
        public float metallic = 0;
        public float roughness = 1;
        public MaterialType type = MaterialType.Diffuse;
        public float IOR = 1;
    }
}
