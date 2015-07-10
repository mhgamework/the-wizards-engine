using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant.Worlding;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    /// <summary>
    /// Simulates the player robot 
    /// </summary>
    public class PlayerRobotSimulator
    {
        private readonly LocalPlayer localPlayer;

        public CustomCamera Camera { get; set; }

        public IWorldLocator WorldLocator { get; set; }

        public PrototypeObjectsFactory PrototypeObjectsFactory { get; set; }

        private RobotPlayerPart robot;
        private RobotInventoryTextView view;

        public PlayerRobotSimulator(CustomCamera camera,
            IWorldLocator worldLocator, 
            PrototypeObjectsFactory prototypeObjectsFactory, 
            LocalPlayer localPlayer,
            RobotInventoryTextView view)
        {
            this.localPlayer = localPlayer;
            this.view = view;
            Camera = camera;
            WorldLocator = worldLocator;
            PrototypeObjectsFactory = prototypeObjectsFactory;
            setupRobot();
            robot = localPlayer.RobotPlayerPart;

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

        private void setupRobot()
        {
            var robot = localPlayer.RobotPlayerPart;
            robot.MeshRenderComponent.Mesh = TW.Assets.LoadMesh("SkyMerchant/DummyRobot/DummyRobot");

            var scale = 0.1f;
            robot.MeshRenderComponent.ObjectMatrix = Matrix.Scaling(scale, scale, scale) * Matrix.RotationY(MathHelper.Pi);



            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());
            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());
            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());
            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());
            robot.Pickup(PrototypeObjectsFactory.CreateWoodBlock());
        }

    }
}