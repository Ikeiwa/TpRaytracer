using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vim.Math3d;

namespace SyntheseTP1
{
	public partial class MainWindow : Form, IMessageFilter
	{
		private DirectBitmap img;

		private const int WM_KEYDOWN = 0x0100;

		private Vector2 res;

		public MainWindow()
		{
			InitializeComponent();
			Application.AddMessageFilter(this);

			/*Timer updateTimer = new Timer();
			updateTimer.Interval = 50;
			updateTimer.Tick += Update;
			updateTimer.Start();*/
			img = new DirectBitmap(Size.Width, Size.Height);
			res = new Vector2(Size.Width, Size.Height);
		}

		private void MainWindow_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImage(img.Bitmap, new Point(0, 0));
		}

		public float distanceToMandelbrot(Vector2 c)
		{
			float di = 1.0f;
			Vector2 z = Vector2.Zero;
			float m2 = 0.0f;
			Vector2 dz = Vector2.Zero;

			for (int i = 0; i < 300; i++)
			{
				if (m2 > 1024.0f) { di = 0.0f; break; }

				dz = 2.0f*new Vector2(z.X*dz.X-z.Y*dz.Y,z.X*dz.Y+z.Y*dz.X)+Vector2.UnitX;

				z = new Vector2(z.X * z.X - z.Y * z.Y, 2.0f * z.X * z.Y) + c;

				m2 = Vector2.Dot(z, z);
			}

			float d = 0.5f * (float)Math.Sqrt(Vector2.Dot(z, z) / Vector2.Dot(dz, dz)) * (float)Math.Log(Vector2.Dot(z, z));
			if (di > 0.5f) d = 0.0f;

			return d;
		}

		private void DrawMandlebrot()
		{
			for (int x = 0; x < img.Width; x++)
			{
				for (int y = 0; y < img.Height; y++)
				{
					Vector2 pos = new Vector2(x, y);
					pos -= res;
					pos /= res;
					float distance = distanceToMandelbrot(pos);
					Console.WriteLine(distance);
					distance = MathOps.Clamp((float)Math.Pow(4.0f * distance, 0.2f), 0.0f, 1.0f);

					int red = (int)(distance*255);

					img.SetPixel(x, y, Color.FromArgb(255, red, 0, 0));
				}
			}

			Invalidate();
		}

		private void DrawCircle()
		{
			for (int x = 0; x < img.Width; x++)
			{
				for (int y = 0; y < img.Height; y++)
				{
					int red = 0;
					Vector2 pos = new Vector2(x, y);
					pos -= res / 2;

					if (pos.Length() < 50.0f)
						red = 255;

					img.SetPixel(x, y, Color.FromArgb(255, red, 0, 0));
				}
			}

			Invalidate();
		}

		private void DrawCircleGradient()
		{
			for (int x = 0; x < img.Width; x++)
			{
				for (int y = 0; y < img.Height; y++)
				{
					int red = 0;
					Vector2 pos = new Vector2(x, y);
					pos -= res / 2;

					red = (int)((1-MathOps.Clamp(pos.Length()/200.0f,0.0f,1.0f))*255);

					img.SetPixel(x, y, Color.FromArgb(255, red, 0, 0));
				}
			}

			Invalidate();
		}

		private void DrawChecker()
		{
			float size = 10;

			for (int x = 0; x < img.Width; x++)
			{
				for (int y = 0; y < img.Height; y++)
				{
					int red = 0;
					if ((Math.Floor(x/size)+ Math.Floor(y / size)) % 2 == 0)
						red = 255;

					img.SetPixel(x, y, Color.FromArgb(255, red, 0, 0));
				}
			}

			Invalidate();
		}

		public bool PreFilterMessage(ref Message m)
		{
			
			if (m.Msg == WM_KEYDOWN)
			{
				switch ((int)m.WParam)
				{
					case (int)Keys.NumPad0:
						DrawMandlebrot();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						return true;
					case (int)Keys.NumPad1:
						DrawCircle();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						return true;
					case (int)Keys.NumPad2:
						DrawChecker();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						return true;
					case (int)Keys.NumPad3:
						DrawCircleGradient();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						return true;
				}
			}

			return false;
		}

		private void MainWindow_Resize(object sender, EventArgs e)
		{
			img = new DirectBitmap(Size.Width, Size.Height);
			res = new Vector2(Size.Width, Size.Height);
		}
	}
}
