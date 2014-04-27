using DirectX11;
using MHGameWork.TheWizards.Engine;

using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Data
{
    public class Sphere : EngineModelObject
    {
        private Engine.WorldRendering.Entity ent;
        private MeshDynamicPhysicsElement dEl;

        public Sphere()
        {
            

            ent = new Engine.WorldRendering.Entity();

            var mBuilder = new MeshBuilder();
            mBuilder.AddSphere(12, 1);
            ent.Mesh = mBuilder.CreateMesh();



            XNAGame game = new XNAGame();

            var data = ent.Mesh.GetCollisionData();


            var box = new MeshCollisionData.Box();
            box.Dimensions = MathHelper.One.xna() * 2;
            box.Orientation = Matrix.Identity.xna();

            data.Boxes.Add(box);

            var builder = new MeshPhysicsActorBuilder(new MeshPhysicsPool());

            dEl = new MeshDynamicPhysicsElement(ent.Mesh, Matrix.Identity.xna(), builder);

            dEl.InitDynamic(TW.Physics.Scene);
            dEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 10;


            //TODO: use the MeshP
            //var factory = new MeshPhysicsElementFactory(engine, root);
            //game.AddXNAObject(factory);


      
          

            //TODO: this update function must be called
            //game.UpdateEvent += delegate
            //{

            //        dEl.Update(root, game);

            //};










        }

      

        /// <summary>
        /// Applies simulated changes from PhysX to this sphere
        /// </summary>
        public void ProcessPhysXChanges()
        {
            ent.WorldMatrix = dEl.Actor.GlobalOrientation.dx();
        }
    }
}
