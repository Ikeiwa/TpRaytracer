using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntheseTP1
{
    abstract class BoundTreeNodeBase
    {
        public abstract Hit Intersect(Ray ray);
    }
}
