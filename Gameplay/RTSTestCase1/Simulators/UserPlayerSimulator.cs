using System.Linq;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Simulators
{
    /// <summary>
    /// Simulates the logic associated with the local player (so not remote players)
    /// 
    /// 
    /// Responsible for processing local user movement input
    /// Responsible for updating targeted items in the world
    /// 
    /// </summary>
    public class UserPlayerSimulator : ISimulator, IUserTargeter, IRockCreator
    {
        public UserButtonEvent Forward;
        public UserButtonEvent Backward;
        public UserButtonEvent StrafeLeft;
        public UserButtonEvent StrafeRight;
        public UserButtonEvent Jump;
        public UserButtonEvent Attack;
        public UserButtonEvent Use;
        private UserButtonEvent BuildCannon;


        private IPlayerMovementController playerController;

        private UserPlayer localPlayer = TW.Data.Get<LocalGameData>().LocalPlayer;

        /// <summary>
        /// This is a dependency (and it is circular!)
        /// Pull out the attacksimulator?
        /// </summary>
        public PlayerGroundAttacker groundAttacker { get; set; }

        public UserPlayerSimulator(IPlayerMovementController playerController)
        {
            this.playerController = playerController;
            var buttonFactory = TW.Data.Get<InputFactory>();

            Forward = buttonFactory.GetButton("moveForward");
            Backward = buttonFactory.GetButton("moveBackward");
            StrafeLeft = buttonFactory.GetButton("moveStrafeLeft");
            StrafeRight = buttonFactory.GetButton("moveStrafeRight");
            Jump = buttonFactory.GetButton("moveJump");
            Attack = buttonFactory.GetButton("attack");
            Use = buttonFactory.GetButton("use");
            BuildCannon = buttonFactory.GetButton("build1");

        }

        public void Simulate()
        {
            updateLookDirection();
            simulateMovement();

            updateTargeted();

            simulateAttacks();

            simulateBuilding();

            simulateInteraction();


        }

        private void simulateInteraction()
        {
            if (!Use.Pressed) return;
            localPlayer.PlayerInteraction.Targeter = this;
            localPlayer.PlayerInteraction.Interact();
        }


        private void simulateBuilding()
        {
            localPlayer.PlayerInteraction.Targeter = this;

            if (BuildCannon.Pressed)
                localPlayer.PlayerInteraction.BuildCannon();
        }

        private static void updateLookDirection()
        {
            // use specatorcamera lookdir
            TW.Data.Get<LocalGameData>().LocalPlayer.LookDirection = TW.Graphics.SpectaterCamera.CameraDirection;
        }

        private float chargeStrength = 0;
        private void simulateAttacks()
        {
            if (Attack.Pressed) { chargeStrength = 1; }
            if (Attack.Down) chargeStrength += TW.Graphics.Elapsed * 10;
            if (Attack.Released)
            {
                if (chargeStrength > 10) chargeStrength = 10;
                groundAttacker.Attack(chargeStrength);
            }

            groundAttacker.Update(TW.Graphics.Elapsed);
            groundAttacker.Render(TW.Graphics.LineManager3D);
        }

        private void updateTargeted()
        {
            //TODO: fix the foreach because wtf is this
            foreach (var pl in TW.Data.Objects.Where(o => o is UserPlayer).Cast<UserPlayer>().ToArray())
            {
                var r = pl.GetTargetingRay();
                var obj = TW.Data.Get<Engine.WorldRendering.World>().Raycast(r);
                targeted = obj.IsHit ? obj.Object : null;
                if (obj.IsHit)
                    targetPoint = r.GetPoint(obj.Distance);
            }

        }

        private void simulateMovement()
        {
            if (Forward.Down) playerController.MoveForward();
            if (Backward.Down) playerController.MoveBackward();
            if (StrafeLeft.Down) playerController.MoveLeft();
            if (StrafeRight.Down) playerController.MoveRight();
            if (Jump.Pressed) playerController.Jump();

            playerController.ProcessMovement(TW.Graphics.Elapsed);
        }


        private object targeted;
        object IUserTargeter.Targeted { get { return targeted; } }

        private Vector3 targetPoint;
        Vector3 IUserTargeter.TargetPoint { get { return targetPoint; } }

        IRock IRockCreator.CreateRock()
        {
            return new GroundAttackRock();
        }

        void IRockCreator.DestroyRock(IRock rock)
        {
            var r = (GroundAttackRock)rock;
            r.Physical.Visible = false;
            TW.Data.RemoveObject(r);
        }
    }

    [ModelObjectChanged]
    public class GroundAttackRock : EngineModelObject, IPhysical, IRock
    {
        public GroundAttackRock()
        {
            Physical = new Physical();
        }

        private Vector3 position;
        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            var size = 1;
            var height = 20;
            Physical.Mesh = UtilityMeshes.CreateMeshWithTexture(size, TW.Assets.LoadTexture("RTS//bark.jpg"));
            Physical.WorldMatrix = Matrix.Translation(position);
            Physical.ObjectMatrix = Matrix.Scaling(1, height, 1) * Matrix.Translation(-Vector3.UnitY * (size * height));
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
