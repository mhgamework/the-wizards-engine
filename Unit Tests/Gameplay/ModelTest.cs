﻿using System.IO;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Gameplay
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
            var container = new ModelContainer.ModelContainer();
            TW.Model = container;

            var ent = new WorldRendering.Entity();

            ent.Mesh = new RAMMesh();

            int length;
            ModelContainer.ModelContainer.ObjectChange[] array;
            container.GetEntityChanges(out array, out length);




            Assert.AreEqual(1, length);
            Assert.AreEqual(ent, array[0].ModelObject);
        }

        [Test]
        public void TestWorldRenderer()
        {

            var game = new LocalGame();

            var ent = new WorldRendering.Entity();

            mesh = Sphere.CreateSphereMesh();
            ent.Mesh = mesh;

            var time = 0f;


            game
                .AddSimulator(new BasicSimulator(delegate
                                                 {
                                                     time += TW.Game.Elapsed;
                                                     ent.Mesh = mesh;
                                                     ent.WorldMatrix = Matrix.Translation(Vector3.UnitX * time);
                                                 }))
                .AddSimulator(new RenderingSimulator());



            game.Run();

        }

        [Test]
        public void TestEntityWithNoMesh()
        {

            var game = new LocalGame();

            new WorldRendering.Entity();

            game
                .AddSimulator(new RenderingSimulator());

            game.Run();

        }

        [Test]
        public void TestPlayerMovement()
        {
            var game = new LocalGame();

            var ent = new WorldRendering.Entity();
            ent.Mesh = mesh;

            var player = new PlayerData();
            player.Entity = ent;


            game
                .AddSimulator(new LocalPlayerSimulator(player))
                .AddSimulator(new ThirdPersonCameraSimulator())
                .AddSimulator(new RenderingSimulator());


            TW.Model.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.FirstPerson;
            TW.Model.GetSingleton<CameraInfo>().FirstPersonCameraTarget = player.Entity;

            game.Run();
        }

        [Test]
        public void TestSphere()
        {
            var game = new LocalGame();


            var sphere = new Sphere();

            game
                .AddSimulator(new PhysXUpdateSimulator())
                .AddSimulator(new RenderingSimulator());



            game.Run();
        }


    }
}
