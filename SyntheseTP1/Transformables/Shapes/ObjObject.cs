using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.Math3d;

namespace SyntheseTP1.Shapes
{
    class ObjObject : Shape
    {
        public string filename;
        private List<Shape> triangles;

        public List<Shape> LoadObj(string fileName)
        {
            List<Shape> shapes = new List<Shape>();
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            if (File.Exists(path))
            {
                string[] objData = File.ReadAllText(path).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                List<Vector3> vertices = new List<Vector3>();

                for(int i = 0; i < objData.Length; i++)
                {
                    if(objData[i].StartsWith("v "))
                    {
                        string[] verticeData = objData[i].Substring(2).Split(' ');
                        float X = float.Parse(verticeData[0], CultureInfo.InvariantCulture);
                        float Y = float.Parse(verticeData[1], CultureInfo.InvariantCulture);
                        float Z = float.Parse(verticeData[2], CultureInfo.InvariantCulture);

                        vertices.Add(new Vector3(X,Y,Z));
                    }
                    else if (objData[i].StartsWith("f "))
                    {
                        string[] verticeData = objData[i].Substring(2).Split(' ');
                        int A = int.Parse(verticeData[0].Split('/')[0])-1;
                        int B = int.Parse(verticeData[1].Split('/')[0])-1;
                        int C = int.Parse(verticeData[2].Split('/')[0])-1;

                        shapes.Add(new Triangle
                        {
                            A = vertices[A],
                            B = vertices[B],
                            C = vertices[C],
                            material = material
                        });
                    }
                }
            }

            return shapes;
        }

        public ObjObject(string filename)
        {
            this.filename = filename;
            triangles = LoadObj(filename);
        }

        public override float? Intersect(Ray ray)
        {
            return Intersect(ray,out Vector3 normal);
        }

        public float? Intersect(Ray ray, out Vector3 normal)
        {
            List<Hit> hits = new List<Hit>();

            foreach (Shape tri in triangles)
            {
                Hit tmpHit = tri.Trace(ray);
                if (tmpHit != null)
                {
                    hits.Add(tmpHit);
                }
            }

            if (hits.Count > 0)
            {
                Hit hit = hits[0];
                if (hits.Count > 1)
                {
                    
                    float? minDist = float.MaxValue;

                    foreach (Hit tmpHit in hits)
                    {
                        if (tmpHit.distance < minDist.Value)
                        {
                            minDist = tmpHit.distance;
                            hit = tmpHit;
                        }
                    }
                }

                normal = hit.normal;
                return hit.distance;
            }

            normal = Vector3.UnitZ;
            return null;
        }

        public override Hit Trace(Ray ray)
        {
            float? dist = Intersect(ray, out Vector3 normal);

            if (dist.HasValue)
            {
                Hit hit = new Hit();
                hit.material = material;
                hit.distance = dist.Value;
                hit.truePosition = ray.GetEnd(dist.Value);
                hit.position = ray.GetEnd(dist.Value - MathEx.RayOffset);
                hit.normal = normal.Normalize();
                return hit;
            }
            return null;
        }
    }
}
