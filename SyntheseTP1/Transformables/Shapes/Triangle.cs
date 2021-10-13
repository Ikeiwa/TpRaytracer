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

        public Vector3 nA;
        public Vector3 nB;
        public Vector3 nC;

        public Vector2 uA;
        public Vector2 uB;
        public Vector2 uC;

        public Vector3 center { get; private set; }

        public Triangle(Vector3 A,Vector3 B,Vector3 C)
        {
            this.A = A;
            this.B = B;
            this.C = C;
            center = (A + B + C) / 3;
        }

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

        private Vector3 CalculateBarycenter(Vector3 pos)
        {
            Vector3 U = B - A;
            Vector3 V = C - A;
            Vector3 W = pos - A;

            float vCrossW = V.Cross(W).Length();
            float uCrossW = U.Cross(W).Length();
            float uCrossV = U.Cross(V).Length();

            float b1 = vCrossW / uCrossV;
            float b2 = uCrossW / uCrossV;
            float b0 = (1 - b1) - b2;

            return new Vector3(b0, b1, b2);
        }


        public override Hit Trace(Ray ray)
        {
            float? dist = Intersect(ray);

            if (dist.HasValue)
            {
                Vector3 pos = ray.GetEnd(dist.Value);
                Vector3 bary = CalculateBarycenter(pos);

                Vector3 nbA = nA * bary.X;
                Vector3 nbB = nB * bary.Y;
                Vector3 nbC = nC * bary.Z;
                Vector3 normal = new Vector3(
                    nbA.X + nbB.X + nbC.X, 
                    nbA.Y + nbB.Y + nbC.Y,
                    nbA.Z + nbB.Z + nbC.Z
                );

                Vector2 ubA = uA * bary.X;
                Vector2 ubB = uB * bary.Y;
                Vector2 ubC = uC * bary.Z;
                Vector2 uv = new Vector2(
                    ubA.X + ubB.X + ubC.X,
                    ubA.Y + ubB.Y + ubC.Y
                );

                Hit hit = new Hit
                {
                    material = material,
                    distance = dist.Value,
                    truePosition = pos,
                    position = ray.GetEnd(dist.Value - MathEx.RayOffset),
                    //hit.normal = (B - A).Cross(C - A).Normalize();
                    normal = normal,
                    uv = uv
                };
                return hit;
            }
            return null;
        }
    }
}
