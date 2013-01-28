using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Default;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
{
    [TestFixture]
    public class RenderingTest
    {

        public static RAMMesh CreateMerchantsHouseMeshOLD()
        {
            ObjImporter importer = new ObjImporter();


            importer.AddMaterialFileStream("MerchantsHouse.mtl", new FileStream(TestFiles.MerchantsHouseMtl, FileMode.Open));
            importer.ImportObjFile(TestFiles.MerchantsHouseObj);

            var textureFactory = new RAMTextureFactory();
            /*textureFactory.AddAssemblyResolvePath(typeof(ObjImporter).Assembly,
                                                  "MHGameWork.TheWizards.OBJParser.Files.maps");*/
            var conv = new OBJToRAMMeshConverter(textureFactory);
            return conv.CreateMesh(importer);
        }
        public static RAMMesh CreateMerchantsHouseMesh(OBJToRAMMeshConverter c)
        {
            ObjImporter importer;
            importer = new ObjImporter();
            importer.AddMaterialFileStream("MerchantsHouse.mtl", File.OpenRead(TestFiles.MerchantsHouseMtl));
            importer.ImportObjFile(TestFiles.MerchantsHouseObj);

            return c.CreateMesh(importer);
        }
        public static RAMMesh CreateGuildHouseMesh(OBJToRAMMeshConverter c)
        {
            ObjImporter importer;
            importer = new ObjImporter();
            importer.AddMaterialFileStream("GuildHouse01.mtl", File.OpenRead(TestFiles.GuildHouseMtl));
            importer.ImportObjFile(TestFiles.GuildHouseObj);

            return c.CreateMesh(importer);
        }

        public static RAMMesh CreateMeshFromObj(OBJToRAMMeshConverter c, string obj, string mtl)
        {
            var fi = new FileInfo(mtl);
            ObjImporter importer;
            importer = new ObjImporter();
            importer.AddMaterialFileStream(fi.Name, File.OpenRead(mtl));
            importer.ImportObjFile(obj);

            return c.CreateMesh(importer);
        }




        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestLoadTexture()
        {
            XNAGame game = new XNAGame();
            TexturePool pool = new TexturePool();
            game.AddXNAObject(pool);
            game.DrawFps = true;

            RAMTexture tex = GetTestTexture();


            game.DrawEvent += delegate
            {
                //We should do this ACTUALLY in real usage situations, but it proves we cache the data.
                game.SpriteBatch.Begin();
                int row = 0;
                int col = 0;
                int width = 10;
                for (int i = 0; i < 100; i++)
                {
                    row = i / width;
                    col = i % width;
                    game.SpriteBatch.Draw(pool.LoadTexture(tex), new Rectangle(10 + col * 40, 10 + row * 40, 40, 40), Color.White);
                }

                game.SpriteBatch.End();

            };

            game.Run();
        }

        public static RAMTexture GetTestTexture()
        {
            var tex = new RAMTexture();

            var data = tex.GetCoreData();
            data.StorageType = TextureCoreData.TextureStorageType.Disk;
            data.DiskFilePath = TestFiles.BrickRoundJPG;
            /*data.StorageType = TextureCoreData.TextureStorageType.Assembly;
            data.Assembly = Assembly.GetExecutingAssembly();
            data.AssemblyResourceName = "MHGameWork.TheWizards.Tests.OBJParser.Files.maps.BrickRound0030_7_S.jpg";*/
            return tex;
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestMeshPartPool()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;

            var pool = new MeshPartPool();
            game.AddXNAObject(pool);

            RAMMeshPart meshPart = new RAMMeshPart();
            meshPart.SetGeometryData(MeshPartGeometryData.CreateTestSquare());

            DefaultModelShader shader = null;

            VertexDeclaration decl = null;

            game.InitializeEvent += delegate
            {
                decl = TangentVertex.CreateVertexDeclaration(game);
                shader = new DefaultModelShader(game, new EffectPool());

                shader.DiffuseColor = Color.Red.ToVector4();
                shader.Technique = DefaultModelShader.TechniqueType.Colored;


            };

            game.DrawEvent += delegate
            {
                game.GraphicsDevice.RenderState.CullMode = CullMode.None;
                game.GraphicsDevice.VertexDeclaration = decl;

                shader.ViewProjection = game.Camera.ViewProjection;

                shader.World = Matrix.CreateTranslation(Vector3.Right * 0 * 3) * Matrix.CreateScale(10);
                shader.DrawPrimitives(delegate
                {
                    var vb = pool.GetVertexBuffer(meshPart);
                    var ib = pool.GetIndexBuffer(meshPart);
                    var vertexCount =
                        meshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position).Length;

                    game.GraphicsDevice.Vertices[0].SetSource(vb, 0, TangentVertex.SizeInBytes);
                    game.GraphicsDevice.Indices = ib;
                    game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, vertexCount / 3);
                });


            };

            game.Run();

        }


        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestVertexDeclarationPool()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;

            var pool = new VertexDeclarationPool();

            pool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);


            game.AddXNAObject(pool);


            game.DrawEvent += delegate
                              {
                                  var decl1 = pool.GetVertexDeclaration<TangentVertex>();
                                  var decl2 = pool.GetVertexDeclaration<TangentVertex>();
                                  Assert.NotNull(decl1);
                                  Assert.AreEqual(decl1, decl2);
                                  game.Exit();
                              };

            game.Run();

        }



        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestLoadDefaultModelShader()
        {
            XNAGame game = new XNAGame();
            game.InitializeEvent += delegate
                                    {
                                        DefaultModelShader shader = new DefaultModelShader(game, new EffectPool());

                                    };

            game.DrawEvent += delegate
                              {
                                  Console.WriteLine("Shader loaded succesfully!");
                                  game.Exit();
                              };
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestRenderDefaultModelShader()
        {

            var shaders = new List<DefaultModelShader>();
            DefaultModelShader shader = null;
            VertexBuffer vb = null;

            VertexDeclaration decl = null;

            XNAGame game = new XNAGame();
            game.InitializeEvent += delegate
            {
                decl = TangentVertex.CreateVertexDeclaration(game);
                TangentVertex[] vertices = new TangentVertex[4];


                vertices[0] = new TangentVertex(Vector3.Zero, Vector2.Zero, Vector3.Up, Vector3.Zero);
                vertices[1] = new TangentVertex(Vector3.Forward, Vector2.UnitX, Vector3.Up, Vector3.Zero);
                vertices[2] = new TangentVertex(Vector3.Forward + Vector3.Right, Vector2.UnitX + Vector2.UnitY, Vector3.Up, Vector3.Zero);
                vertices[3] = new TangentVertex(Vector3.Right, Vector2.UnitY, Vector3.Up, Vector3.Zero);

                vb = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertices.Length, BufferUsage.None);

                vb.SetData(vertices);
                shader = new DefaultModelShader(game, new EffectPool());

                DefaultModelShader s;

                Texture2D tex = Texture2D.FromFile(game.GraphicsDevice, File.OpenRead(TestFiles.WoodPlanksBareJPG));


                s = shader.Clone();
                s.DiffuseColor = Color.Red.ToVector4();
                s.Technique = DefaultModelShader.TechniqueType.Colored;
                shaders.Add(s);

                s = shader.Clone();
                s.DiffuseTexture = tex;
                s.Technique = DefaultModelShader.TechniqueType.Textured;
                shaders.Add(s);


            };

            game.DrawEvent += delegate
                              {
                                  game.GraphicsDevice.RenderState.CullMode = CullMode.None;

                                  game.GraphicsDevice.VertexDeclaration = decl;

                                  shader.ViewProjection = game.Camera.ViewProjection;

                                  for (int i = 0; i < shaders.Count; i++)
                                  {
                                      //shaders[i].ViewProjection = game.Camera.ViewProjection;
                                      shaders[i].World = Matrix.CreateTranslation(Vector3.Right * i * 3) * Matrix.CreateScale(10);
                                      shaders[i].DrawPrimitives(delegate()
                                                                {
                                                                    game.GraphicsDevice.Vertices[0].SetSource(vb, 0,
                                                                                                              TangentVertex.SizeInBytes);
                                                                    game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                                                                }
                                          );


                                  }
                              };

            game.Run();
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestMeshRendererSimple()
        {
            var mesh = CreateSimpleTestMesh();

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);

            SimpleMeshRenderElement middle = null;

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {

                    var el = renderer.AddMesh(mesh);
                    el.WorldMatrix = Matrix.CreateTranslation(Vector3.Right * i * 2 + Vector3.UnitZ * j * 2);

                    if (i > 20 && i < 30 && j > 20 && j < 30)
                        el.Delete();
                }

            }


            var game = new XNAGame();
            game.DrawFps = true;

            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);

            game.Run();

        }



        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestMeshRendererAdvanced()
        {
            var texFactory = new RAMTextureFactory();
            var c = new OBJToRAMMeshConverter(texFactory);

            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Crate01.mtl", File.OpenRead(TestFiles.CrateMtl));
            importer.ImportObjFile(TestFiles.CrateObj);

            var mesh = c.CreateMesh(importer);

            RAMMesh mesh2 = CreateMerchantsHouseMesh(c);


            RAMMesh mesh3 = CreateGuildHouseMesh(c);

            SimpleMeshRenderer renderer = InitDefaultMeshRenderer(game);


            var el = renderer.AddMesh(mesh);
            el.WorldMatrix = Matrix.CreateTranslation(Vector3.Right * 0 * 2 + Vector3.UnitZ * 0 * 2);

            el = renderer.AddMesh(mesh2);
            el.WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 80));

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    el = renderer.AddMesh(mesh3);
                    el.WorldMatrix = Matrix.CreateTranslation(new Vector3(j * 30, 0, 70 + i * 30));
                }
            }




            game.Run();

        }

        public static SimpleMeshRenderer InitDefaultMeshRenderer(XNAGame game)
        {
            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);
            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);
            return renderer;
        }


        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestDefaultRendererRenderModel()
        {
            //TODO: maybe detach this from the defaultrenderer?


            DefaultRenderer renderer = new DefaultRenderer();


            TangentVertex[] vertices;
            short[] indices;
            var mat = new DefaultModelMaterialTextured();
            BoxMesh.CreateUnitBoxVerticesAndIndices(out vertices, out indices);



            var renderable = renderer.CreateModelRenderable(vertices, indices, mat);

            var el = renderer.CreateRenderElement(renderable);


            XNAGame game = new XNAGame();

            game.InitializeEvent += delegate
                                    {
                                        //TODO: make the renderer manage textures!!!

                                        using (var fs = new FileStream(TestFiles.WoodPlanksBareJPG, FileMode.Open, FileAccess.Read, FileShare.Read))
                                        {
                                            mat.DiffuseTexture = Texture2D.FromFile(game.GraphicsDevice, fs);
                                        }
                                    };

            game.AddXNAObject(renderer);

            game.Run();
        }



        [Test]
        public void TestDiskRenderingAssetFactory()
        {
            var factory = new DiskRenderingAssetFactory();
            factory.SaveDir = TWDir.Test.CreateSubdirectory("Rendering\\DiskFactory");
            var mesh = CreateGuildHouseMesh(new OBJToRAMMeshConverter(factory));
            factory.AddAsset(mesh);

            factory.SaveAllAssets();

            factory = new DiskRenderingAssetFactory();
            factory.SaveDir = TWDir.Test.CreateSubdirectory("Rendering\\DiskFactory");

            var loadMesh = factory.GetMesh(mesh.Guid);

        }


        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestSimpleRenderer()
        {
            XNAGame game = new XNAGame();
            SimpleRenderer renderer = new SimpleRenderer(game, new CullerNoCull());
            renderer.RenderCamera = game.SpectaterCamera;
            game.AddXNAObject(renderer);

            SimpleBoxMesh box = renderer.CreateBoxMesh();
            box.WorldMatrix = Matrix.CreateTranslation(new Vector3(5, 0, 5));

            SimplePlaneMesh plane = renderer.CreatePlaneMesh();


            game.InitializeEvent += delegate
                {
                    SimpleSphereMesh sphere = renderer.CreateSphereMesh();
                    sphere.WorldMatrix = Matrix.CreateTranslation(new Vector3(1, 0, -2));
                };

            game.Run();

        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestRendererFrustumCulling()
        {
            XNAGame game = new XNAGame();
            Vector3 radius = new Vector3(100, 1000, 100);
            FrustumCullerSimple culler = new FrustumCullerSimple(new BoundingBox(-radius, radius).dx(), 5);
            SimpleRenderer renderer = new SimpleRenderer(game, culler);
            game.AddXNAObject(renderer);

            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            QuadTreeVisualizerXNA visualizer = new QuadTreeVisualizerXNA();
            List<TestCullObject> cullObjects = new List<TestCullObject>();


            SpectaterCamera cullCam = new SpectaterCamera(game, 10f, 80);

            cullCam.Positie = new Vector3(8, 10, 8);
            cullCam.EnableUserInput = false;


            SpectaterCamera renderCam = game.SpectaterCamera;


            renderer.CullCamera = cullCam;
            renderer.RenderCamera = renderCam;

            bool rotate = true;
            int selectedNode = -1;



            Seeder seeder = new Seeder(2);

            for (int i = 0; i < 1000; i++)
            {
                Vector3 pos;
                pos.X = seeder.NextFloat(-90, 90);
                pos.Y = seeder.NextFloat(9, 11);
                pos.Z = seeder.NextFloat(-90, 90);

                float iRadius = seeder.NextFloat(0.3f, 2);

                if (seeder.NextInt(0, 2) == 0)
                {
                    SimpleBoxMesh mesh = renderer.CreateBoxMesh();
                    mesh.WorldMatrix = Matrix.CreateTranslation(pos);
                    mesh.Dimensions = Vector3.One * iRadius;
                    renderer.UpdateRenderable(mesh);
                }
                else
                {
                    SimpleSphereMesh mesh = renderer.CreateSphereMesh();
                    mesh.WorldMatrix = Matrix.CreateTranslation(pos);
                    mesh.Radius = iRadius;
                    renderer.UpdateRenderable(mesh);

                }

            }


            game.UpdateEvent +=
                delegate
                {
                    if (rotate)
                        cullCam.AngleHorizontal += game.Elapsed * MathHelper.Pi * (1 / 8f);

                    if (game.Keyboard.IsKeyPressed(Keys.Add))
                        selectedNode++;
                    if (game.Keyboard.IsKeyPressed(Keys.Subtract))
                        selectedNode--;

                    if (game.Keyboard.IsKeyPressed(Keys.Enter))
                    {
                        int count = -1;
                        visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                        delegate(FrustumCuller.CullNode node, out Color col)
                        {
                            col = Color.Red;
                            count++;
                            if (count == selectedNode)
                            {
                                node.Tag = "SELECTED!";
                            }
                            return count == selectedNode;
                        });
                    }

                    if (game.Keyboard.IsKeyPressed(Keys.NumPad0))
                        rotate = !rotate;

                };

            game.DrawEvent +=
                delegate
                {
                    game.LineManager3D.AddViewFrustum(new BoundingFrustum(cullCam.ViewProjection), Color.Black);
                    for (int i = 0; i < cullObjects.Count; i++)
                    {
                        game.LineManager3D.AddBox(cullObjects[i].BoundingBox, Color.Red);
                    }
                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                        delegate(FrustumCuller.CullNode node, out Color col)
                        {
                            if (culler.View.IsNodeVisible(node))
                            {
                                col = Color.Orange;
                            }
                            else
                            {
                                col = Color.Green;

                            }

                            return true;
                        });


                    /*int count = -1;
                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                    delegate(Culler.CullNode node, out Color col)
                    {
                        col = Color.Red;
                        count++;
                        return count == selectedNode;
                    });*/
                };



            game.Run();
        }

        public class TestCullObject : ICullable
        {
            private BoundingBox boundingBox;
            private int visibleReferenceCount;
            public int VisibleReferenceCount
            {
                get { return visibleReferenceCount; }
                set { visibleReferenceCount = value; }
            }

            public TestCullObject(Vector3 pos, float radius)
            {
                boundingBox = new BoundingBox(pos - Vector3.One * radius, pos + Vector3.One * radius);
            }

            public TestCullObject(BoundingBox bb)
            {
                boundingBox = bb;
            }

            #region ICullable Members

            public BoundingBox BoundingBox
            {
                get { return boundingBox; }
            }

            #endregion
        }



        public static IMesh CreateSimpleTestMesh()
        {
            IMesh mesh;

            mesh = new RAMMesh();

            var part = new MeshCoreData.Part();
            part.ObjectMatrix = Matrix.Identity;
            part.MeshPart = new RAMMeshPart();
            ((RAMMeshPart)part.MeshPart).SetGeometryData(MeshPartGeometryData.CreateTestSquare());

            var mat = new MeshCoreData.Material();

            mat.DiffuseMap = GetTestTexture();

            part.MeshMaterial = mat;
            mesh.GetCoreData().Parts.Add(part);

            return mesh;
        }
    }
}
