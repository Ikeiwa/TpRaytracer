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
using SyntheseTP1.Renderables;
using SyntheseTP1.Components;
using Vim.Math3d;
using SyntheseTP1.Shapes;
using Sphere = SyntheseTP1.Shapes.Sphere;
using SyntheseTP1.Transformables.Lights;

namespace SyntheseTP1
{
	public partial class MainWindow : Form, IMessageFilter
	{
		private DirectBitmap img;

		private const int WM_KEYDOWN = 0x0100;

		private Vector2 res;

		private int mode = 0;
		private DateTime lastFrame;
		private DateTime startTime;
		private bool running = true;

		private Task loopTask;

		private List<GameObject> gameObjects;

		

		public MainWindow()
		{
			InitializeComponent();
			Application.AddMessageFilter(this);

			gameObjects = new List<GameObject>();
			startTime = DateTime.Now;

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

			img = new DirectBitmap(ClientSize.Width, ClientSize.Height);
			res = new Vector2(ClientSize.Width, ClientSize.Height);
		}

		private void MainWindow_Resize(object sender, EventArgs e)
		{
			img = new DirectBitmap(ClientSize.Width, ClientSize.Height);
			res = new Vector2(ClientSize.Width, ClientSize.Height);
		}

		public async void MainLoop()
        {
			lastFrame = DateTime.Now;

			while (running)
			{
				TimeSpan dTime = DateTime.Now - lastFrame;
				Time.deltaTime = (float)dTime.TotalSeconds;
				Time.time = (float)(DateTime.Now - startTime).TotalSeconds;
				lastFrame = DateTime.Now;
				UpdateLoop();
				Invalidate();

				await Task.Delay(8);
			}
		}

		public void UpdateLoop()
        {
			foreach (GameObject gameObject in gameObjects)
			{
				gameObject.Update();
			}

			//gameObjects[0].position = new Vector3((float)Math.Sin(Time.time)*100+200, 200, 0);
        }

		private void MainWindow_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;

            switch (mode)
            {
				case 0:
					graphics.DrawImage(img.Bitmap, new Point(0, 0));
					break;
				case 1:
					foreach(GameObject gameObject in gameObjects)
                    {
						gameObject.Render(ref graphics);
                    }
					break;
            }
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


		private bool GetClosestShape(Ray ray,List<Shape> shapes, out Hit hit)
        {
			float? minDist = float.MaxValue;
			Hit minHit = null;

			foreach (Shape shape in shapes)
			{
				Hit tmpHit = shape.Trace(ray);
				if (tmpHit != null && tmpHit.distance < minDist.Value)
				{
					minDist = tmpHit.distance;
					minHit = tmpHit;
				}
			}

			hit = minHit;

			return hit != null;
		}

		private bool IsPointVisible(Ray ray,List<Shape> shapes)
        {
			foreach (Shape shape in shapes)
			{
				float? shapeDist = shape.Intersect(ray);
                if (shapeDist.HasValue && shapeDist.Value < ray.length)
						return false;
			}
			return true;
		}

		private void DrawRayTrace()
        {
			Camera cam = new Camera();

			List<Shape> shapes = new List<Shape>();

			Material red = new Material { color = new HDRColor(1, 0, 0) };
			Material green = new Material { color = new HDRColor(0, 1, 0) };

			shapes.Add(new Sphere 
			{ 
				position = new Vector3(0, 0, 4), 
				material = red
			});

			shapes.Add(new Sphere
			{
				radius = 0.5f,
				position = new Vector3(0.75f, 0, 3.5f),
				material = green
			});


			Light light = new Light { position = new Vector3(2, 0, 3) };

			for (int x = 0; x < img.Width; x++)
			{
				for (int y = 0; y < img.Height; y++)
				{
					Ray camRay = cam.PixelToRay(new Vector2(x, y), res);

					bool hasHit = GetClosestShape(camRay, shapes, out Hit hit);

					if(hasHit)
                    {
						if(IsPointVisible(new Ray(hit.position, light.position- hit.position), shapes))
                        {
							HDRColor finalColor = hit.shape.material.color;
							finalColor *= Vector3.Dot(hit.normal, (light.position - hit.position).Normalize());

							img.SetPixel(x, y, finalColor.ToColor());
                        }
                        else
                        {
							img.SetPixel(x, y, Color.Black);
						}


					}
					else
					{
						img.SetPixel(x, y, Color.Magenta);
					}
                    
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
					case (int)Keys.Escape:
						running = false;
						break;
					case (int)Keys.NumPad0:
						mode = 0;
						DrawMandlebrot();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						return true;
					case (int)Keys.NumPad1:
						mode = 0;
						DrawCircle();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						return true;
					case (int)Keys.NumPad2:
						mode = 0;
						DrawChecker();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						return true;
					case (int)Keys.NumPad3:
						mode = 0;
						DrawCircleGradient();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						return true;
					case (int)Keys.NumPad4:
						mode = 1;
						Time.deltaTime = 1;
						gameObjects = new List<GameObject>();
						Circle circle = new Circle(Color.Red);
						circle.AddComponent<RigidBody2D>().AddForce(new Vector3(50,0,0));
						gameObjects.Add(circle);
						if (loopTask == null)
							loopTask = Task.Run(MainLoop);
						break;
					case (int)Keys.NumPad5:
						mode = 0;
						DrawRayTrace();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						break;
				}
			}

			return false;
		}

		
	}
}
