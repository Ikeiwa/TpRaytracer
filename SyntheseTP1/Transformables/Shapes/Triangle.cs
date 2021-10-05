using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1.Shapes
{
    class Triangle : Shape
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;

        public Triangle() { }

        public override float? Intersect(Ray ray)
        {
            var edge1 = B - A;
            var edge2 = C - A;

            var h = ray.direction.Cross(edge2);
            var a = edge1.Dot(h);
            if (a > -MathEx.Tolerance && a < MathEx.Tolerance)
                return null; // This ray is parallel to this triangle.

            var f = 1.0f / a;
            var s = ray.position - A;
            var u = f * s.Dot(h);
            if (u < 0.0 || u > 1.0)
                return null;

            var q = s.Cross(edge1);
            var v = f * ray.direction.Dot(q);
            if (v < 0.0 || u + v > 1.0)
                return null;

            // At this stage we can compute t to find out where the intersection point is on the line.
            var t = f * edge2.Dot(q);
            if (t > MathEx.Tolerance)
                return t;

            // This means that there is a line intersection but not a ray intersection.
            return null;
        }



        public override Hit Trace(Ray ray)
        {
            float? dist = Intersect(ray);

            if (dist.HasValue)
            {
                Hit hit = new Hit();
                hit.material = material;
                hit.distance = dist.Value;
                hit.truePosition = ray.GetEnd(dist.Value);
                hit.position = ray.GetEnd(dist.Value - MathEx.RayOffset);
                hit.normal = (B - A).Cross(C - A).Normalize();
                return hit;
            }
            return null;
        }
    }
}
