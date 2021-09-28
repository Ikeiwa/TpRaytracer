using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1.Transformables.Lights
{
    class DirectionalLight : Light
    {

        public override float GetEnergyAtPoint(Vector3 point, Vector3 normal)
        {
            if (!shadows || Scene.IsPointVisible(new Ray(point, -Vector3.UnitZ.Transform(rotation) * float.MaxValue), Scene.shapes))
            {
                return intensity;
            }
            return 0;
        }
    }
}
