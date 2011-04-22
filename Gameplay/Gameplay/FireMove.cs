using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GamePlay;
using MHGameWork.TheWizards.Scripting;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Gameplay
{
    public class FireMove : IPlayerMove, IStateScript
    {
        private PlayerController controller;

        private float strength = 40f;

        public FireMove(PlayerController controller)
        {
            this.controller = controller;
        }

        public Vector3 FireDirection { get; set; }

        public void StartPrimaryAttack()
        {
            var orb = new EnergyOrb();
            orb.Position = calculateLeftOrbPosition();
            orb.Charge = 0.2f;
            ScriptLayer.ScriptRunner.RunScript(orb);
            orb.Fire(calculateFireDirection() * strength);
            controller.ApplyFeedbackVelocity(-calculateFireDirection() * 5);

        }

        public void EndPrimaryAttack()
        {

        }

        public void StartSecondaryAttack()
        {
            var orb = new EnergyOrb();
            orb.Position = calculateRightOrbPosition();
            orb.Charge = 0.2f;
            ScriptLayer.ScriptRunner.RunScript(orb);
            orb.Fire(calculateFireDirection() * strength);
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

            return controller.Player.Position + Vector3.Up * 1.5f + offset * +0.5f + right * -0.5f;

        }
        private Vector3 calculateRightOrbPosition()
        {
            var offset = calculateFireDirection();
            offset.Y = 0;
            offset.Normalize();

            var right = Vector3.Cross(offset, Vector3.Up);

            return controller.Player.Position + Vector3.Up * 1.5f + offset * +0.5f + right * 0.5f;

        }
    }
}
