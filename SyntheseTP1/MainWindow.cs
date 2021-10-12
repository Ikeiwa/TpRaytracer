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
		

		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;

		private Dictionary<Keys,int> keyActions;
		private Vector2 mouse;
		private Vector2 lastMouse;

		private int mode = 0;
		private DateTime lastFrame;
		private DateTime startTime;
		private bool running = true;

		private List<GameObject> gameObjects;

		DirectBitmap fullRes;
		DirectBitmap smallRes;
		private DirectBitmap img;

		HDRColor[,] buffer;

		bool highRes = true;

		const int samples = 1;

		float rotX = 0;
		float rotY = 0;

		public MainWindow()
		{
			InitializeComponent();
			Application.AddMessageFilter(this);

			gameObjects = new List<GameObject>();
			keyActions = new Dictionary<Keys, int>();
			mouse = new Vector2(Cursor.Position.X, Cursor.Position.Y);
			lastMouse = mouse;
			startTime = DateTime.Now;

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
			Cursor.Hide();

			SetupBuffers();
			SetupScene();
			if (highRes)
            {
				DrawRayTrace();
				img.Bitmap.Save("img.png", ImageFormat.Png);
			}
			MainLoop();
		}


		public void SetupScene()
        {
			Scene.InitScene();

			Scene.camera.type = Camera.CameraType.Perspective;
			Scene.camera.position = new Vector3(0, 0, -2);
			Scene.camera.fov = 39;

			Material white = new Material { color = new HDRColor(1, 1, 1) };
			Material black = new Material { color = new HDRColor(0, 0, 0) };
			Material green = new Material { color = new HDRColor(0, 1, 0) };
			Material red = new Material { color = new HDRColor(1, 0, 0) };
			Material mirror = new Material { color = new HDRColor(1f, 1f, 1f), type = MaterialType.Mirror };
			Material glass = new Material { color = new HDRColor(1f, 1f, 1f), type = MaterialType.Glass, IOR = 1.5f };

			/*Scene.shapes.Add(new Sphere
			{
				position = new Vector3(-0.2f, -0.2f, 0),
				material = mirror,
				radius = 0.15f
			});

			Scene.shapes.Add(new Sphere
			{
				position = new Vector3(0.2f, -0.2f, 0),
				material = glass,
				radius = 0.15f
			});

			Scene.shapes.Add(new Sphere
			{
				position = new Vector3(0, 0.2f, 0),
				material = white,
				radius = 0.15f
			});*/

			Scene.shapes.Add(new ObjObject("monkey.obj") {material = white});

			Scene.shapes.Add(new Plane
			{
				position = new Vector3(0, -0.5f, 0),
				rotation = Quaternion.CreateFromEulerAnglesDeg(0, 0, 0),
				material = white
			});

			Scene.shapes.Add(new Plane
			{
				position = new Vector3(0, 0.5f, 0),
				rotation = Quaternion.CreateFromEulerAnglesDeg(0, 0, 180),
				material = white
			});

			Scene.shapes.Add(new Plane
			{
				position = new Vector3(0, 0, 0.5f),
				rotation = Quaternion.CreateFromEulerAnglesDeg(-90, 0, 0),
				material = white
			});

			Scene.shapes.Add(new Plane
			{
				position = new Vector3(0, 0, -0.5f),
				rotation = Quaternion.CreateFromEulerAnglesDeg(90, 0, 0),
				material = black
			});

			Scene.shapes.Add(new Plane
			{
				position = new Vector3(-0.5f, 0, 0),
				rotation = Quaternion.CreateFromEulerAnglesDeg(0, 0, -90),
				material = red
			});

			Scene.shapes.Add(new Plane
			{
				position = new Vector3(0.5f, 0, 0),
				rotation = Quaternion.CreateFromEulerAnglesDeg(0, 0, 90),
				material = green
			});

			Scene.lights.Add(new PointLight
			{
				position = new Vector3(-0.4f, 0.4f, -0.4f),
				intensity = 0.05f,
				radius = 0.1f
			});
		}

		void SetupBuffers()
        {
			fullRes = new DirectBitmap(ClientSize.Width, ClientSize.Height);
			smallRes = new DirectBitmap(ClientSize.Width / 4, ClientSize.Height / 4);
			img = highRes ? fullRes : smallRes;
			buffer = new HDRColor[(int)img.Res.X, (int)img.Res.Y];
		}

		private void MainWindow_Resize(object sender, EventArgs e)
		{
			SetupBuffers();
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
				ProcessInputs();
				UpdateLoop();
				Invalidate();

				await Task.Delay(8);
			}
		}

        private void ProcessInputs()
        {
            if (Focused)
            {
				foreach (Keys key in keyActions.Keys)
				{
					switch (key)
					{
						case Keys.Escape:
							Close();
							break;

						//FORCE RENDER
						case Keys.Decimal:
							if (keyActions[key] == 0)
							{
								DrawRayTrace();
								img.Bitmap.Save("img.png", ImageFormat.Png);
							}
							break;

						//RESOLUTION SWAP
						case Keys.NumPad0:
							if (keyActions[key] == 0)
							{
								highRes = !highRes;
								SetupBuffers();
								DrawRayTrace();
								img.Bitmap.Save("img.png", ImageFormat.Png);
							}
							break;

						//CAMERA TRANSLATION
						case Keys.Q:
							if (Scene.camera != null)
								Scene.camera.Translate(-Time.deltaTime*2, 0, 0);
							break;
						case Keys.D:
							if (Scene.camera != null)
								Scene.camera.Translate(Time.deltaTime * 2, 0, 0);
							break;
						case Keys.Z:
							if (Scene.camera != null)
								Scene.camera.Translate(0, 0, Time.deltaTime * 2);
							break;
						case Keys.S:
							if (Scene.camera != null)
								Scene.camera.Translate(0, 0, -Time.deltaTime * 2);
							break;
						case Keys.A:
							if (Scene.camera != null)
								Scene.camera.Translate(0, -Time.deltaTime * 2, 0);
							break;
						case Keys.E:
							if (Scene.camera != null)
								Scene.camera.Translate(0, Time.deltaTime * 2, 0);
							break;


						//CAMERA ROTATION
						case Keys.Left:
							rotY -= Time.deltaTime * 20;
							break;
						case Keys.Right:
							rotY += Time.deltaTime * 20;
							break;
						case Keys.Up:
							rotX += Time.deltaTime * 20;
							break;
						case Keys.Down:
							rotX -= Time.deltaTime * 20;
							break;

						//CAMERA FOV
						case Keys.Add:
							if (keyActions[key] == 0)
								Scene.camera.fov += 5;
							break;
						case Keys.Subtract:
							if (keyActions[key] == 0)
								Scene.camera.fov -= 5;
							break;
					}
				}

				Point windowCenter = new Point(DesktopLocation.X + ClientRectangle.Width / 2, DesktopLocation.Y + ClientRectangle.Height / 2);

				lastMouse = mouse;
				mouse = new Vector2(Cursor.Position.X, Cursor.Position.Y);

				rotY += (mouse.X - windowCenter.X) / 20;
				rotX += (mouse.Y - windowCenter.Y) / 20;

				Cursor.Position = windowCenter;
            }
			
		}

        public void UpdateLoop()
        {
			Scene.camera.rotation = Quaternion.CreateFromYawPitchRoll(rotY * (float)MathEx.DegToRad, rotX * (float)MathEx.DegToRad, 0);

			if (!highRes)
				DrawRayTrace();
		}

		private void MainWindow_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;

            switch (mode)
            {
				case 0:
					graphics.DrawImage(img.Bitmap, 0, 0, ClientSize.Width,ClientSize.Height);
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
            //Send pixels rays
            if (highRes)
            {
				float offset = 0.5f;
				if (samples == 1)
					offset = 0;

				Parallel.For(0, img.Width,
				x => {
					for (int y = 0; y < img.Height; y++)
					{
						List<HDRColor> results = new List<HDRColor>();
						for(int i = 0; i < samples; i++)
                        {
							Vector2 pixelOffset = new Vector2(MathEx.NextFloat(-offset, offset), MathEx.NextFloat(-offset, offset));
							Ray camRay = Scene.camera.PixelToRay(new Vector2(x, y) + pixelOffset, img.Res);
							results.Add(Scene.SendRay(camRay));
                        }
						HDRColor result = HDRColor.GetAverage(results);

						buffer[x, y] = result;
					}
				});
			}
            else
            {
				Parallel.For(0, img.Width,
				x => {
					for (int y = 0; y < img.Height; y++)
					{
						Ray camRay = Scene.camera.PixelToRay(new Vector2(x, y), img.Res);
						buffer[x, y] = Scene.SendRay(camRay);
					}
				});
			}
			

			//Render buffer to screen
			Parallel.For(0, img.Width,
			x => {
				for (int y = 0; y < img.Height; y++)
				{
					img.SetPixel(x, y, buffer[x, y].ToColor());
				}
			});

			Invalidate();


		}

		public bool PreFilterMessage(ref Message m)
		{
			
			if (m.Msg == WM_KEYDOWN)
			{
                if (!keyActions.ContainsKey((Keys)m.WParam))
                {
					keyActions.Add((Keys)m.WParam, 0);
                }
                else
                {
					keyActions[(Keys)m.WParam] = 1;
                }
			}
			else if (m.Msg == WM_KEYUP)
            {
				if (keyActions.ContainsKey((Keys)m.WParam))
				{
					keyActions.Remove((Keys)m.WParam);
				}
			}

			return false;
		}

		
	}
}
