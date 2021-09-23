using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntheseTP1.Transformables;
using Vim.Math3d;

namespace SyntheseTP1.Transformables.Lights
{
     abstract class Light : Transformable
     {
        public float intensity = 1;
        public HDRColor color = new HDRColor(1,1,1);

        public abstract float GetEnergyAtPoint(Vector3 point);

        public abstract Vector3 GetDirection(Vector3 point);
    }
}
