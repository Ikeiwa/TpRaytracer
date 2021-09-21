using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1.Transformables
{
    abstract class Transformable
    {
        public Vector3 position;
        public Quaternion rotation = Quaternion.Identity;
        public Vector3 scale = Vector3.One;

        public Matrix4x4 GetTransformMatrix()
        {
            return Matrix4x4.CreateTRS(position, rotation, scale);
        }
    }
}
