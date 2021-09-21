using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;
using SyntheseTP1.Shapes;

namespace SyntheseTP1
{
    class Ray
    {
        public Vector3 position;
        public Vector3 direction;
        public float length = 1;

        public Ray(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction.Normalize();
            length = direction.Length();
        }

        public Vector3 GetEnd()
        {
            return GetEnd(length);
        }

        public Vector3 GetEnd(float distance)
        {
            return position + direction * distance;
        }
    }
}
