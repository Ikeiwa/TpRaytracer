using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntheseTP1.Shapes;
using SyntheseTP1.Transformables.Lights;
using Vim.Math3d;

namespace SyntheseTP1
{
    static class Scene
    {
        public static Camera camera { get; private set; }
        public static List<Shape> shapes { get; private set; }
        public static List<Light> lights { get; private set; }

        public static void InitScene()
        {
            camera = new Camera();
            shapes = new List<Shape>();
            lights = new List<Light>();
        }

		public static bool GetClosestShape(Ray ray, List<Shape> shapes, out Hit hit)
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

		public static bool IsPointVisible(Ray ray, List<Shape> shapes)
		{
			foreach (Shape shape in shapes)
			{
				float? shapeDist = shape.Intersect(ray);
				if (shapeDist.HasValue && shapeDist.Value < ray.length)
					return false;
			}
			return true;
		}

		public static HDRColor SendRay(Ray ray)
        {
			bool hasHit = GetClosestShape(ray, shapes, out Hit hit);

			HDRColor energy = new HDRColor(0, 0, 0);

			if (hasHit)
			{
				foreach(Light light in lights)
                {
					float lightEnergy = light.GetEnergyAtPoint(hit.position);
					if (lightEnergy>0)
					{
						HDRColor finalColor = hit.material.color;
						float LdotN = MathOps.Clamp(Vector3.Dot(hit.normal, -light.GetDirection(hit.position)),0,1);
						finalColor *= light.color * LdotN * lightEnergy;
						energy += finalColor;
					}
				}

				return energy;
			}

			return new HDRColor(1, 0, 1);
		}
	}
}
