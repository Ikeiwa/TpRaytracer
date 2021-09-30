using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1.Shapes
{
    class Plane : Shape
    {

        public Plane() { }

        public override float? Intersect(Ray ray)
        {
            Vector3 normal = -Vector3.UnitY.Transform(rotation).Normalize();

            float denom = Vector3.Dot(normal, ray.direction);

            if (denom > 0)
            {
                float t = Vector3.Dot(position - ray.position,normal) / denom;

                if (t >= 0)
                    return t;
            }

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
                hit.normal = Vector3.UnitY.Transform(rotation).Normalize();
                return hit;
            }
            return null;
        }
    }
}
