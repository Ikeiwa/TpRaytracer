using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntheseTP1.Shapes;
using Vim.Math3d;

namespace SyntheseTP1
{
    class Hit
    {
        public Vector3 position;
        public Vector3 truePosition;
        public Vector3 normal;
        public Vector2 uv = Vector2.Zero;
        public float distance;
        public Material material;
    }
}
