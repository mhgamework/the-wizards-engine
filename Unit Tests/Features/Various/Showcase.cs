using System.Collections.Generic;
using System.IO;
using System.Threading;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;

using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Features.Data.OBJParser;
using MHGameWork.TheWizards.Tests.Features.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11;
using MHGameWork.TheWizards.Tests.Features.Simulation.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;
using MathHelper = DirectX11.MathHelper;
using Matrix = Microsoft.Xna.Framework.Matrix;
using TexturePool = MHGameWork.TheWizards.Rendering.TexturePool;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace MHGameWork.TheWizards.Tests.Features.Various
{
    [TestFixture]
    public class Showcase
    {
        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestOBJToRAMMeshConverterPerObjectVisualCool()
        {
            var textureFactory = new RAMTextureFactory();
            var c = new OBJToRAMMeshConverter(textureFactory);


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Town001.mtl", new FileStream("../GameData/Town/OBJ03/Town001.mtl", FileMode.Open));
            importer.ImportObjFile("../GameData/Town/OBJ03/Town001.obj");

            var meshes = c.CreateMeshesFromObjects(importer);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);





            var spheres = new List<ClientPhysicsTestSphere>();
            var engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;

            var root = PhysicsMeshTest.CreatePhysicsQuadtree(20, 5);

            var physicsElementFactoryXNA = new MeshPhysicsFactoryXNA(engine, root);
            var physicsElementFactory = physicsElementFactoryXNA.Factory;

            var physicsElements = new List<MeshStaticPhysicsElement>();
            for (int i = 0; i < 0 * 100 + 1 * meshes.Count; i++)
            {
                var mesh = meshes[i];
                var el = renderer.AddMesh(mesh);
                el.WorldMatrix = Matrix.CreateTranslation(Vector3.Right * 0 * 2 + Vector3.UnitZ * 0 * 2);

                var pEl = physicsElementFactory.CreateStaticElement(mesh, Matrix.Identity);
                physicsElements.Add(pEl);

            }
            var gameMeshes = new List<OBJParserTest.TestGameMesh>();

            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;
            game.SpectaterCamera.FarClip = 5000;
            game.Graphics1.PreparingDeviceSettings += delegate(object sender, PreparingDeviceSettingsEventArgs e)
            {
                DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
                e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
                e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;
                game.SpectaterCamera.AspectRatio = displayMode.Width / (float)displayMode.Height;

            };
            game.Graphics1.ToggleFullScreen();

            var barrelMesh = OBJParserTest.GetBarrelMesh(c);
            var crateMesh = OBJParserTest.GetCrateMesh(c);

            var sphereMesh = new SphereMesh(0.3f, 20, Color.Green);
            var visualizer = new QuadTreeVisualizerXNA();

            game.AddXNAObject(physicsElementFactoryXNA);

            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);


            game.InitializeEvent += delegate
            {
                engine.Initialize();
                debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                debugRenderer.Initialize(game);
                sphereMesh.Initialize(game);

                for (int i = 0; i < meshes.Count; i++)
                {
                    var mesh = meshes[i];
                    var data = mesh.GetCollisionData();
                    /*if (data.TriangleMesh != null)
                        physicsElementFactory.MeshPhysicsPool.PreloadTriangleMesh(engine.Scene, data.TriangleMesh);*/
                }



            };

            bool showPhysics = true;
            game.DrawEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Keys.P))
                    showPhysics = !showPhysics;
                if (showPhysics)
                    debugRenderer.Render(game);
                /*visualizer.RenderNodeGroundBoundig(game, root,
                    delegate(ClientPhysicsQuadTreeNode node, out Color col)
                    {
                        col = Color.Green;

                        return node.PhysicsObjects.Count == 0;
                    });

                visualizer.RenderNodeGroundBoundig(game, root,
                   delegate(ClientPhysicsQuadTreeNode node, out Color col)
                   {
                       col = Color.Orange;

                       return node.PhysicsObjects.Count > 0;
                   });*/

                for (int i = 0; i < physicsElements.Count; i++)
                {
                    var el = physicsElements[i];
                    //game.LineManager3D.AddBox(BoundingBox.CreateFromSphere( el.BoundingSphere), Color.Orange);
                }
                for (int i = 0; i < spheres.Count; i++)
                {
                    sphereMesh.WorldMatrix = Matrix.CreateTranslation(spheres[i].Center);
                    sphereMesh.Render(game);
                }

            };
            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
                sphereMesh.Update(game);
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var pEl = physicsElementFactory.CreateDynamicElement(crateMesh,
                                                                         Matrix.CreateTranslation(
                                                                             game.SpectaterCamera.CameraPosition +
                                                                             game.SpectaterCamera.CameraDirection));
                    pEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 30;

                    var rEl = renderer.AddMesh(crateMesh);


                    gameMeshes.Add(new OBJParserTest.TestGameMesh(rEl, pEl));
                }
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.E))
                {
                    var pEl = physicsElementFactory.CreateDynamicElement(barrelMesh,
                                                                         Matrix.CreateTranslation(
                                                                             game.SpectaterCamera.CameraPosition +
                                                                             game.SpectaterCamera.CameraDirection));
                    pEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 30;

                    var rEl = renderer.AddMesh(barrelMesh);


                    gameMeshes.Add(new OBJParserTest.TestGameMesh(rEl, pEl));
                }


                for (int i = 0; i < gameMeshes.Count; i++)
                {
                    var m = gameMeshes[i];
                    m.RenderElement.WorldMatrix = m.PhysicsElement.World;
                }

            };

            game.Run();

        }


        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestThrowCrates()
        {
            var texFactory = new RAMTextureFactory();
            var c = new OBJToRAMMeshConverter(texFactory);


            RAMMesh mesh = OBJParserTest.GetCrateMesh(c);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);





            var gameMeshes = new List<OBJParserTest.TestGameMesh>();
            var engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;

            var root = PhysicsMeshTest.CreatePhysicsQuadtree(16, 4);

            var physicsElementFactoryXNA = new MeshPhysicsFactoryXNA(engine, root);
            var physicsElementFactory = physicsElementFactoryXNA.Factory;

            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;

            var visualizer = new QuadTreeVisualizerXNA();

            game.AddXNAObject(physicsElementFactoryXNA);

            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);


            game.InitializeEvent += delegate
            {
                engine.Initialize();
                debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                debugRenderer.Initialize(game);



            };

            bool showPhysics = true;
            game.DrawEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Keys.P))
                    showPhysics = !showPhysics;
                if (showPhysics)
                    debugRenderer.Render(game);
                visualizer.RenderNodeGroundBoundig(game, root,
                    delegate(ClientPhysicsQuadTreeNode node, out Color col)
                    {
                        col = Color.Green;

                        return node.PhysicsObjects.Count == 0;
                    });

                visualizer.RenderNodeGroundBoundig(game, root,
                   delegate(ClientPhysicsQuadTreeNode node, out Color col)
                   {
                       col = Color.Orange;

                       return node.PhysicsObjects.Count > 0;
                   });


            };
            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var pEl = physicsElementFactory.CreateDynamicElement(mesh,
                                                                         Matrix.CreateTranslation(
                                                                             game.SpectaterCamera.CameraPosition +
                                                                             game.SpectaterCamera.CameraDirection));
                    pEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 30;

                    var rEl = renderer.AddMesh(mesh);


                    gameMeshes.Add(new OBJParserTest.TestGameMesh(rEl, pEl));
                }

                for (int i = 0; i < gameMeshes.Count; i++)
                {
                    var m = gameMeshes[i];
                    m.RenderElement.WorldMatrix = m.PhysicsElement.World;
                }

            };

            game.Run();

        }
        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestThrowBarrels()
        {
            var texFactory = new RAMTextureFactory();
            var c = new OBJToRAMMeshConverter(texFactory);


            RAMMesh mesh = OBJParserTest.GetBarrelMesh(c);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);





            var gameMeshes = new List<OBJParserTest.TestGameMesh>();
            var engine = new PhysicsEngine();
            PhysicsDebugRendererXNA debugRenderer = null;

            var root = PhysicsMeshTest.CreatePhysicsQuadtree(16, 4);

            var physicsElementFactoryXNA = new MeshPhysicsFactoryXNA(engine, root);
            var physicsElementFactory = physicsElementFactoryXNA.Factory;

            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;

            var visualizer = new QuadTreeVisualizerXNA();

            game.AddXNAObject(physicsElementFactoryXNA);

            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);


            game.InitializeEvent += delegate
            {
                engine.Initialize();
                debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);
                debugRenderer.Initialize(game);



            };

            bool showPhysics = true;
            game.DrawEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Keys.P))
                    showPhysics = !showPhysics;
                if (showPhysics)
                    debugRenderer.Render(game);
                visualizer.RenderNodeGroundBoundig(game, root,
                    delegate(ClientPhysicsQuadTreeNode node, out Color col)
                    {
                        col = Color.Green;

                        return node.PhysicsObjects.Count == 0;
                    });

                visualizer.RenderNodeGroundBoundig(game, root,
                   delegate(ClientPhysicsQuadTreeNode node, out Color col)
                   {
                       col = Color.Orange;

                       return node.PhysicsObjects.Count > 0;
                   });


            };
            game.UpdateEvent += delegate
            {
                engine.Update(game.Elapsed);
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var pEl = physicsElementFactory.CreateDynamicElement(mesh,
                                                                         Matrix.CreateTranslation(
                                                                             game.SpectaterCamera.CameraPosition +
                                                                             game.SpectaterCamera.CameraDirection));
                    pEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 30;

                    var rEl = renderer.AddMesh(mesh);


                    gameMeshes.Add(new OBJParserTest.TestGameMesh(rEl, pEl));
                }

                for (int i = 0; i < gameMeshes.Count; i++)
                {
                    var m = gameMeshes[i];
                    m.RenderElement.WorldMatrix = m.PhysicsElement.World;
                }

            };

            game.Run();

        }


        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestMeshRendererRenderCity()
        {
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Town001.mtl", new FileStream("../../bin/GameData/Core/Town/OBJ03/Town001.mtl", FileMode.Open));
            importer.ImportObjFile("../../bin/GameData/Core/Town/OBJ03/Town001.obj");

            var mesh = c.CreateMesh(importer);

            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);


            var el = renderer.AddMesh(mesh);
            el.WorldMatrix = Matrix.CreateTranslation(Vector3.Right * 0 * 2 + Vector3.UnitZ * 0 * 2);


            var game = new XNAGame();
            game.IsFixedTimeStep = false;
            game.DrawFps = true;
            game.Graphics1.PreparingDeviceSettings += delegate(object sender, PreparingDeviceSettingsEventArgs e)
            {
                DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
                e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
                e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;
                game.SpectaterCamera.AspectRatio = displayMode.Width / (float)displayMode.Height;

            };
            game.Graphics1.ToggleFullScreen();



            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);

            game.Run();

        }


        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestDeferredMeshRendererRenderCity()
        {
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Town001.mtl", new FileStream("../../bin/GameData/Core/Town/OBJ03/Town001.mtl", FileMode.Open));
            importer.ImportObjFile("../../bin/GameData/Core/Town/OBJ03/Town001.obj");

            var mesh = c.CreateMesh(importer);

            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;


            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshesRenderer(game, gBuffer, texturePool);



            var el = renderer.AddMesh(mesh);
            el.WorldMatrix = SlimDX.Matrix.Translation(MathHelper.Right * 0 * 2 + SlimDX.Vector3.UnitZ * 0 * 2);


            game.GameLoopEvent += delegate
                                  {





                                      gBuffer.Clear();
                                      gBuffer.SetTargetsToOutputMerger();

                                      renderer.Draw();

                                      context.ClearState();
                                      game.SetBackbuffer();

                                      DeferredTest.DrawGBuffer(game, gBuffer);

                                  };
            SlimDX.Configuration.EnableObjectTracking = false;

            game.Run();

        }


        [Test]
        public void TestDeferredRendererCity()
        {
            var game = new DX11Game();
            game.InitDirectX();
            SlimDX.Configuration.EnableObjectTracking = false;

            var renderer = new DeferredRenderer(game);

            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Town001.mtl", new FileStream("../../bin/GameData/Core/Town/OBJ03/Town001.mtl", FileMode.Open));
            importer.ImportObjFile("../../bin/GameData/Core/Town/OBJ03/Town001.obj");

            var meshes = c.CreateMeshesFromObjects(importer);

            for (int index = 0; index < meshes.Count; index++)
            {
                var ramMesh = meshes[index];
                var el = renderer.CreateMeshElement(ramMesh);

            }
            var directional = renderer.CreateDirectionalLight();
            directional.ShadowsEnabled = true;
            var point = renderer.CreatePointLight();
            point.LightRadius *= 2;
            point.ShadowsEnabled = true;
            var spot = renderer.CreateSpotLight();
            spot.LightRadius *= 2;
            spot.ShadowsEnabled = true;

            var visualizer = new QuadTreeVisualizer();

            var otherCam = new SpectaterCamera(game.Keyboard, game.Mouse, 1, 10000);
            var camState = false;
            int state = 0;
            bool rotate = false;
            game.GameLoopEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Key.D1))
                    state = 0;
                if (game.Keyboard.IsKeyPressed(Key.D2))
                    state = 1;
                if (game.Keyboard.IsKeyPressed(Key.D3))
                    state = 2;
                if (game.Keyboard.IsKeyPressed(Key.D4))
                    state = 3;

                if (game.Keyboard.IsKeyPressed(Key.C))
                    camState = !camState;

                if (game.Keyboard.IsKeyPressed(Key.NumberPad0))
                    rotate = !rotate;

                if (rotate)
                    game.SpectaterCamera.AngleHorizontal += game.Elapsed * MathHelper.Pi * (1 / 8f);

                switch (state)
                {
                    case 0:
                        break;
                    case 1:
                        directional.LightDirection = game.SpectaterCamera.CameraDirection;
                        break;
                    case 2:
                        point.LightPosition = game.SpectaterCamera.CameraPosition;

                        break;
                    case 3:
                        spot.LightPosition = game.SpectaterCamera.CameraPosition;
                        spot.SpotDirection = game.SpectaterCamera.CameraDirection;
                        break;
                }

                if (camState)
                {
                    game.Camera = game.SpectaterCamera;
                    renderer.DEBUG_SeperateCullCamera = null;
                }
                else
                {
                    game.Camera = otherCam;
                    renderer.DEBUG_SeperateCullCamera = game.SpectaterCamera;


                }
                game.SpectaterCamera.EnableUserInput = camState;
                otherCam.EnableUserInput = !camState;
                otherCam.Update(game.Elapsed);


                renderer.Draw();


                if (false)
                {
                    //renderer.DEBUG_FrustumCuller.CullCamera = game.Camera;
                    //renderer.DEBUG_FrustumCuller.UpdateVisibility();
                    //for (int i = 0; i < renderer.DebugMeshesRenderer.Elements.Count; i++)
                    //{
                    //    var el = renderer.DebugMeshesRenderer.Elements[i];

                    //    game.LineManager3D.AddBox(el.BoundingBox.dx(), new SlimDX.Color4(0, 1, 0));
                    //}

                    //visualizer.RenderNodeGroundBoundig(game, renderer.DEBUG_FrustumCuller.RootNode);
                    visualizer.RenderNodeGroundBoundig(game, renderer.DEBUG_FrustumCuller.RootNode,
                                                   delegate(FrustumCuller.CullNode quadTreeNode, out Color4 color4)
                                                       {
                                                           if (point.Views[2].IsNodeVisible(quadTreeNode))
                                                               color4 = new Color4(1, 0, 0);
                                                           else
                                                               color4 = new Color4(0, 1, 0);
                                                           return true;
                                                       });
                }

                game.LineManager3D.AddViewFrustum(game.SpectaterCamera.ViewProjection, new SlimDX.Color4(1, 0, 0));



            };


            game.Run();
        }
    }
}
