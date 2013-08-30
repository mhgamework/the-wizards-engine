using System;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using DirectX11;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._Windsor;
using NUnit.Framework;
using System.Linq;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant
{
    [EngineTest]
    [TestFixture]
    public class PrototypeTest
    {
        [Test]
        public void TestPlayPrototype()
        {
            try
            {
                var container = BootstrapContainer();

                var prototype = container.Resolve<PrototypeTest>();
                prototype.Run();
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Init prototype");
            }

        }



        #region Injection
        [NonOptional]
        public TWEngine Engine { get; set; }
        [NonOptional]
        public PrototypeWorldGenerator PrototypeWorldGenerator { get; set; }
        [NonOptional]
        public ITypedFactory TypedFactory { get; set; }
        [NonOptional]
        public CustomCamera Camera { get; set; }
        [NonOptional]
        public IWorldLocator WorldLocator { get; set; }
        #endregion

        private RobotPlayerPart robot;
        private RobotInventoryTextView view;


        public PrototypeTest()
        {

        }

        private void Run()
        {
            DI.Get<TestSceneBuilder>().Setup = setupTest;
            Engine.AddSimulator(new BasicSimulator(simulate));

            Engine.AddSimulator(new PhysicalSimulator());
            Engine.AddSimulator(new WorldRenderingSimulator());

            Engine.Run();
        }

        private void simulate()
        {
            if (TW.Debug.LastException != null) return;
            foreach (var i in TW.Data.Objects.OfType<IslandPart>())
            {
                i.FixPhysical();
                //i.SimulateFloatForce();
                //i.SimulateIslandAirDrag();
                //i.SimulateIslandShieldCollision();
                //i.SimulateIslandWrapping();
                //i.SimulateMovement();
            }
            foreach (var i in TW.Data.Objects.OfType<ItemPart>())
            {
                i.FixPosition();
            }
            foreach (var i in TW.Data.Objects.OfType<GenerationSourcePart>())
            {
                i.SimulateGeneration();
            }
            foreach (var i in TW.Data.Objects.OfType<ProximityChaseEnemyPart>())
            {
                i.SimulateBehaviour();
            }
            processRobot();
        }

        private void processRobot()
        {
            tryPickup();

            robot.NormalMovement.LookDirection = TW.Graphics.SpectaterCamera.CameraDirection;
            robot.SimulateMovement();
            robot.SimulateCogConsumption();
            robot.SimulateDeath();
            setCameraView();
            view.Update();
        }

        private void tryPickup()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.F)) return;
            var resource = WorldLocator.AtObject<GenerationSourcePart>(robot.Physical, 4).FirstOrDefault();
            if (resource != null && resource.GenerationPart.HasResource)
            {
                resource.GenerationPart.PlayerPickResource(robot);
                return;
            }

            robot.PickupClosest();
        }

        private void setCameraView()
        {
            var eye = robot.Physical.GetPosition();
            var dir = TW.Graphics.SpectaterCamera.CameraDirection;

            eye += new Vector3(0, 2, 0);
            eye -= dir * 2.5f;
            eye += Vector3.Cross(dir, MathHelper.Up) * 0.5f;

            Camera.SetViewMatrix(Matrix.LookAtRH(eye, eye + dir, MathHelper.Up));
        }

        private void setupTest()
        {
            PrototypeWorldGenerator.GenerateWorld(30);

            robot = createRobot();

            TW.Data.Get<CameraInfo>().ActivateCustomCamera(Camera);

            Camera.SetProjectionMatrix(TW.Graphics.SpectaterCamera.Projection);
        }

        private RobotPlayerPart createRobot()
        {
            var mov = TypedFactory.CreateRobotMovementPart();
            var robot = TypedFactory.CreateRobotPlayerPart();
            robot.Physical = TypedFactory.CreatePhysical();
            robot.NormalMovement = mov;
            robot.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/DummyRobot/DummyRobot");

            mov.Physical = robot.Physical;
            mov.Physics = TypedFactory.CreatePhysics();

            var scale = 0.1f;
            robot.Physical.ObjectMatrix = Matrix.Scaling(scale, scale, scale) * Matrix.RotationY(MathHelper.Pi);


            view = new RobotInventoryTextView(robot);

            return robot;
        }


        public IWindsorContainer BootstrapContainer()
        {
            return new WindsorContainer()
                .Install(new PrototypeInstaller());
        }
    }
}