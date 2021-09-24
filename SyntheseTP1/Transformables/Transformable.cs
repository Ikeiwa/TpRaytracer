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

        public void Translate(float X,float Y,float Z,bool relative = true)
        {
            if(relative)
                position += new Vector3(X, Y, Z).Transform(rotation);
            else
                position += new Vector3(X, Y, Z);
        }

        public void Rotate(float X,float Y,float Z)
        {
            rotation *= Quaternion.CreateFromEulerAnglesDeg(X, Y, Z);
        }
    }
}
