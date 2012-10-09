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
            TW.Data.EnsureAttachment<PlayerPositionCondition, RenderData>(arg => new RenderData());
            foreach (var obj in TW.Data.GetChangedObjects<PlayerPositionCondition>())
            {
                var data = obj.get<RenderData>();
                data.box.FromBoundingBox(obj.BoundingBox);
                data.box.Visible = true;
            }
        }

       class RenderData : IModelObjectAddon<PlayerPositionCondition>
       {
           public WireframeBox box;
           public RenderData()
           {
               box = new WireframeBox();
           }

           public void Dispose()
           {
               TW.Data.RemoveObject(box);
           }
       }
    }
}
