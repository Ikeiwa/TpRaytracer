using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public static float NextFloat(Random random)
        {
            return (float)(float.MaxValue * 2.0 * (StaticRandom.NextDouble() - 0.5));
        }

        public static float NextFloat(Random random, float min, float max)
        {
            double val = (StaticRandom.NextDouble() * (max - min) + min);
            return (float)val;
        }
    }

    public static class StaticRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int Next()
        {
            return random.Value.Next();
        }

        public static double NextDouble()
        {
            return random.Value.NextDouble();
        }
    }
}
