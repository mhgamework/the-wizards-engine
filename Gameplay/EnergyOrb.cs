using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Gameplay
{
    public class EnergyOrb : Scripting.IStateScript
    {
        private bool _chargeMode;
        public bool ChargeModeEnabled
        {
            get { return _chargeMode; }
            set
            {
                _chargeMode = value;
                //actor.ActorFlags.DisableCollision = true;
                /*if (actor != null)
                    actor.BodyFlags.DisableGravity = value;*/
            }
        }

        private float _charge;
        public float Charge
        {
            get { return _charge; }
            set
            {
                _charge = value;

                if (_chargeMode)
                {

                    if (mesh != null) mesh.Radius = _charge;
                    if (_charge <= 0) return;
                    if (actor != null) ((SphereShape) actor.Shapes[0]).Radius = _charge;
                }
                else
                {
                    var factor = _charge/chargedCharge;
                    var color1 = Color.Green.ToVector3();
                    var color2 = Color.Red.ToVector3();
                    var color = new Color(Vector3.Lerp(color1, color2, 1-factor));
                    if (mesh != null) mesh.Color = color;
                }
            }
        }

        private SphereMesh mesh;
        private Actor actor;

        private float explodeTime;
        private float oneOverExplodeDuration;

        private float decay = 0.5f;


        private float chargedCharge;

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                if (actor != null)
                    actor.GlobalPosition = value;
            }
        }

        private bool startExplode;

        public void Init()
        {
            mesh = ScriptLayer.CreateSphereModel();
            mesh.Radius = 0.1f;
            actor = ScriptLayer.CreateSphereActor(0.1f, 20);
            actor.GlobalPosition = position;
            actor.BodyFlags.DisableGravity = true;

            actor.ContactReportFlags = ContactPairFlag.All;
            actor.Name = "EnergyOrb" + DateTime.Now.Second.ToString();

            

            ScriptLayer.Physics.AddContactNotification(onContact);
            explodeTime = -1;
            oneOverExplodeDuration = 1 / 0.2f;
        }

        private void onContact(ContactPair contactInformation, ContactPairFlag events)
        {
            if (actor == null) return;
            if (contactInformation.ActorA != actor && contactInformation.ActorB != actor) return;
            startExplode = true;
        }

        private void Explode()
        {
            explodeTime = 1 / oneOverExplodeDuration;
            if (actor == null) return;
            actor.Dispose();
            actor = null;

        }

        public void Destroy()
        {
            //Destroy mesh for real(now temp)
            mesh.WorldMatrix = new Matrix();


            if (actor == null) return;
            actor.Dispose();
            actor = null;


        }



        public void Update()
        {
            if (startExplode)
            {
                Explode();
                startExplode = false;
            }
            if (_chargeMode)
            {
                chargedCharge = _charge;
            }

            if (explodeTime > 0)
            {
                mesh.WorldMatrix = Matrix.CreateScale((1 - explodeTime * oneOverExplodeDuration) * 10 + 1) * Matrix.CreateTranslation(position);
                explodeTime -= ScriptLayer.Elapsed;

                if (explodeTime <= 0)
                {
                    //Cheat atm
                    mesh.WorldMatrix = new Matrix();
                }
                return;
            }

            Charge -= ScriptLayer.Elapsed * decay;
            if (Charge < 0) Charge = 0;
            if (Charge <= 0) Destroy();

            if (actor == null) return;


                

            position = actor.GlobalPosition;
            mesh.WorldMatrix = Matrix.CreateTranslation(position);

        }

        public void Draw()
        {
        }

        public void Fire(Vector3 velocity)
        {
            if (actor == null) return;
            actor.LinearVelocity = velocity;
        }

        /// <summary>
        /// TODO: use a generalized system for this
        /// </summary>
        private class ContactReport : UserContactReport
        {
            public override void OnContactNotify(ContactPair contactInformation, ContactPairFlag events)
            {

            }
        }
    }
}
