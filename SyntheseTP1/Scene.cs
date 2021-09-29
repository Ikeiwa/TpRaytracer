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
		public const int maxIndirectRays = 4;

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
				return new HDRColor(0, 0, 0);

			bool hasHit = GetClosestShape(ray, shapes, out Hit hit);

			HDRColor energy = new HDRColor(0, 0, 0);

			if (hasHit)
			{
				switch (hit.material.type)
				{
					case MaterialType.Diffuse:
						foreach (Light light in lights)
						{
							float lightEnergy = 0;
							for (int i = 0; i < maxLightRays; i++)
							{
								lightEnergy += light.GetEnergyAtPoint(hit.position, hit.normal);
							}
							lightEnergy /= maxLightRays;

							if (lightEnergy > 0)
							{
								energy += hit.material.color * light.color * lightEnergy;
							}
						}



						break;
					case MaterialType.Mirror:
						energy = SendRay(new Ray(hit.position, ray.direction.Reflect(hit.normal)), bounce + 1) * hit.material.color;
						break;
					case MaterialType.Glass:
						//energy = SendRay(new Ray(hit.position+ray.direction*MathEx.RayOffset*4, ray.direction.Refract(hit.material.IOR,hit.normal)), bounce + 1) * hit.material.color;

						float theta = Vector3.Dot(ray.direction, hit.normal);

						Vector3 normal = hit.normal;
						float IOR = hit.material.IOR;

						if (theta > 0)
                        {
							normal = -hit.normal;
							IOR = 1.0f / IOR;
                        }

						float R0 = ((1 - IOR) * (1 - IOR)) / 
								   ((1 + IOR) * (1 + IOR));
						float RTheta = R0 + (1 - R0) * (float)Math.Pow(1 - (float)Math.Cos(theta), 5);

						Vector3 refractDir = ray.direction.Refract(IOR, normal);

						HDRColor refractEnergy = SendRay(new Ray(hit.position + refractDir * MathEx.RayOffset * 2, refractDir), bounce + 1);
						HDRColor reflectEnergy = SendRay(new Ray(hit.position, ray.direction.Reflect(hit.normal)), bounce + 1);

						energy = HDRColor.Lerp(reflectEnergy, refractEnergy, RTheta) * hit.material.color;
						break;
					
				}

				return energy;
			}

			return new HDRColor(1, 0, 1);
		}
	}
}
