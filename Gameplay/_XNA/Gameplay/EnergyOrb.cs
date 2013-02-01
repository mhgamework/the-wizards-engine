using System;
using MHGameWork.TheWizards._XNA.Scripting;
using MHGameWork.TheWizards._XNA.Scripting.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards._XNA.Gameplay
{
    public partial class EnergyOrb : IStateScript, IScript
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

                    //if (mesh != null) mesh.Radius = _charge;
                    if (_charge <= 0) return;
                    //if (actor != null) ((SphereShape) actor.Shapes[0]).Radius = _charge;
                }
                else
                {
                    var factor = _charge/chargedCharge;
                    var color1 = Color.Green.ToVector3();
                    var color2 = Color.Red.ToVector3();
                    var color = new Color(Vector3.Lerp(color1, color2, 1-factor));
                    //if (mesh != null) mesh.Color = color;
                }
            }
        }

        //private SphereMesh mesh;
        //private Actor actor;

        private float explodeTime;
        private float oneOverExplodeDuration;

        private float decay = 0.5f;


        private float chargedCharge;

      

        private bool startExplode;
        private IEntityHandle handle;

        public void Init()
        {
            throw new InvalidOperationException();
         
        }

        private void onContact(ContactInformation information)
        {
            startExplode = true;
        }

        private void Explode()
        {
            explodeTime = 1 / oneOverExplodeDuration;
            handle.Solid = false;
        }

        public void Init(IEntityHandle handle)
        {
            this.handle = handle;
            //TODO: cheat!!!
            handle.Mesh = handle.GetMesh("Barrel");

            /*mesh = ScriptLayer.CreateSphereModel();
            mesh.Radius = 0.1f;
            actor = ScriptLayer.CreateSphereActor(0.1f, 20);
            actor.GlobalPosition = position;
            actor.BodyFlags.DisableGravity = true;

            actor.ContactReportFlags = ContactPairFlag.All;
            actor.Name = "EnergyOrb" + DateTime.Now.Second.ToString();*/


            handle.Solid = true;
            handle.Static = false;

            handle.RegisterContactHandler(onContact);
            //ScriptLayer.Physics.AddContactNotification(onContact);
            explodeTime = -1;
            oneOverExplodeDuration = 1 / 0.2f;
        }

        public void Destroy()
        {
            //Destroy mesh for real(now temp)
            /*mesh.WorldMatrix = new Matrix();


            if (actor == null) return;
            actor.Dispose();
            actor = null;*/


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
                //mesh.WorldMatrix = Matrix.CreateScale((1 - explodeTime * oneOverExplodeDuration) * 10 + 1) * Matrix.CreateTranslation(position);
                //handle.Position = position;
                explodeTime -= ScriptLayer.Elapsed;

                if (explodeTime <= 0)
                {
                    //Cheat atm
                    //mesh.WorldMatrix = new Matrix();
                    handle.Destroy();
                }
                return;
            }

            Charge -= ScriptLayer.Elapsed * decay;
            if (Charge < 0) Charge = 0;
            if (Charge <= 0) Destroy();

            //if (actor == null) return;


                

            //position = actor.GlobalPosition;
            //mesh.WorldMatrix = Matrix.CreateTranslation(position);

        }

        public void Draw()
        {
        }

        public void Fire(Vector3 velocity)
        {
            //if (actor == null) return;
            //actor.LinearVelocity = velocity;
        }

    }
}
