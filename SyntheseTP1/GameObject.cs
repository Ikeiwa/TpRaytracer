using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;
using System.Drawing;

namespace SyntheseTP1
{
    class GameObject
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale = Vector3.One;

        public Color color;

        public List<Component> components { get; private set; }

        public GameObject()
        {
            components = new List<Component>();
        }

        public T AddComponent<T>() where T : Component
        {
            components.Add((T)Activator.CreateInstance(typeof(T), this));
            return (T)components.Last();
        }

        public T GetComponent<T>() where T : Component
        {
            return components.OfType<T>().First();
        }

        public void Update()
        {
            foreach(Component comp in components)
            {
                comp.Update();
            }
        }

        public virtual void Render(ref Graphics graphics)
        {

        }
    }
}
