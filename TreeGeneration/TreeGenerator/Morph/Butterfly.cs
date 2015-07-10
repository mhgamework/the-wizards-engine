using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.EntityOud.Editor;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework;
using TreeGenerator.help;

using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;

namespace TreeGenerator.Morph
{
    public class ButterflyBuilder
    {
        private EditorMesh SingleButterfly;
        private Seeder seeder = new Seeder(486);
        EditorMeshPart meshPart = new EditorMeshPart();

        public EditorMesh CreateButterFliesMesh(Vector3 bottomLeftFrontCorner,Vector3 topRightBackCorner,int count,int seed,string filePath)
        {
            seeder = new Seeder(seed);
            ObjImporter objImporter = new ObjImporter();

            SingleButterfly = objImporter.ImportObjFile(filePath);
            EditorMesh butterfly=new EditorMesh();

            List<Vector3> position = new List<Vector3>();
            List<Vector3> position2 = new List<Vector3>();
            List<Vector3> normal = new List<Vector3>();
            List<Vector3> normal2 = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<Vector2> uv2 = new List<Vector2>();

            for (int i = 0; i < count; i++)
            {
                Vector3 transPosition=seeder.NextVector3(bottomLeftFrontCorner, topRightBackCorner);

                Vector3[] positionArray= ((EditorMeshPart) SingleButterfly.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
                Vector3[] positionArray2 =((EditorMeshPart) SingleButterfly.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
                Vector3[] normalArray= ((EditorMeshPart) SingleButterfly.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
                Vector3[] normalArray2 =((EditorMeshPart) SingleButterfly.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
                Vector2[] uvArray =((EditorMeshPart) SingleButterfly.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);
                Vector2[] uvArray2 =((EditorMeshPart) SingleButterfly.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);
                
                if (i%2==0)
                    {
                        for (int j = 0; j < positionArray.Length; j++)
                        {
                           


                                position.Add(positionArray[j] + transPosition);
                                position2.Add(positionArray2[j] + transPosition);
                                normal.Add(normalArray[j]);
                                normal2.Add(normalArray2[j]);
                                uv.Add(uvArray[j]);
                                uv2.Add(uvArray2[j]);
                           
                        }
            }
            else
            {
                for (int j = 0; j < positionArray.Length; j++)
                {
                    position.Add(positionArray2[j] + transPosition);
                    position2.Add(positionArray[j] + transPosition);
                    normal.Add(normalArray2[j]);
                    normal2.Add(normalArray[j]);
                    uv.Add(uvArray2[j]);
                    uv2.Add(uvArray[j]);
                }
            }


            }
            meshPart = new EditorMeshPart();
            MeshPartGeometryData.Source s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Position;
            s.DataVector3 = position.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Normal;
            s.DataVector3 = normal.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Texcoord;
            s.DataVector2 = uv.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            butterfly.AddPart(meshPart);

            meshPart = new EditorMeshPart();
            s = new MeshPartGeometryData.Source();
            s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Position;
            s.DataVector3 = position2.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Normal;
            s.DataVector3 = normal2.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Texcoord;
            s.DataVector2 = uv2.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            butterfly.AddPart(meshPart);

            position = null;
            position2 = null;
            normal = null;
            normal2 = null;
            uv = null;
            uv2 = null;
            return butterfly;

        }


        public static void TestButterflies()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            ButterflyBuilder builder = new ButterflyBuilder();
            MorphModel morph = new MorphModel();
            MorphModel morph2 = new MorphModel();
            

            game.InitializeEvent +=
                delegate
                {
                    morph.Initialize(game, builder.CreateButterFliesMesh(new Vector3(-45, 0, -45), new Vector3(100, 60, 100), 100000,12, game.RootDirectory.ToString() +"Morph\\butterfly.obj"), "Morph\\butterflyTexture\\butterfly4.png");
                    morph2.Initialize(game, builder.CreateButterFliesMesh(new Vector3(-45, 0, -45), new Vector3(100, 60,100), 100000,4698, game.RootDirectory.ToString() + "Morph\\butterfly.obj"), "Morph\\butterflyTexture\\transparentButterfly.png");
                    morph2.SetStartLerpValue(0.5f);

                };
            float angle=0;
            Matrix mat = new Matrix();
            Matrix mat2 = new Matrix();
            game.UpdateEvent +=
                delegate
                {
                    //group movement
                    Vector3 transPosition = new Vector3(0, 0, 100);
                    angle +=game.Elapsed/30f;
                    if (angle==MathHelper.TwoPi)
                    {
                        angle = 0;
                    }
                    mat = Matrix.CreateTranslation(transPosition);
                    Vector3 pos = new Vector3(transPosition.X, transPosition.Y+(float)Math.Sin((double)angle*6)*20, transPosition.Z);
                    mat2 = Matrix.CreateTranslation(pos);


                    mat*=Matrix.CreateRotationY(angle);
                    mat2 *= Matrix.CreateRotationY(angle);

                    

                 
                    morph.SetWorldMatrix(mat);
                    morph2.SetWorldMatrix(mat);

                    //flapping of the wings
                    morph.Update(10);
                    morph2.Update(10);
                  

                };

            game.DrawEvent +=
                delegate
                {

                 

                    morph.Render();
                    morph2.Render();
                };
            game.Run();
        }
    }
}
