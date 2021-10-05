using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntheseTP1.Shapes;
using Vim.Math3d;
using Triangle = SyntheseTP1.Shapes.Triangle;

namespace SyntheseTP1
{
    class ObjLoader
    {
        public static void LoadObj(string fileName, ref List<Shape> shapes)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            if (File.Exists(path))
            {
                Console.WriteLine("Loading obj");
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
                        Console.WriteLine("added " + vertices.Last().ToString());
                    }
                    else if (objData[i].StartsWith("f "))
                    {
                        string[] verticeData = objData[i].Substring(2).Split(' ');
                        int A = int.Parse(verticeData[0].Split('/')[0]);
                        int B = int.Parse(verticeData[1].Split('/')[0]);
                        int C = int.Parse(verticeData[2].Split('/')[0]);

                        Console.WriteLine(A + " " + B + " " + C);

                        Triangle tri = new Triangle
                        {
                            A = vertices[A],
                            B = vertices[B],
                            C = vertices[C]
                        };

                        /*shapes.Add(new Triangle
                        {
                            A = vertices[A],
                            B = vertices[B],
                            C = vertices[C]
                        });*/
                        Console.WriteLine("Adding triangle " + A + " " + B + " " + C);
                    }
                }
            }
        }
    }
}
