using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinSimulator :ISimulator
    {
        public void Simulate()
        {
            
            foreach (var spawner in TW.Data.GetChangedObjects<GoblinSpawner>())
            {
                if (spawner.get<WorldRendering.Entity>() == null)
                    spawner.set(new WorldRendering.Entity());

                var ent = spawner.get<WorldRendering.Entity>();


                if (TW.Data.HasChanged(ent))
                {
                    spawner.Position = ent.WorldMatrix.xna().Translation.dx();
                }

                ent.WorldMatrix = Matrix.Translation(spawner.Position);
                ent.Mesh = MeshFactory.Load("\\Core\\Barrel01");

                ent.Solid = true;
                ent.Static = false;

            }
        }
    }
}
