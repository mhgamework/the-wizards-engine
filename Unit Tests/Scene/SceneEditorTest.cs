using System;
using System.Threading;
using System.Windows;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scene;
using MHGameWork.TheWizards.Tests.OBJParser;
using MHGameWork.TheWizards.Tests.Physics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using SceneEditor = MHGameWork.TheWizards.Scene.Editor.SceneEditor;

namespace MHGameWork.TheWizards.Tests.Scene
{
    [TestFixture]
    public class SceneEditorTest
    {
        [Test]
        public void TestSceneEditor()
        {
            SimpleEditorForm form = null;
            var game = new XNAGame();
            game.Mouse.CursorEnabled = true;
            game.IsMouseVisible = true;
            game.DrawFps = true;
            game.SpectaterCamera.Enabled = false;

            var physicsEngine = new PhysicsEngine();
            physicsEngine.Initialize();
            var physicsDebugRenderer = new PhysicsDebugRenderer(game, physicsEngine.Scene);
            game.AddXNAObject(physicsEngine);
            game.AddXNAObject(physicsDebugRenderer);
            var root =
                new ClientPhysicsQuadTreeNode(new Microsoft.Xna.Framework.BoundingBox(new Vector3(-2048, -4000, -2048),
                                                                                      new Vector3(2048, 4000, 2048)));
            QuadTree.Split(root, 7);

            var physicsFactory = new MeshPhysicsElementFactory(physicsEngine, root);
            game.AddXNAObject(physicsFactory);

            var renderer = Rendering.RenderingTest.InitDefaultMeshRenderer(game);
            var scene = new TheWizards.Scene.Scene(renderer, physicsFactory);
            var editor = new SceneEditor();

            var mesh =
                Rendering.RenderingTest.CreateGuildHouseMesh(
                    new OBJToRAMMeshConverter(new RAMTextureFactory()));
            var mesh2 =
             Rendering.RenderingTest.CreateMerchantsHouseMesh(
                 new OBJToRAMMeshConverter(new RAMTextureFactory()));

            var textureFactory = new RAMTextureFactory();
            var converter = new OBJToRAMMeshConverter(textureFactory);

            var mesh3 = OBJParserTest.GetBarrelMesh(converter);

            editor.PlaceModeMesh = mesh;
            editor.SetEditingScene(scene);
            game.AddXNAObject(editor);




            var shooter = new TestSphereShooter(game, physicsEngine, root, editor.EditorCamera);
            game.AddXNAObject(shooter);
            game.AddXNAObject(scene);

            var ev = new AutoResetEvent(false);
            var t = new Thread(delegate()
                                   {
                                       form = new SimpleEditorForm();
                                       var app = new Application();

                                       var vm = SimpleEditorViewModel.Create(game, editor, form);
                                       vm.PlaceMeshes.Clear();
                                       vm.PlaceMeshes.Add(new SimpleEditorViewModel.MeshItem { Name = "GuildHouse", Mesh = mesh });
                                       vm.PlaceMeshes.Add(new SimpleEditorViewModel.MeshItem { Name = "MerchantsHouse", Mesh = mesh2 });
                                       vm.PlaceMeshes.Add(new SimpleEditorViewModel.MeshItem { Name = "Barrel", Mesh = mesh3 });
                                       form.DataContext = vm;


                                       ev.Set();
                                       app.Run(form);
                                   });
            t.SetApartmentState(ApartmentState.STA);




            t.Start();
            game.InitializeEvent +=
                delegate
                {
                    int left = game.GetWindowForm().Location.X + game.GetWindowForm().Size.Width;
                    int top = game.GetWindowForm().Location.Y;
                    ev.WaitOne();
                    form.Dispatcher.Invoke(new Action(delegate
                                               {
                                                   form.Left = left + 8;
                                                   form.Top = top;
                                               }), null);

                };

            game.UpdateEvent += delegate
            {

            };

            game.Run();


        }


    }
}
