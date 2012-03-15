using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.Model.Simulation;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Simulation;
using MHGameWork.TheWizards.World.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Model
{
    [TestFixture]
    public class ModelTest
    {
        private RAMMesh mesh;

        [SetUp]
        public void Setup()
        {
            mesh = OBJParser.OBJParserTest.GetBarrelMesh(new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));
        }

        [Test]
        public void TestChangeEntity()
        {
            var container = new ModelContainer();

            var ent = new TheWizards.Model.Entity();
            container.AddObject(ent);

            ent.Mesh = new RAMMesh();

            int length;
            ModelContainer.ObjectChange[] array;
            container.GetEntityChanges(out array, out length);




            Assert.AreEqual(1, length);
            Assert.AreEqual(ent, array[0].ModelObject);
        }

        [Test]
        public void TestWorldRenderer()
        {

            var game = new LocalGame();

            var ent = new TheWizards.Model.Entity();
            TW.Model .AddObject(ent);

           
            ent.Mesh = mesh;

            var time = 0f;


            game
                .AddSimulator(new BasicSimulator(delegate
                                                     {
                                                         time += TW.Game.Elapsed;
                                                         ent.Mesh = mesh;
                                                         ent.WorldMatrix = Matrix.Translation(Vector3.UnitX * time);
                                                     }))
                .AddSimulator(new SimpleWorldRenderer());



            game.Run();

        }

        [Test]
        public void TestPlayerMovement()
        {
            var game = new LocalGame();

            var ent = new TheWizards.Model.Entity();
            ent.Mesh = mesh;
            TW.Model.AddObject(ent);

            var player = new PlayerData();
            TW.Model.AddObject(player);
            player.Entity = ent;

            game
                .AddSimulator(new LocalPlayer(player))
                .AddSimulator(new ThirdPersonCameraSimulator())
                .AddSimulator(new SimpleWorldRenderer());


            TW.Model.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.FirstPerson;


            game.Run();
        }
    }
}
