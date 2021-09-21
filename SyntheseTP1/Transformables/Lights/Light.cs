using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntheseTP1.Transformables;

namespace SyntheseTP1.Transformables.Lights
{
     class Light : Transformable
    {
        public double Intensity = 1;
        public HDRColor color = new HDRColor(1,1,1);
    }
}
