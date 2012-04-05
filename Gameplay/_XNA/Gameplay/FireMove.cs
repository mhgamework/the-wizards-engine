using MHGameWork.TheWizards._XNA.Gameplay.Fortress;
using MHGameWork.TheWizards._XNA.Scripting;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards._XNA.Gameplay
{
    public class FireMove : IPlayerMove, IStateScript
    {
        private readonly PlayerEntity playerEntity;
        private PlayerController controller;

        private float strength = 40f;

        public FireMove(PlayerEntity playerEntity, PlayerController controller)
        {
            this.playerEntity = playerEntity;
            this.controller = controller;
        }

        public Vector3 FireDirection { get; set; }

        public void StartPrimaryAttack()
        {
            createOrb(calculateLeftOrbPosition());
        }

        private void createOrb(Vector3 position)
        {
            var ent = playerEntity.Handle.CreateEntity();
            var orb = ent.AttachScript<EnergyOrb>();
           

            ent.Position = position;
            orb.Charge = 0.2f;
            orb.Fire(calculateFireDirection() * strength);
            controller.ApplyFeedbackVelocity(-calculateFireDirection() * 5);
        }

        public void EndPrimaryAttack()
        {

        }

        public void StartSecondaryAttack()
        {
            createOrb(calculateRightOrbPosition());
        }

        public void EndSecondaryAttack()
        {
        }

        public void Init()
        {
        }

        public void Destroy()
        {
        }

        public void Update()
        {


        }

        public void Draw()
        {
        }

        private Vector3 calculateFireDirection()
        {
            /*var pos = Vector3.Transform(Vector3.Zero, controller.Camera.ViewInverse);
            Vector3 dir = Vector3.Transform(Vector3.Forward, controller.Camera.ViewInverse);
            return Vector3.Normalize(dir - pos);*/
            return FireDirection;
        }

        private Vector3 calculateLeftOrbPosition()
        {
            var offset = calculateFireDirection();
            offset.Y = 0;
            offset.Normalize();

            var right = Vector3.Cross(offset, Vector3.Up);

            return controller.Player.Position.xna() + Vector3.Up * 1.5f + offset * +0.5f + right * -0.5f;

        }
        private Vector3 calculateRightOrbPosition()
        {
            var offset = calculateFireDirection();
            offset.Y = 0;
            offset.Normalize();

            var right = Vector3.Cross(offset, Vector3.Up);

            return controller.Player.Position.xna() + Vector3.Up * 1.5f + offset * +0.5f + right * 0.5f;

        }
    }
}
