using System.IO;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Simulation;
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

            var ent = new TheWizards.Model.Entity();
            container.AddObject(ent);

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

            var ent = new TheWizards.Model.Entity();
            TW.Model .AddObject(ent);

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
                .AddSimulator(new LocalPlayerSimulator(player))
                .AddSimulator(new ThirdPersonCameraSimulator())
                .AddSimulator(new SimpleWorldRenderer());


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
                .AddSimulator(new SimpleWorldRenderer());



            game.Run();
        }


    }
}
