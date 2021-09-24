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
using Plane = SyntheseTP1.Shapes.Plane;
using System.Diagnostics;

namespace SyntheseTP1
{
	public partial class MainWindow : Form, IMessageFilter
	{
		private DirectBitmap img;

		private const int WM_KEYDOWN = 0x0100;

		private int mode = 0;
		private DateTime lastFrame;
		private DateTime startTime;
		private bool running = true;

		private Task loopTask;

		private List<GameObject> gameObjects;

		DirectBitmap fullRes;
		DirectBitmap smallRes;

		bool highRes = true;

		float rotX = 0;
		float rotY = 0;

		public MainWindow()
		{
			InitializeComponent();
			Application.AddMessageFilter(this);

			gameObjects = new List<GameObject>();
			startTime = DateTime.Now;

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);


			fullRes = new DirectBitmap(ClientSize.Width, ClientSize.Height);
			smallRes = new DirectBitmap(ClientSize.Width/8, ClientSize.Height/8);
			img = highRes ? fullRes : smallRes;

			SetupScene();
			DrawRayTrace();
			img.Bitmap.Save("img.png", ImageFormat.Png);
		}


		public void SetupScene()
        {
			Scene.InitScene();

			Material red = new Material { color = new HDRColor(1, 1, 1) };
			Material green = new Material { color = new HDRColor(0.01f, 1, 0.01f) };

			Scene.shapes.Add(new Sphere
			{
				position = new Vector3(0, 0, 4),
				material = red
			});

			Scene.shapes.Add(new Sphere
			{
				radius = 0.5f,
				position = new Vector3(0.75f, 0, 3.5f),
				material = green
			});

			Scene.shapes.Add(new Plane
			{
				position = new Vector3(0, 1, 0),
				rotation = Quaternion.CreateFromEulerAnglesDeg(0, 0, 0),
				material = red
			});

			Scene.lights.Add(new PointLight
			{
				position = new Vector3(2, 0.5f, 3),
				intensity = 3
			});
		}

		private void MainWindow_Resize(object sender, EventArgs e)
		{
			fullRes = new DirectBitmap(ClientSize.Width, ClientSize.Height);
			smallRes = new DirectBitmap(ClientSize.Width / 8, ClientSize.Height / 8);
			img = highRes ? fullRes : smallRes;
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
					graphics.DrawImage(img.Bitmap, 0,0,ClientSize.Width,ClientSize.Height);
					break;
				case 1:
					foreach(GameObject gameObject in gameObjects)
                    {
						gameObject.Render(ref graphics);
                    }
					break;
            }
		}

		private void DrawRayTrace()
        {
			Scene.camera.rotation = Quaternion.CreateFromYawPitchRoll(rotY * (float)MathEx.DegToRad, rotX * (float)MathEx.DegToRad, 0);

			HDRColor[,] buffer = new HDRColor[(int)img.Res.X, (int)img.Res.Y];

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			//Send pixels rays
			Parallel.For(0, img.Width, 
			x => {
				for (int y = 0; y < img.Height; y++)
				{
					Ray camRay = Scene.camera.PixelToRay(new Vector2(x, y), img.Res);

					buffer[x, y] = Scene.SendRay(camRay);
				}
			});

			stopwatch.Stop();
			Console.WriteLine("Rendered in {0} ms", stopwatch.ElapsedMilliseconds);

			//Render buffer to screen
			for (int x = 0; x < img.Width; x++)
			{
				for (int y = 0; y < img.Height; y++)
				{
					img.SetPixel(x, y, buffer[x, y].ToColor());
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
					//FORCE RENDER
					case (int)Keys.Decimal:
						mode = 0;
						DrawRayTrace();
						img.Bitmap.Save("img.png", ImageFormat.Png);
						break;

					//RESOLUTION SWAP
					case (int)Keys.NumPad0:
						highRes = !highRes;
						img = highRes ? fullRes : smallRes;
						DrawRayTrace();
						break;

					//CAMERA TRANSLATION
					case (int)Keys.Q:
						if (Scene.camera != null)
							Scene.camera.Translate(-0.1f, 0, 0);
						DrawRayTrace();
						break;
					case (int)Keys.D:
						if (Scene.camera != null)
							Scene.camera.Translate(0.1f, 0, 0);
						DrawRayTrace();
						break;
					case (int)Keys.Z:
						if (Scene.camera != null)
							Scene.camera.Translate(0, 0, 0.1f);
						DrawRayTrace();
						break;
					case (int)Keys.S:
						if (Scene.camera != null)
							Scene.camera.Translate(0, 0, -0.1f);
						DrawRayTrace();
						break;
					case (int)Keys.A:
						if (Scene.camera != null)
							Scene.camera.Translate(0, 0.1f, 0);
						DrawRayTrace();
						break;
					case (int)Keys.E:
						if (Scene.camera != null)
							Scene.camera.Translate(0, -0.1f, 0);
						DrawRayTrace();
						break;


					//CAMERA ROTATION
					case (int)Keys.Left:
						rotY -= 5;
						DrawRayTrace();
						break;
					case (int)Keys.Right:
						rotY += 5;
						DrawRayTrace();
						break;
					case (int)Keys.Up:
						rotX += 5;
						DrawRayTrace();
						break;
					case (int)Keys.Down:
						rotX -= 5;
						DrawRayTrace();
						break;
				}
			}

			return false;
		}

		
	}
}
