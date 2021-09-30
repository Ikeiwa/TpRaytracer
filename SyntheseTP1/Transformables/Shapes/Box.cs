using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1.Shapes
{
    class Box : Shape
    {
        public float radius = 1;

        public Box() { }

        public override float? Intersect(Ray ray)
        {
            return Intersect(ray,out Vector3 normal);
        }

        public float? Intersect(Ray ray, out Vector3 normal)
        {
            /*Vector3 ro = ray.position.Transform(GetTransformMatrix().Inverse());
            Vector3 rd = ray.direction.Transform(rotation.Inverse());*/

            Vector3 ro = ray.position - position;
            Vector3 rd = ray.direction;

            Vector3 m = Vector3.OneOn(rd); // can precompute if traversing a set of aligned boxes
            Vector3 n = m * ro;   // can precompute if traversing a set of aligned boxes
            Vector3 k = Vector3.Abs(m) * scale;

            Vector3 t1 = -n - k;
            Vector3 t2 = -n + k;
            float tN = Math.Max(Math.Max(t1.X, t1.Y), t1.Z);
            float tF = Math.Min(Math.Min(t2.X, t2.Y), t2.Z);
            normal = Vector3.UnitZ;
            if (tN > tF || tF < 0.0) return null; // no intersection

            normal = -Vector3.Sign(rd) * Vector3.Step(t1.YZX, t1) * Vector3.Step(t1.ZXY, t1);

            if (tN > 0)
                return tN;

            return tF;
        }

        public override Hit Trace(Ray ray)
        {
            float? dist = Intersect(ray, out Vector3 normal);

            if (dist.HasValue)
            {
                Hit hit = new Hit();
                hit.material = material;
                hit.distance = dist.Value;
                hit.truePosition = ray.GetEnd(dist.Value);
                hit.position = ray.GetEnd(dist.Value - MathEx.RayOffset);
                hit.normal = normal.Normalize();
                return hit;
            }
            return null;
        }
    }
}
