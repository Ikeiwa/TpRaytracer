using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1
{
    static class MathEx
    {
        public const float RayOffset = 0.0001f;

        public const double DegToRad = Math.PI / 180.0;
        public static float Lerp(float A,float B, float t) {return A * (1 - t) + B * t;}
        public static double Lerp(double A, double B, float t) {return A * (1 - t) + B * t;}


    }
}
