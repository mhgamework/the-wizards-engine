using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Responsible for batching entities that are static into one big entity
    /// </summary>
    public class EntityBatcher : ISimulator
    {
        private Entity superEntity;

        bool temp = true;
        public void Simulate()
        {
            var changed = false;
            foreach (var ent in TW.Data.GetChngedObjectsOfType<Entity>())
            {
                if (ent == superEntity) continue;
                changed = true;
            }

            if (changed && temp)
            {
                temp = false;
                IMesh superMesh = new RAMMesh();

                foreach (var ent in TW.Data.Objects.Where(o => o is Entity).Select(o => (Entity)o))
                {
                    if (ent == superEntity) continue;
                    if (ent.Mesh == null ) continue;
                    if (!ent.Visible) continue;

                    MeshBuilder.AppendMeshTo(ent.Mesh, superMesh, ent.WorldMatrix);
                    ent.Visible = false;
                }

                var optimizer = new MeshOptimizer();
                superMesh = optimizer.CreateOptimized(superMesh);

                if (superEntity == null) superEntity = new Entity();
                superEntity.Mesh = superMesh;


            }



        }
    }
}
