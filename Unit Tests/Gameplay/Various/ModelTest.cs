using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Tests.Features.Data.OBJParser;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Gameplay.Various
{
    /// <summary>
    /// TODO: these tests smear across layers and should be fixed
    /// </summary>
    [TestFixture]
    public class ModelTest
    {
        private RAMMesh mesh;

        [SetUp]
        public void Setup()
        {
            mesh = OBJParserTest.GetBarrelMesh(new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));
        }

        [Test]
        public void TestChangeEntity()
        {
            var container = new Data.ModelContainer();
            SimpleModelObject.CurrentModelContainer = new Data.ModelContainer();

            var ent = new Engine.WorldRendering.Entity();

            ent.Mesh = new RAMMesh();

            int length;
            Data.ModelContainer.ObjectChange[] array;
            container.GetObjectChanges(out array, out length);




            Assert.AreEqual(1, length);
            Assert.AreEqual(ent, array[0].ModelObject);
        }

        [Test]
        public void TestWorldRenderer()
        {

            var game = new LocalGame();

            var ent = new Engine.WorldRendering.Entity();

            var mBuilder = new MeshBuilder();
            mBuilder.AddSphere(12, 1);
            ent.Mesh = mBuilder.CreateMesh();

            var time = 0f;


            game
                .AddSimulator(new BasicSimulator(delegate
                                                 {
                                                     time += TW.Graphics.Elapsed;
                                                     ent.Mesh = mesh;
                                                     ent.WorldMatrix = Matrix.Translation(Vector3.UnitX * time);
                                                 }))
                .AddSimulator(new WorldRenderingSimulator());



            game.Run();

        }

        [Test]
        public void TestEntityWithNoMesh()
        {

            var game = new LocalGame();

            new Engine.WorldRendering.Entity();

            game
                .AddSimulator(new WorldRenderingSimulator());

            game.Run();

        }

        [Test]
        public void TestPlayerMovement()
        {
            var game = new LocalGame();

            var ent = new Engine.WorldRendering.Entity();
            ent.Mesh = mesh;

            var ent2 = new Engine.WorldRendering.Entity();
            ent2.Mesh = mesh;
            ent2.WorldMatrix = Matrix.Translation(5, 0, 0);

            var player = new PlayerData();
            player.Entity = ent;

            player.GroundHeight = 5;
            player.DisableGravity = true;


            game
                .AddSimulator(new LocalPlayerSimulator(player))
                .AddSimulator(new ThirdPersonCameraSimulator())
                .AddSimulator(new WorldRenderingSimulator());


            TW.Data.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.ThirdPerson;
            TW.Data.GetSingleton<CameraInfo>().FirstPersonCameraTarget = player.Entity;

            game.Run();
        }

        [Test]
        public void TestSphere()
        {
            var game = new LocalGame();


            var sphere = new Sphere();

            game
                .AddSimulator(new PhysXSimulator())
                .AddSimulator(new WorldRenderingSimulator());



            game.Run();
        }


    }
}
