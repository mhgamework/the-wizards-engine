using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Rendering;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Scattered.Simulation.Constructions;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using MHGameWork.TheWizards.Scattered.Simulation.Sandbox;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class FlightTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private Island cameraIsland;

        [Test]
        public void TestSingleIsland()
        {
            var level = setupRendering();

            var isl = level.CreateNewIsland(new Vector3(5, 3, 5));
            cameraIsland = isl;
        }
        [Test]
        public void TestCluster()
        {
            var level = setupRendering();

            var isl = level.CreateNewIsland(new Vector3(5, 3, 5));
            cameraIsland = isl;

            var isl2 = level.CreateNewIsland(new Vector3(15, 3, 5));
            isl.AddBridgeTo(isl2);
        }

        [Test]
        public void TestClusterBig()
        {
            var level = setupRendering();

            var isl = level.CreateNewIsland(new Vector3(5, 3, 5));
            cameraIsland = isl;

            var isl2 = level.CreateNewIsland(new Vector3(15, 3, 5));
            isl.AddBridgeTo(isl2);

            var isl3 = level.CreateNewIsland(new Vector3(25, 3, 5));
            isl3.AddBridgeTo(isl2);

            var isl4 = level.CreateNewIsland(new Vector3(5, 3, 15));
            isl4.AddBridgeTo(isl);

            var isl5 = level.CreateNewIsland(new Vector3(15, 3, 15));
            isl5.AddBridgeTo(isl2);
            isl5.AddBridgeTo(isl4);
        }

        private Level setupRendering()
        {
            var constructionFactory = new ConstructionFactory(new Lazy<DistributionHelper>(() => null),
                                                              new Lazy<RoundState>(() => null));
            var level = new Level(constructionFactory);


            var cam = TW.Data.Get<CameraInfo>();
            cam.Mode = CameraInfo.CameraMode.ThirdPerson;

            var camEntity = new Entity();
            cam.FirstPersonCameraTarget = camEntity;


            var controller = new ClusterFlightController(TW.Graphics.Keyboard);

            engine.AddSimulator(new BasicSimulator(() =>
                {
                    controller.SimulateFlightStep(cameraIsland);
                    camEntity.WorldMatrix =
                        Matrix.Translation(
                            cameraIsland.GetIslandsInCluster().Aggregate(new Vector3(), (acc, el) => acc + el.Position)/
                            cameraIsland.GetIslandsInCluster().Count());
                }));
            engine.AddSimulator(new ClusterPhysicsSimulator(level));

            engine.AddSimulator(new ThirdPersonCameraSimulator());
            engine.AddSimulator(new LevelRenderer(level));
            engine.AddSimulator(new WorldRenderingSimulator());
            return level;
        }
    }
}