using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Physics;
using SlimDX;

namespace MHGameWork.TheWizards.PhysX
{
    public class EntityPhysXUpdater
    {
        private MeshPhysicsElementFactory factory;
        private ClientPhysicsQuadTreeNode root;

        public EntityPhysXUpdater(MeshPhysicsElementFactory factory, ClientPhysicsQuadTreeNode root)
        {
            this.factory = factory;
            this.root = root;
        }

        public void Update()
        {
           


            foreach (var change in TW.Model.GetChangesOfType<WorldRendering.Entity>())
            {
                var ent = change.ModelObject as WorldRendering.Entity;

                if (change.Change == ModelContainer.ModelChange.Removed)
                {
                    deletePhysicsElement(factory, ent);
                    continue;
                }



                deletePhysicsElement(factory, ent);
                var el = factory.CreateStaticElement(ent.Mesh, ent.WorldMatrix.xna());
                ent.set(el);



            }
        }

        private void deletePhysicsElement(MeshPhysicsElementFactory factory, WorldRendering.Entity ent)
        {
            var el = ent.get<MeshStaticPhysicsElement>();
            if (el != null)
            {
                factory.DeleteStaticElement(el);
            }
            ent.set<MeshStaticPhysicsElement>(null);
        }
    }
}
