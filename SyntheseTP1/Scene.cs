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
		public static Camera camera;
		public static List<Shape> shapes;
		public static List<Light> lights;

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
								energy += hit.material.GetColor(hit.uv) * light.color * lightEnergy;
							}
						}

						break;
					case MaterialType.Mirror:
						Vector3 reflectVec = ray.direction.Reflect(hit.normal);
						energy = SendRay(new Ray(hit.truePosition + reflectVec * MathEx.RayOffset, reflectVec), bounce + 1) * hit.material.GetColor(hit.uv);
						break;
					case MaterialType.Glass:
						float theta = Vector3.Dot(ray.direction, hit.normal);

						Vector3 normal = hit.normal;
						float IOR = hit.material.IOR;

						if (theta > 0)
                        {
							normal = -hit.normal;
							IOR = 1.0f / IOR;
                        }

						theta = Math.Abs(theta);
						float R0 = ((1 - IOR) * (1 - IOR)) / 
								   ((1 + IOR) * (1 + IOR));
						float RTheta = R0 + (1 - R0) * (float)Math.Pow(1 - theta, 5);
						RTheta = MathOps.Clamp(RTheta, 0, 1);

						bool randType = StaticRandom.NextDouble() > RTheta;

                        if (randType)
                        {
							Vector3 refractDir = ray.direction.Refract(IOR, normal);
							if(refractDir == Vector3.Zero)
								refractDir = ray.direction.Reflect(hit.normal);

							HDRColor refractEnergy = SendRay(new Ray(hit.truePosition + refractDir * MathEx.RayOffset, refractDir), bounce + 1);
							energy = refractEnergy * hit.material.GetColor(hit.uv);
						}
                        else
                        {
							Vector3 reflectDir = ray.direction.Reflect(hit.normal);
							HDRColor reflectEnergy = SendRay(new Ray(hit.truePosition + reflectDir * MathEx.RayOffset, reflectDir), bounce + 1);
							energy = reflectEnergy * hit.material.GetColor(hit.uv);

							//energy = reflectEnergy * hit.material.color * (1 / RTheta) * RTheta;
							//RTheta s'annule
						}

						break;
					
				}

				return energy;
			}

			return new HDRColor(1, 0, 1);
		}
	}
}
