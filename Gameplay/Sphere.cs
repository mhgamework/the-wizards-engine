using DirectX11;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Entity.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.ModelContainer
{
    public class Sphere : EngineModelObject
    {
        private WorldRendering.Entity ent;
        private MeshDynamicPhysicsElement dEl;

        public Sphere()
        {
            ent = new WorldRendering.Entity();


            ent.Mesh = CreateSphereMesh();




            XNAGame game = new XNAGame();

            var data = ent.Mesh.GetCollisionData();


            var box = new MeshCollisionData.Box();
            box.Dimensions = MathHelper.One.xna() * 2;
            box.Orientation = Matrix.Identity.xna();

            data.Boxes.Add(box);

            var builder = new MeshPhysicsActorBuilder(new MeshPhysicsPool());

            dEl = new MeshDynamicPhysicsElement(ent.Mesh, Matrix.Identity.xna(), builder);

            dEl.InitDynamic(TW.Scene);
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

        public static RAMMesh CreateSphereMesh()
        {
            MeshPartGeometryData.Source source;


            TangentVertex[] vertices;
            short[] indices;
            SphereMesh.CreateUnitSphereVerticesAndIndices(12, out vertices, out indices);


            var mesh = new RAMMesh();
            mesh.GetCoreData().Parts.Add(new MeshCoreData.Part());
            var part = new RAMMeshPart();
            mesh.GetCoreData().Parts[0].MeshPart = part;
            mesh.GetCoreData().Parts[0].ObjectMatrix = Matrix.Identity.xna();


            part.GetGeometryData().SetSourcesFromTangentVertices(indices, vertices);


            mesh.GetCoreData().Parts[0].MeshMaterial = new MeshCoreData.Material
                                                           {
                                                                DiffuseColor = new Color4(1,0,1,0).xna(),
                                                                DiffuseMap = null
                                                           };

            return mesh;
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
