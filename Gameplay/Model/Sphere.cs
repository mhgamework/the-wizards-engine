using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Entity.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Model
{
    public class Sphere : BaseModelObject
    {

        public Sphere()
        {
            var ent = new Entity();

            ent.Mesh = createSphereMesh();



























            XNAGame game = new XNAGame();

            var data = ent.Mesh.GetCollisionData();


            var box = new MeshCollisionData.Box();
            box.Dimensions = MathHelper.One.xna() * 2;
            box.Orientation = Matrix.Identity.xna();

            data.Boxes.Add(box);

            var builder = new MeshPhysicsActorBuilder(new MeshPhysicsPool());

            var dEl = new MeshDynamicPhysicsElement(ent.Mesh, Matrix.Identity.xna(), builder);

            dEl.InitDynamic(TW.Scene);
            dEl.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 10;


            //TODO: use the MeshP
            var factory = new MeshPhysicsElementFactory(engine, root);
            game.AddXNAObject(factory);


      
          

            //TODO: this update function must be called
            //game.UpdateEvent += delegate
            //{

            //        dEl.Update(root, game);

            //};










        }

        private static RAMMesh createSphereMesh()
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
            return mesh;
        }
    }
}
