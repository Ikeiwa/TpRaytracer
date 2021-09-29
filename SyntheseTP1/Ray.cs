using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;
using SyntheseTP1.Shapes;

namespace SyntheseTP1
{
    struct Ray
    {
        public readonly Vector3 position;
        public readonly Vector3 direction;
        public readonly float length;

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
