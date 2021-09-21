using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;
using SyntheseTP1.Transformables;

namespace SyntheseTP1.Shapes
{
    abstract class Shape : Transformable
    {
        public Material material;

        public abstract float? Intersect(Ray ray);

        public abstract Hit Trace(Ray ray);
    }
}
