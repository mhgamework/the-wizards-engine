using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MHGameWork.TheWizards.Graphics;
using TreeGenerator.AtlasTool;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Entity.Editor;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.Morph;
using TreeGenerator.Clouds;
using TreeGenerator.TreeEngine;

namespace TreeGenerator
{
    [TestFixture]
    public class SideProjectTests
    {
       
        [Test]
        public  void TestTextureAtlasCreater()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.RenderAxis = false;


            TextureAtlasCreater atlas = new TextureAtlasCreater();
            game.InitializeEvent +=
                delegate
                {
                    game.SpectaterCamera.Enabled = false;
                    game.Mouse.CursorEnabled = true;
                    game.IsMouseVisible = true;
                    Cursor cursor = new Cursor(game, new GameFile(game.Content.RootDirectory + @"Content\Cursor001.dds"), Vector2.Zero);
                    game.Cursor = cursor;
                    atlas.Initialize(game);
                };
            game.UpdateEvent +=
                delegate
                {
                    atlas.Update();

                };
            game.DrawEvent +=
                delegate
                {

                    atlas.Draw();

                };
            game.Run();
        }
        //[Test]
        //public  void TestEditorMeshPartRenderDataSimple()
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
        //[Test]
        //public  void TestEditorMeshPartRenderDataComplex()
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
        [Test]
        public  void TestMorphModel()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            MorphModel morph = new MorphModel();

            game.InitializeEvent +=
                delegate
                {
                    morph.Initialize(game);
                };

            game.UpdateEvent +=
                delegate
                {
                    morph.Update(1);
                };

            game.DrawEvent +=
                delegate
                {

                    game.LineManager3D.AddCenteredBox(morph.GetPosition()[0], 4, Color.Black);
                    game.LineManager3D.AddCenteredBox(morph.GetPosition2()[0], 4, Color.Black);


                    morph.Render();
                };
            game.Run();
        }
        [Test]
        public  void TestButterflies()
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
                    morph.Initialize(game, builder.CreateButterFliesMesh(new Vector3(-45, 0, -45), new Vector3(100, 60, 100), 100000, 12, game.RootDirectory.ToString() + "Morph\\butterfly.obj"), "Morph\\butterflyTexture\\butterfly4.png");
                    morph2.Initialize(game, builder.CreateButterFliesMesh(new Vector3(-45, 0, -45), new Vector3(100, 60, 100), 100000, 4698, game.RootDirectory.ToString() + "Morph\\butterfly.obj"), "Morph\\butterflyTexture\\transparentButterfly.png");
                    morph2.SetStartLerpValue(0.5f);

                };
            float angle = 0;
            Matrix mat = new Matrix();
            Matrix mat2 = new Matrix();
            game.UpdateEvent +=
                delegate
                {
                    //group movement
                    Vector3 transPosition = new Vector3(0, 0, 100);
                    angle += game.Elapsed / 30f;
                    if (angle == MathHelper.TwoPi)
                    {
                        angle = 0;
                    }
                    mat = Matrix.CreateTranslation(transPosition);
                    Vector3 pos = new Vector3(transPosition.X, transPosition.Y + (float)Math.Sin((double)angle * 6) * 20, transPosition.Z);
                    mat2 = Matrix.CreateTranslation(pos);


                    mat *= Matrix.CreateRotationY(angle);
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
        [Test]
        public  void TestImposterRing()
        {
            ImposterRing.ImposterRing.TestImposterRing();
        }
        [Test]
        public  void UberDuberTestImposterRing()
        {
            //ImposterRing.ImposterRing.UberDuberTestImposterRing();
        }
        [Test]
        public  void TestEditorMeshPartRenderDataSimple2()
        {
            ObjExporter.TestEditorMeshPartRenderDataSimple();
        }
        [Test]
        public void TestCreateVertices()
        {
            BillBoards.TestCreateVertices();
        }
        [Test]
        public void TestVolumetricLeaf()
        {
            TestVolumetricLeafShader.TestLeaf();
        }
    }

}
