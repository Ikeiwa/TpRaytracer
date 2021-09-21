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
        public double A = 1;

        public HDRColor(double R,double G,double B,double A=1)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public Color ToColor()
        {
            return Color.FromArgb((int)MathOps.Clamp(A * 255, 0, 255),
                                  (int)MathOps.Clamp(R * 255, 0, 255),
                                  (int)MathOps.Clamp(G * 255, 0, 255),
                                  (int)MathOps.Clamp(B * 255, 0, 255));
        }

        public static HDRColor operator *(HDRColor color, float factor)
        {
            return new HDRColor(color.R * factor, color.G * factor, color.B * factor, color.A);
        }

        public static HDRColor Lerp(HDRColor A,HDRColor B, float t)
        {
            return new HDRColor(
                MathEx.Lerp(A.R,B.R,t),
                MathEx.Lerp(A.R,B.R,t),
                MathEx.Lerp(A.R,B.R,t),
                MathEx.Lerp(A.R,B.R,t)
            );
        }
    }

    class Material
    {
        public HDRColor color;
        public float metallic = 0;
        public float roughness = 1;
    }
}
