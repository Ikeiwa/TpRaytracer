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
                List<Vector3> normals = new List<Vector3>();
                List<Vector2> uv = new List<Vector2>();

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
                    else if(objData[i].StartsWith("vn "))
                    {
                        string[] verticeData = objData[i].Substring(3).Split(' ');
                        float X = float.Parse(verticeData[0], CultureInfo.InvariantCulture);
                        float Y = float.Parse(verticeData[1], CultureInfo.InvariantCulture);
                        float Z = float.Parse(verticeData[2], CultureInfo.InvariantCulture);

                        Vector3 vert = new Vector3(X, Y, Z);

                        normals.Add(vert);
                    }
                    else if(objData[i].StartsWith("vt "))
                    {
                        string[] verticeData = objData[i].Substring(3).Split(' ');
                        float X = float.Parse(verticeData[0], CultureInfo.InvariantCulture);
                        float Y = float.Parse(verticeData[1], CultureInfo.InvariantCulture);

                        Vector2 vert = new Vector2(X, Y);

                        uv.Add(vert);
                    }
                    else if (objData[i].StartsWith("f "))
                    {
                        string[] verticeData = objData[i].Substring(2).Split(' ');

                        string[] vertA = verticeData[0].Split(new[] {'/'}, StringSplitOptions.None);
                        string[] vertB = verticeData[1].Split(new[] {'/'}, StringSplitOptions.None);
                        string[] vertC = verticeData[2].Split(new[] {'/'}, StringSplitOptions.None);
                        
                        int vA = int.Parse(vertA[0])-1;
                        int vB = int.Parse(vertB[0])-1;
                        int vC = int.Parse(vertC[0])-1;

                        Vector2 uvA = Vector2.Zero;
                        Vector2 uvB = Vector2.Zero;
                        Vector2 uvC = Vector2.Zero;

                        if (!string.IsNullOrEmpty(vertA[2]))
                        {
                            uvA = uv[int.Parse(vertA[1]) - 1];
                            uvB = uv[int.Parse(vertB[1]) - 1];
                            uvC = uv[int.Parse(vertC[1]) - 1];
                        }

                        Vector3 normalA = Vector3.Zero;
                        Vector3 normalB = Vector3.Zero;
                        Vector3 normalC = Vector3.Zero;

                        if (!string.IsNullOrEmpty(vertA[2]))
                        {
                            normalA = normals[int.Parse(vertA[2]) - 1];
                            normalB = normals[int.Parse(vertB[2]) - 1];
                            normalC = normals[int.Parse(vertC[2]) - 1];
                        }
                        else
                        {
                            Vector3 normal = (vertices[vB] - vertices[vA]).Cross(vertices[vC] - vertices[vA]);
                            normalA = normal;
                            normalB = normal;
                            normalC = normal;
                        }

                        shapes.Add(new Triangle(vertices[vA],vertices[vB],vertices[vC])
                        {
                            material = material,
                            nA = normalA,
                            nB = normalB,
                            nC = normalC,
                            uA = uvA,
                            uB = uvB,
                            uC = uvC
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
