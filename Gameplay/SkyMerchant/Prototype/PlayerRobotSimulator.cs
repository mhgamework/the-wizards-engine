using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._Engine.Windsor;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    /// <summary>
    /// Simulates the player robot 
    /// </summary>
    public class PlayerRobotSimulator
    {
        [NonOptional]
        public CustomCamera Camera { get; set; }

        [NonOptional]
        public ITypedFactory TypedFactory { get; set; }

        [NonOptional]
        public IWorldLocator WorldLocator { get; set; }

        [NonOptional]
        public PrototypeObjectsFactory PrototypeObjectsFactory { get; set; }

        private RobotPlayerPart robot;
        private RobotInventoryTextView view;

        public PlayerRobotSimulator(CustomCamera camera, ITypedFactory typedFactory, IWorldLocator worldLocator, PrototypeObjectsFactory prototypeObjectsFactory)
        {
            Camera = camera;
            TypedFactory = typedFactory;
            WorldLocator = worldLocator;
            PrototypeObjectsFactory = prototypeObjectsFactory;
            robot = createRobot();

        }

        public void ActivateRobotCamera()
        {

            TW.Data.Get<CameraInfo>().ActivateCustomCamera(Camera);

            Camera.SetProjectionMatrix(TW.Graphics.SpectaterCamera.Projection);
        }

        /// <summary>
        /// Simulates user input on the robot
        /// </summary>
        public void SimulateRobotUserInput()
        {
            tryPickup();

            robot.NormalMovement.LookDirection = TW.Graphics.SpectaterCamera.CameraDirection;
            setCameraView();
        }

        /// <summary>
        /// Simulates robot behaviour which does not originate from the user.
        /// </summary>
        public void SimulateRobotNonUserInput()
        {
            robot.SimulateMovement();
            robot.SimulateCogConsumption();
            robot.SimulateDeath();
        }

        private void tryPickup()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.F)) return;
            var trader = WorldLocator.AtObject<TraderVisualizerPart>(robot.Physical, 4).FirstOrDefault();
            if (trader != null && trader.TraderPart.CanTradeWith(robot))
            {
                trader.TraderPart.PerformTrade(robot);
            }

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
            var eye = robot.Physical.Position;
            var dir = TW.Graphics.SpectaterCamera.CameraDirection;

            eye += new Vector3(0, 2, 0);
            eye -= dir * 2.5f;
            eye += Vector3.Cross(dir, MathHelper.Up) * 0.5f;

            Camera.SetViewMatrix(Matrix.LookAtRH(eye, eye + dir, MathHelper.Up));
        }

        private RobotPlayerPart createRobot()
        {
            var mov = TypedFactory.CreateRobotMovementPart();
            var robot = TypedFactory.CreateRobotPlayerPart();
            robot.Physical = TypedFactory.CreatePhysicalPart();
            robot.NormalMovement = mov;
            robot.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/DummyRobot/DummyRobot");

            mov.Physical = robot.Physical;
            mov.Physics = TypedFactory.CreatePhysics();

            var scale = 0.1f;
            robot.Physical.ObjectMatrix = Matrix.Scaling(scale, scale, scale) * Matrix.RotationY(MathHelper.Pi);


            view = new RobotInventoryTextView(robot);

            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());
            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());
            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());
            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());
            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());

            return robot;
        }

    }
}