using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;

namespace TreeGenerator
{
    public class ObjExporter
    {
        private TextWriter textWriter;

        private int count = 0;
        private int totalVertexCount = 0;
        public void ExportToObj(string fileName,List<TangentVertex> vertices)
        {
            count = 0;
            textWriter = new StreamWriter(fileName);
            WriteObject(vertices);
            textWriter.Close();
        }
        public void ExportToObj(string fileName,TreeEngine.EngineTreeRenderData renderData)
        {
            count = 0;
            totalVertexCount = 0;
            textWriter = new StreamWriter(fileName);
            WriteObject(renderData.TreeBody.Vertices1);
            count++;
            for (int i = 0; i < renderData.Leaves.Count; i++)
            {
                WriteObject(renderData.Leaves[i].Vertices1);
                count++;
            }
            textWriter.Close();
        }
        private void WriteObject(List<TangentVertex> vertices)
        {
            int vertexCount = vertices.Count;
            textWriter.WriteLine('#');
            textWriter.WriteLine("#object Object" + count.ToString());
            textWriter.WriteLine('#');
            textWriter.WriteLine("");

            for (int i = 0; i < vertexCount; i++)
            {
                textWriter.WriteLine(("v  " + vertices[i].pos.X.ToString("F4") + " " + vertices[i].pos.Y.ToString("F4") + " " + vertices[i].pos.Z.ToString("F4")).Replace(',','.'));
            }
            textWriter.WriteLine("# " + vertexCount.ToString() + " Vertices");
            textWriter.WriteLine("");

            for (int i = 0; i < vertexCount; i++)
            {
                textWriter.WriteLine(("vn " + (vertices[i].normal.X).ToString("F4") + " " + (vertices[i].normal.Y).ToString("F4") + " " + (vertices[i].normal.Z).ToString("F4")).Replace(',','.'));
            }
            textWriter.WriteLine("# " + vertexCount.ToString() + " vertex normals");
            textWriter.WriteLine("");

            for (int i = 0; i < vertexCount; i++)
            {
                textWriter.WriteLine(("vt " + vertices[i].U.ToString("F4") + " " + vertices[i].V.ToString("F4") + " " + "0.0000").Replace(',','.'));
            }

            textWriter.WriteLine("g Object" + count.ToString());

            for (int i = 0; i < vertexCount/3; i++)
            {
                //for some reasen 3dsmax uses an other cullmode than I do
                textWriter.WriteLine("f " + (totalVertexCount + i * 3 + 1).ToString() + '/' + (totalVertexCount + i * 3 + 1).ToString() + '/' + (totalVertexCount + i * 3 + 1).ToString() + ' ' + (totalVertexCount + i * 3 + 3).ToString() + '/'+ (totalVertexCount + i * 3 + 3).ToString() + '/' + (totalVertexCount + i * 3 + 3).ToString() +' ' + (totalVertexCount + i * 3 + 2).ToString() + '/' + (totalVertexCount + i * 3 + 2).ToString() + '/' + (totalVertexCount + i * 3 + 2).ToString() );
            }
            textWriter.WriteLine("# " + (vertexCount/3).ToString() + " faces");

            totalVertexCount += vertexCount;
        }




        public static void TestEditorMeshPartRenderDataSimple()
        {
           
            XNAGame game = new XNAGame();
            ColladaShader shader = null;
         


            game.InitializeEvent +=
                delegate
                    {
                        TreeStructure struc;
                        TreeEngine.EngineTreeRenderDataGenerater gen = new TreeEngine.EngineTreeRenderDataGenerater(20);
                        struc = TreeStructure.GetTestTreeStructure(game);
                        TreeEngine.EngineTreeRenderData renderData = gen.GetRenderData(struc, game, 0);
                        //List<TangentVertex> vertices = new List<TangentVertex>();
                        //vertices.Add(new TangentVertex(new Vector3(-1, 2.5f, 0), new Vector2(0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)));
                        //vertices.Add(new TangentVertex(new Vector3(-1, 0, 0), new Vector2(0, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 0)));
                        //vertices.Add(new TangentVertex(new Vector3(1, 0, 0), new Vector2(1, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 0)));

                        //vertices.Add(new TangentVertex(new Vector3(-1, 2.5f, 0), new Vector2(0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)));
                        //vertices.Add(new TangentVertex(new Vector3(1, 2.5f, 0), new Vector2(1, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)));
                        //vertices.Add(new TangentVertex(new Vector3(1, 0, 0), new Vector2(1, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 0)));

                        ObjExporter exp=new ObjExporter();
                        exp.ExportToObj("testExportNoFlipNormals.obj", renderData);
                    };

            

            game.Run();

        }

    }
}
