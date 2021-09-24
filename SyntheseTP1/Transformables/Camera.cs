using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntheseTP1.Transformables;
using Vim.Math3d;

namespace SyntheseTP1
{
    class Camera : Transformable
    {
        public enum CameraType
        {
            Perspective,
            Lens
        }

        public float fov = 45;
        public float width = (float)Math.PI;
        public CameraType type = CameraType.Perspective;

        public Ray PixelToRay(Vector2 pixel,Vector2 res)
        {
            Vector2 pos = new Vector2(pixel.X + 0.5f, pixel.Y + 0.5f);
            pos = pos / res;
            pos = 2 * pos - 1;

            Vector3 start = Vector3.Zero;

            switch (type)
            {
                case CameraType.Perspective:
                    pos *= (float)Math.Tan(fov / 2.0 * Math.PI / 180.0);
                    pos = pos.SetX(pos.X * (res.X / res.Y));

                    Vector3 dir = new Vector3(pos.X, pos.Y, 1);

                    start = position;
                    dir = dir.Transform(rotation);

                    return new Ray(start, dir.Normalize());
                case CameraType.Lens:
                    dir = SphereToCartesian(-pos.X*width+(float)Math.PI/2, pos.Y);
                    dir = dir.Transform(rotation);

                    return new Ray(position, dir.Normalize());
            }
            return new Ray(position,Vector3.UnitZ);
        }

        public Vector3 SphereToCartesian(float polar, float elevation)
        {
            float a = (float)Math.Cos(elevation);
            return new Vector3(a * (float)Math.Cos(polar), (float)Math.Sin(elevation), a * (float)Math.Sin(polar));
        }
    }
}
