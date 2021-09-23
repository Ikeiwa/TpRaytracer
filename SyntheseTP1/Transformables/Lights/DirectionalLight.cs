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
        public override Vector3 GetDirection(Vector3 point = default)
        {
            return Vector3.UnitZ.Transform(rotation);
        }

        public override float GetEnergyAtPoint(Vector3 point)
        {
            if (Scene.IsPointVisible(new Ray(point, -GetDirection()*float.MaxValue), Scene.shapes))
            {
                return intensity;
            }
            return 0;
        }
    }
}
