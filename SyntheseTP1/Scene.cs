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

		public const int maxBounces = 8;
		public const int maxLightRays = 10;

		public static Random rand;

		public static void InitScene()
        {
            camera = new Camera();
            shapes = new List<Shape>();
            lights = new List<Light>();
			rand = new Random();
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

		public static HDRColor SendRay(Ray ray, int bounce = 0)
        {
			if(bounce > maxBounces)
				return new HDRColor(1, 0, 1);

			bool hasHit = GetClosestShape(ray, shapes, out Hit hit);

			HDRColor energy = new HDRColor(0, 0, 0);

			if (hasHit)
			{
				foreach(Light light in lights)
                {
					HDRColor surfColor = hit.material.color;
					/*if (hit.material.roughness < 1)
                    {
						surfColor = HDRColor.Lerp(SendRay(new Ray(hit.position,ray.direction.Reflect(hit.normal)), bounce+1), surfColor, hit.material.roughness);
					}*/

					float lightEnergy = 0;
					for(int i=0;i< maxLightRays; i++)
                    {
						lightEnergy += light.GetEnergyAtPoint(hit.position,hit.normal);
                    }
					lightEnergy /= (float)maxLightRays;

					if (lightEnergy>0)
					{
						surfColor *= light.color * lightEnergy;
						energy += surfColor;
					}
				}

				return energy;
			}

			return new HDRColor(1, 0, 1);
		}
	}
}
