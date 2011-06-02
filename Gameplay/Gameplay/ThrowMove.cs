using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GamePlay;
using MHGameWork.TheWizards.Scripting;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Gameplay
{
    public class ThrowMove : IPlayerMove ,IStateScript
    {
        private PlayerController controller;

        private float chargeTime;
        private float maxCharge=30;
        private float startCharge = 1;
        private float strength = 30;
        private float chargeSpeed = 0.15f;

        public Vector3 FireDirection { get; set; }


        public ThrowMove(PlayerController controller)
        {
            this.controller = controller;
        }

        private EnergyOrb chargeOrb;

        public void StartPrimaryAttack()
        {
           /* chargeTime = startCharge;
            chargeOrb = new EnergyOrb();
            chargeOrb.Position = calculateOrbPosition();
            chargeOrb.ChargeModeEnabled = true;
            ScriptLayer.ScriptRunner.RunScript(chargeOrb);*/

            controller.DisableMovement();

        }

        public void EndPrimaryAttack()
        {
            chargeOrb.ChargeModeEnabled = false;
            chargeOrb.Fire(calculateFireDirection() * strength*chargeTime);
            controller.EnableMovement();
            chargeOrb = null;
            controller.ApplyFeedbackVelocity(-calculateFireDirection() * 5*  chargeTime);
        }

        public void StartSecondaryAttack()
        {
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
            if (chargeOrb == null) return;

          /*  chargeOrb.Position = calculateOrbPosition();
            chargeTime += ScriptLayer.Elapsed;
            chargeOrb.Charge = chargeTime*chargeSpeed;
            if (chargeTime > maxCharge) chargeTime = maxCharge;
            */


        }

        public void Draw()
        {
        }

        private Vector3 calculateFireDirection()
        {
            return FireDirection;
            /*var pos = Vector3.Transform(Vector3.Zero, controller.Camera.ViewInverse);
            Vector3 dir = Vector3.Transform(Vector3.Forward, controller.Camera.ViewInverse);
            return Vector3.Normalize(dir-pos);*/
        }

        private Vector3 calculateOrbPosition()
        {
            var offset = calculateFireDirection();
            offset.Y = 0;
            offset.Normalize();

            var right = Vector3.Cross(offset, Vector3.Up);



            return controller.Player.Position + Vector3.Up * 2 + offset * -0.5f+right*0.5f;
            
        }
    }
}
