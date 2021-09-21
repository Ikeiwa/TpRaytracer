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
        public float fov = 45;

        public Ray PixelToRay(Vector2 pixel,Vector2 res)
        {
            Vector2 pos = new Vector2(pixel.X + 0.5f, pixel.Y + 0.5f);
            pos = pos / res;
            pos = 2 * pos - 1;
            pos *= (float)Math.Tan(fov / 2.0 * Math.PI / 180.0);
            pos = pos.SetX(pos.X * (res.X/res.Y));

            Vector3 dir = new Vector3(pos.X, pos.Y, 1);

            Matrix4x4 transformMat = GetTransformMatrix();

            Vector3 start = Vector3.Zero.Transform(rotation);
            dir = dir.Transform(rotation);
            dir = dir - start;

            return new Ray(start, dir.Normalize());
        }
    }
}
