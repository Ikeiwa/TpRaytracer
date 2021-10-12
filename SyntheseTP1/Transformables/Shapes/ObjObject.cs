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
        public BoundingBox bounds { get; private set; }
        private List<Triangle> triangles;
        private BoundTreeNode boundingBoxTree;

        private List<Triangle> LoadObj(string fileName)
        {
            List<Triangle> shapes = new List<Triangle>();
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            if (File.Exists(path))
            {
                Vector3 min = Vector3.MaxValue;
                Vector3 max = Vector3.MinValue;

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

                        Vector3 vert = new Vector3(X, Y, Z);

                        min = min.Min(vert);
                        max = max.Max(vert);

                        vertices.Add(vert);
                    }
                    else if (objData[i].StartsWith("f "))
                    {
                        string[] verticeData = objData[i].Substring(2).Split(' ');
                        int A = int.Parse(verticeData[0].Split('/')[0])-1;
                        int B = int.Parse(verticeData[1].Split('/')[0])-1;
                        int C = int.Parse(verticeData[2].Split('/')[0])-1;

                        shapes.Add(new Triangle(vertices[A],vertices[B],vertices[C])
                        {
                            material = material
                        });
                    }
                }

                bounds = new BoundingBox(min, max);
            }

            return shapes;
        }

        public ObjObject(string filename, Material material = null)
        {
            if (material == null)
                this.material = Material.white;
            else
                this.material = material;
            this.filename = filename;
            triangles = LoadObj(filename);
            boundingBoxTree = new BoundTreeNode(triangles, bounds);
        }

        public override float? Intersect(Ray ray)
        {
            Ray offsetRay = new Ray(ray.position-position,ray.direction);
            
            Hit result = boundingBoxTree.Intersect(offsetRay);
            if (result != null)
                return result.distance;
            return null;
        }

        public override Hit Trace(Ray ray)
        {
            Ray offsetRay = new Ray(ray.position-position,ray.direction);
            return boundingBoxTree.Intersect(offsetRay);
        }
    }
}
