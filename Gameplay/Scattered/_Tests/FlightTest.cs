using System;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Core;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Simulation;
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

        [Test]
        public void TestBridgeConnecting()
        {
            var level = setupRendering();

            var isl = level.CreateNewIsland(new Vector3(5, 3, 5));
            cameraIsland = isl;

            var isl2 = level.CreateNewIsland(new Vector3(15, 3, 5));

            addBridge(isl, Vector3.UnitX, new Vector3(3, 0, 0));
            addBridge(isl, Vector3.UnitZ, new Vector3(0, 0, 3));
            addBridge(isl2, -Vector3.UnitX, new Vector3(-3, 0, 0));


        }

        [Test]
        public void TestMultipleConnector()
        {
            var level = setupRendering();

            var isl = level.CreateNewIsland(new Vector3(5, 3, 5));
            cameraIsland = isl;

            var isl2 = level.CreateNewIsland(new Vector3(15, 3, 5));


            addBridge(isl, Vector3.UnitX, new Vector3(3, 0, 0));
            addBridge(isl, Vector3.UnitZ, new Vector3(0, 0, 3));
            addBridge(isl2, -Vector3.UnitX, new Vector3(-3, 0, 0));

            isl = level.CreateNewIsland(new Vector3(5, 3, 20));


            addBridge(isl, Vector3.UnitX, new Vector3(3, 0, 0));
            addBridge(isl, Vector3.UnitZ, new Vector3(0, 0, 3));

        }


        [Test]
        public void TestAutoConnector()
        {
            var level = createLevel();
            var isl = level.CreateNewIsland(new Vector3(5, 3, 5));
            cameraIsland = isl;

            var isl2 = level.CreateNewIsland(new Vector3(15, 3, 5));

            var a = addBridge(isl, Vector3.UnitX, new Vector3(3, 0, 0));
            var c = addBridge(isl, Vector3.UnitZ, new Vector3(0, 0, 3));
            var b = addBridge(isl2, -Vector3.UnitX, new Vector3(-3, 0, 0));


            var flight = new ClusterFlightController(TW.Graphics.Keyboard);
            Assert.False(flight.CanAutoDock(a, b));
            Assert.False(flight.CanAutoDock(a, c));
            Assert.False(flight.CanAutoDock(c, b));

            isl.Position = new Vector3(9, 3, 5);

            Assert.True(flight.CanAutoDock(a, b));
            Assert.False(flight.CanAutoDock(a, c));
            Assert.False(flight.CanAutoDock(c, b));

            isl.Node.Relative *= Matrix.RotationY(MathHelper.PiOver2);

            Assert.False(flight.CanAutoDock(a, b));
            Assert.False(flight.CanAutoDock(a, c));
            Assert.True(flight.CanAutoDock(c, b));

            isl.Position = new Vector3(12, 3, 8);

            Assert.False(flight.CanAutoDock(a, b));
            Assert.False(flight.CanAutoDock(a, c));
            Assert.False(flight.CanAutoDock(c, b));
        }

        private Bridge addBridge(Island isl, Vector3 dir, Vector3 pos)
        {
            var b = new Bridge(new Level(), isl.Node.CreateChild());
            b.Node.Relative = Matrix.Invert(Matrix.LookAtRH(pos, pos + dir, Vector3.UnitY));
            isl.AddAddon(b);
            return b;
        }


        private Level setupRendering()
        {
            var level = createLevel();


            var cam = TW.Data.Get<CameraInfo>();
            cam.Mode = CameraInfo.CameraMode.ThirdPerson;

            var camEntity = new Entity();
            cam.ThirdPersonCameraTarget = camEntity;


            var controller = new ClusterFlightController(TW.Graphics.Keyboard);

            engine.AddSimulator(new BasicSimulator(() =>
                {
                    controller.SimulateFlightStep(cameraIsland);
                    camEntity.WorldMatrix =
                        Matrix.Translation(
                            cameraIsland.GetIslandsInCluster().Aggregate(new Vector3(), (acc, el) => acc + el.Position) /
                            cameraIsland.GetIslandsInCluster().Count());
                }));
            engine.AddSimulator(new ClusterPhysicsSimulator(level));

            engine.AddSimulator(new ThirdPersonCameraSimulator());

            //engine.AddSimulator(new LoadLevelSimulator(level));
            engine.AddSimulator(new WorldRenderingSimulator());
            return level;
        }

        private static Level createLevel()
        {
            var level = new Level();
            return level;
        }
    }
}