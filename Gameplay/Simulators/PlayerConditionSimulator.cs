using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Trigger;
using MHGameWork.TheWizards.WorldRendering;

namespace MHGameWork.TheWizards.Simulators
{
    public class PlayerConditionSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var change in TW.Data.GetChangesOfType<PlayerPositionCondition>())
            {
                var pos = (PlayerPositionCondition)change.ModelObject;

                if (change.Change == Data.ModelChange.Added)
                {
                    var newBox = new WireframeBox();
                    pos.set(newBox);
                }
                var box = pos.get<WireframeBox>();

                if (change.Change == Data.ModelChange.Removed)
                {
                    TW.Data.RemoveObject(box);
                }

                box.FromBoundingBox(pos.BoundingBox);
                box.Visible = true;



            }
        }
    }
}
