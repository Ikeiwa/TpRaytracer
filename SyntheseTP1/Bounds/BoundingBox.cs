using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;
using SyntheseTP1.Shapes;
using Triangle = SyntheseTP1.Shapes.Triangle;

namespace SyntheseTP1
{
    class BoundingBox
    {
        public Vector3 min;
        public Vector3 max;

        public BoundingBox(){}

        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public BoundingBox(List<Vector3> points)
        {
            min = Vector3.MaxValue;
            max = Vector3.MinValue;

            foreach (Vector3 point in points)
            {
                min = min.Min(point);
                max = max.Max(point);
            }
        }

        public BoundingBox(List<Triangle> tris)
        {
            min = Vector3.MaxValue;
            max = Vector3.MinValue;

            foreach (Triangle tri in tris)
            {
                min = min.Min(tri.A);
                min = min.Min(tri.B);
                min = min.Min(tri.C);

                max = max.Max(tri.A);
                max = max.Max(tri.B);
                max = max.Max(tri.C);
            }
        }

        public virtual float? Intersect(Ray ray)
        {
            const float Epsilon = 1e-6f;

            float? tMin = null, tMax = null;

            if (Math.Abs(ray.direction.X) < Epsilon)
            {
                if (ray.position.X < min.X || ray.position.X > max.X)
                    return null;
            }
            else
            {
                tMin = (min.X - ray.position.X) / ray.direction.X;
                tMax = (max.X - ray.position.X) / ray.direction.X;

                if (tMin > tMax)
                {
                    var temp = tMin;
                    tMin = tMax;
                    tMax = temp;
                }
            }

            if (Math.Abs(ray.direction.Y) < Epsilon)
            {
                if (ray.position.Y < min.Y || ray.position.Y > max.Y)
                    return null;
            }
            else
            {
                var tMinY = (min.Y - ray.position.Y) / ray.direction.Y;
                var tMaxY = (max.Y - ray.position.Y) / ray.direction.Y;

                if (tMinY > tMaxY)
                {
                    var temp = tMinY;
                    tMinY = tMaxY;
                    tMaxY = temp;
                }

                if ((tMin.HasValue && tMin > tMaxY) || (tMax.HasValue && tMinY > tMax))
                    return null;

                if (!tMin.HasValue || tMinY > tMin) tMin = tMinY;
                if (!tMax.HasValue || tMaxY < tMax) tMax = tMaxY;
            }

            if (Math.Abs(ray.direction.Z) < Epsilon)
            {
                if (ray.position.Z < min.Z || ray.position.Z > max.Z)
                    return null;
            }
            else
            {
                var tMinZ = (min.Z - ray.position.Z) / ray.direction.Z;
                var tMaxZ = (max.Z - ray.position.Z) / ray.direction.Z;

                if (tMinZ > tMaxZ)
                {
                    var temp = tMinZ;
                    tMinZ = tMaxZ;
                    tMaxZ = temp;
                }

                if ((tMin.HasValue && tMin > tMaxZ) || (tMax.HasValue && tMinZ > tMax))
                    return null;

                if (!tMin.HasValue || tMinZ > tMin) tMin = tMinZ;
                if (!tMax.HasValue || tMaxZ < tMax) tMax = tMaxZ;
            }

            // having a positive tMin and a negative tMax means the ray is inside the box
            // we expect the intesection distance to be 0 in that case
            if (tMin.HasValue && tMin < 0 && tMax > 0) return 0;

            // a negative tMin means that the intersection point is behind the ray's origin
            // we discard these as not hitting the AABB
            if (tMin < 0) return null;

            return tMin;
        }

        public static BoundingBox operator +(BoundingBox a,BoundingBox b)
        {
            a.min = a.min.Min(b.min);
            a.max = a.max.Max(b.max);

            return a;
        }
    }
}
