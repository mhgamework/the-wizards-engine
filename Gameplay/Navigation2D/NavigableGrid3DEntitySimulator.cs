using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class NavigableGrid3DEntitySimulator : ISimulator
    {
        private NavigableGrid2DData data = TW.Data.GetSingleton<NavigableGrid2DData>();
        private HashSet<Engine.WorldRendering.Entity> addedEntities = new HashSet<Engine.WorldRendering.Entity>();

        public void Simulate()
        {
            foreach (var change in TW.Data.GetChangesOfType<Engine.WorldRendering.Entity>())
            {
                var ent = change.ModelObject as Engine.WorldRendering.Entity;

                if (change.Change == ModelChange.Added)
                    tryAdd(ent);
                else if (change.Change == ModelChange.Removed)
                    tryRemove(ent);
                else
                {
                    tryAdd(ent);
                    tryRemove(ent);
                }

            }
        }

        /// <summary>
        /// Removes when necessary
        /// </summary>
        /// <param name="ent"></param>
        private void tryRemove(Engine.WorldRendering.Entity ent)
        {
            if (!addedEntities.Contains(ent)) return;
            data.Grid.RemoveObject(ent, getEntityBB(ent));
            addedEntities.Remove(ent);
        }
        /// <summary>
        /// adds when necessary
        /// </summary>
        /// <param name="ent"></param>
        private void tryAdd(Engine.WorldRendering.Entity ent)
        {
            if (!ent.Solid) return;
            if (addedEntities.Contains(ent)) return;
            data.Grid.AddObject(ent, getEntityBB(ent));
            addedEntities.Add(ent);
        }

        private BoundingBox getEntityBB(Engine.WorldRendering.Entity ent)
        {
            var bb = TW.Assets.GetBoundingBox(ent.Mesh);
            bb = bb.Transform(ent.WorldMatrix);
            return bb;
        }
    }
}