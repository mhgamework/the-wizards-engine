using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.Model.Synchronization;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.World.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Model
{
    [TestFixture]
    public class VirtualServerClientSyncing
    {
        private DX11Game game;
        private ModelContainer clientContainer;
        private ModelContainer serverContainer;
        private RAMMesh mesh;
        private float elapsed;

        private Action GameLoopAction;

        [SetUp]
        public void Init()
        {
            game = new DX11Game();
            game.InitDirectX();


            clientContainer = new ModelContainer();
            serverContainer = new ModelContainer();

            mesh = OBJParser.OBJParserTest.GetBarrelMesh(new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));

            var deferred = new DeferredRenderer(game);
            var renderer = new WorldRenderer(clientContainer, deferred);

            var light = deferred.CreateDirectionalLight();
            light.LightDirection = Vector3.Normalize(new Vector3(1, -1, 1));
            light.ShadowsEnabled = true;


            var serverSyncer = new VirtualModelSyncer(serverContainer);
            var clientSyncer = new VirtualModelSyncer(clientContainer);
            serverSyncer.setEndpoint(clientSyncer);
            clientSyncer.setEndpoint(serverSyncer);





            elapsed = 0f;
            game.GameLoopEvent += delegate
            {
                elapsed += game.Elapsed;
                GameLoopAction();

                clientSyncer.SendLocalChanges();
                serverSyncer.SendLocalChanges();

                renderer.ProcessWorldChanges();
                
                clientContainer.ClearDirty();
                serverContainer.ClearDirty();

                deferred.Draw();
            };
        }

        [Test]
        public void TestBasicClientRendering()
        {
            var ent = new TheWizards.Model.Entity();
            clientContainer.AddObject(ent);

            GameLoopAction = delegate
                             {
                                 ent.Mesh = mesh;
                                 ent.WorldMatrix = Matrix.Translation(Vector3.UnitX * elapsed);
                             };

            game.Run();
        }

        [Test]
        public void TestServerCreateModify()
        {
            var ent = new TheWizards.Model.Entity();
            serverContainer.AddObject(ent);

            GameLoopAction = delegate
            {
                ent.Mesh = mesh;
                ent.WorldMatrix = Matrix.Translation(Vector3.UnitX * elapsed);
            };

            game.Run();
        }


    }
}
