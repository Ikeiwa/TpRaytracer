using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntheseTP1
{
    class Component
    {
        public GameObject Owner { get; private set; }

        public Component(GameObject owner)
        {
            Owner = owner;
        }

        public virtual void Update()
        {

        }
    }
}
