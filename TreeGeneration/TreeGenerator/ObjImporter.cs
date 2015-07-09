using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MHGameWork.TheWizards.EntityOud.Editor;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Rendering;

namespace TreeGenerator
{
    public class ObjImporter
    {
        List<Vector3> positionsReal = new List<Vector3>();
        List<Vector3> normalsReal = new List<Vector3>();
        List<Vector2> textureCoordsReal = new List<Vector2>();
        EditorMesh mesh = new EditorMesh();
        EditorMeshPart meshPart = new EditorMeshPart();
        public EditorMesh ImportObjFile(string FileName)
        {



            FileStream fileStream = new FileStream(FileName, FileMode.Open);
            TextReader textReader = new StreamReader(fileStream);
            //MeshPartGeometryData meshPart = new MeshPartGeometryData();

            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> textureCoords = new List<Vector2>();



            string line = null;
            while (textReader.Peek() != -1)
            {
                line = textReader.ReadLine();
                line = line.Replace('.', ',');
                if (line.StartsWith("#"))
                {
                    continue;
                }
                string[] pieces = line.Split(' ');

                int dataStart = 1;
                while (pieces.Length > dataStart && pieces[dataStart] == "")
                {
                    dataStart++;
                }
                dataStart--;// cheat
                switch (pieces[0])
                {
                    case "v":
                        Vector3 v = new Vector3(float.Parse(pieces[dataStart + 1]), float.Parse(pieces[dataStart + 2]), float.Parse(pieces[dataStart + 3]));
                        positions.Add(v);
                        break;
                    case "vn":
                        Vector3 j = new Vector3(float.Parse(pieces[dataStart + 1]), float.Parse(pieces[dataStart + 2]), float.Parse(pieces[dataStart + 3]));
                        normals.Add(j);
                        break;
                    case "vt":
                        Vector2 l = new Vector2(float.Parse(pieces[dataStart + 1]), float.Parse(pieces[dataStart + 2]));
                        textureCoords.Add(l);
                        break;
                    case "g":
                        if (meshPart.AdditionalData.Name != null || meshPart.AdditionalData.Name == "")
                        {
                            meshPart = new EditorMeshPart();
                            meshPart.AdditionalData.Name = pieces[1];
                            createMeshPart();
                        }
                        meshPart = new EditorMeshPart();
                        meshPart.AdditionalData.Name = pieces[1];
                        break;

                    case "f":
                        for (int i = 0; i < 3; i++)
                        {
                            string[] facePiece = pieces[i + 1].Split('/');
                            positionsReal.Add(positions[int.Parse(facePiece[0]) - 1]);
                            if (facePiece[1] != "")
                            {
                                normalsReal.Add(normals[int.Parse(facePiece[2]) - 1]);
                                textureCoordsReal.Add(textureCoords[int.Parse(facePiece[1]) - 1]);
                            }
                            else
                            {
                                normalsReal.Add(normals[int.Parse(facePiece[2]) - 1]);
                            }

                        }
                        break;
                }

            }

            createMeshPart();


            return mesh;


        }

        private void createMeshPart()
        {

            MeshPartGeometryData.Source s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Position;
            s.DataVector3 = positionsReal.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Normal;
            s.DataVector3 = normalsReal.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            if (textureCoordsReal.Count > 0)
            {
                s = new MeshPartGeometryData.Source();
                s.Semantic = MeshPartGeometryData.Semantic.Texcoord;
                s.DataVector2 = textureCoordsReal.ToArray();
                meshPart.GeometryData.Sources.Add(s);
            }



            mesh.AddPart(meshPart);

            positionsReal = new List<Vector3>();
            normalsReal = new List<Vector3>();
            textureCoordsReal = new List<Vector2>();
        }


        //public static void TestEditorMeshPartRenderDataSimple()
        //{
        //    EditorMesh eMesh = new EditorMesh();
        //    ObjImporter objImporter = new ObjImporter();

        //    eMesh = objImporter.ImportObjFile(@"C:\The Wizards\TreeGenerater\TreeGenerator\bin\x86\Debug\testExportNoFlipNormals.obj");



        //    XNAGame game = new XNAGame();
        //    ColladaShader shader = null;
        //    EditorMeshPartRenderData renderData = null;



        //    game.InitializeEvent +=
        //        delegate
        //        {
        //            renderData = new EditorMeshPartRenderData(eMesh.CoreData.Parts[0].MeshPart as EditorMeshPart);
        //            renderData.Initialize(game);

        //            shader = new ColladaShader(game, null);

        //            shader.DiffuseColor = Color.Red.ToVector4();
        //            shader.SpecularColor = Color.Green.ToVector4();
        //            shader.LightDirection = new Vector3(0, 0, -1);
        //            shader.Shininess = 1;
        //        };

        //    game.DrawEvent +=
        //        delegate
        //        {
        //            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;
        //            shader.World = Matrix.Identity;
        //            shader.ViewInverse = game.Camera.ViewInverse;
        //            shader.ViewProjection = game.Camera.ViewProjection;

        //            shader.RenderPrimitiveSinglePass(renderData, Microsoft.Xna.Framework.Graphics.SaveStateMode.None);

        //        };

        //    game.Run();

        //}
        //public static void TestEditorMeshPartRenderDataComplex()
        //{
        //    EditorMesh eMesh = new EditorMesh();
        //    ObjImporter objImporter = new ObjImporter();

        //    eMesh = objImporter.ImportObjFile(@"C:\The Wizards\TreeGenerater\TreeGenerator\bin\x86\Debug\testExportNoFlipNormals.obj");



        //    XNAGame game = new XNAGame();
        //    game.SpectaterCamera.FarClip = 5000;
        //    ColladaShader shader = null;
        //    List<EditorMeshPartRenderData> renderData = new List<EditorMeshPartRenderData>();

        //    game.InitializeEvent +=
        //        delegate
        //        {
        //            for (int i = 0; i < eMesh.CoreData.Parts.Count; i++)
        //            {
        //                renderData.Add(new EditorMeshPartRenderData(eMesh.CoreData.Parts[i].MeshPart as EditorMeshPart));
        //                renderData[i].Initialize(game);

        //            }


        //            shader = new ColladaShader(game, null);

        //            shader.DiffuseColor = Color.Red.ToVector4();
        //            shader.SpecularColor = Color.Green.ToVector4();
        //            shader.LightDirection = new Vector3(0, 0, -1);
        //            shader.Shininess = 1;
        //        };

        //    game.DrawEvent +=
        //        delegate
        //        {
        //            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace;
        //            shader.World = Matrix.Identity;
        //            shader.ViewInverse = game.Camera.ViewInverse;
        //            shader.ViewProjection = game.Camera.ViewProjection;
        //            for (int i = 0; i < renderData.Count; i++)
        //            {
        //                shader.RenderPrimitiveSinglePass(renderData[i], Microsoft.Xna.Framework.Graphics.SaveStateMode.None);

        //            }
        //        };

        //    game.Run();

        //}

    }
}
