using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1.Transformables.Lights
{
    class PointLight : Light
    {
        public float radius = 0.1f;

        public override float GetEnergyAtPoint(Vector3 point, Vector3 normal)
        {
            
            Vector3 posOffset = new Vector3(MathEx.NextFloat(Scene.rand,-1.0f, 1.0f), 
                                            MathEx.NextFloat(Scene.rand, -1.0f, 1.0f), 
                                            MathEx.NextFloat(Scene.rand, -1.0f, 1.0f));
            posOffset = posOffset.Normalize() * MathEx.NextFloat(Scene.rand, 0f, radius);

            posOffset += position;

            Ray ray = new Ray(posOffset, point - posOffset);
            if (!shadows || Scene.IsPointVisible(ray, Scene.shapes))
            {
                float NdotL = MathOps.Clamp(Vector3.Dot(normal, -(point - posOffset).Normalize()), 0, 1) * (float)Math.PI;

                return intensity * NdotL / (ray.length*ray.length);
            }
            return 0;
        }
    }
}
