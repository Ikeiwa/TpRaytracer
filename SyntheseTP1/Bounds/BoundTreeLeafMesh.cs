using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntheseTP1.Shapes;

namespace SyntheseTP1
{
    class BoundTreeLeafMesh : BoundTreeNodeBase
    {
        public List<Triangle> triangles;

        public BoundTreeLeafMesh(List<Triangle> triangles)
        {
            this.triangles = triangles;
        }

        public override Hit Intersect(Ray ray)
        {
            List<Hit> hits = new List<Hit>();

            foreach (Triangle tri in triangles)
            {
                Hit tmpHit = tri.Trace(ray);
                if (tmpHit != null)
                {
                    hits.Add(tmpHit);
                }
            }

            if (hits.Count > 0)
            {
                Hit triHit = hits[0];
                if (hits.Count > 1)
                {
                    float? minDist = float.MaxValue;
                    foreach (Hit tmpHit in hits)
                    {
                        if (tmpHit.distance < minDist.Value)
                        {
                            minDist = tmpHit.distance;
                            triHit = tmpHit;
                        }
                    }
                }
                    
                return triHit;
            }

            return null;
        }
    }
}
