using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1.Shapes
{
    class Sphere : Shape
    {
        public float radius = 1;

        public Sphere() { }

        public Sphere(float radius)
        {
            this.radius = radius;
        }

        public override float? Intersect(Ray ray)
        {
            double A = ray.direction.MagnitudeSquared();
            double B = 2 * Vector3.Dot(ray.position - position, ray.direction);
            double C = (ray.position - position).MagnitudeSquared() - (radius * radius);

            double delta = (B * B) - (4 * A * C);

            if (delta < 0) return null;

            if (delta == 0) return (float?)(-B / (2 * A));

            double Dg = (-B + Math.Sqrt(delta)) / (2 * A);
            double Dl = (-B - Math.Sqrt(delta)) / (2 * A);

            if (Dl > 0)
                return (float)Dl;
            else if (Dg > 0)
                return (float)Dg;

            return null;
        }

        public override Hit Trace(Ray ray)
        {
            float? dist = Intersect(ray);

            if (dist.HasValue)
            {
                Hit hit = new Hit();
                hit.shape = this;
                hit.distance = dist.Value;
                hit.position = ray.GetEnd(dist.Value - MathEx.RayOffset);
                hit.normal = (hit.position - position).Normalize();
                return hit;
            }
            return null;
        }
    }
}
