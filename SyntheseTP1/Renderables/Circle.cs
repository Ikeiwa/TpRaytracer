using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;
using System.Drawing;

namespace SyntheseTP1.Renderables
{
    class Circle : GameObject
    {
        public Circle(){}

        private Vector3 size = new Vector3(100, 100,100);

        public Circle(Color color)
        {
            this.color = color;
        }

        public override void Render(ref Graphics graphics)
        {
            Brush brush = new SolidBrush(color);
            Vector3 realSize = size * scale;
            graphics.FillEllipse(brush, position.X*100- realSize.X/2, position.Y*100 - realSize.Y/2, realSize.X, realSize.Y);
        }
    }
}
