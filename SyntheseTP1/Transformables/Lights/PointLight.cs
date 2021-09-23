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
        public override Vector3 GetDirection(Vector3 point)
        {
            return (point - position).Normalize();
        }

        public override float GetEnergyAtPoint(Vector3 point)
        {
            if (Scene.IsPointVisible(new Ray(position, point - position), Scene.shapes))
            {
                return intensity;
            }
            return 0;
        }
    }
}
