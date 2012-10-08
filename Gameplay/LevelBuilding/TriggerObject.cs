using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.LevelBuilding
{
    /// <summary>
    /// Represents a Trigger in the world.
    /// </summary>
    public class TriggerObject
    {
        public Trigger.Trigger Trigger { get; set; }
        public WorldRendering.Entity Entity {get; set;}

        public TriggerObject()
        {
            Entity = new WorldRendering.Entity();
            Entity.Mesh = MeshFactory.Load("Helpers\\RedHelper\\RedHelper");
            Trigger = new Trigger.Trigger();
        }

        public void SetPosition(Matrix worldMatrix)
        {
            Entity.WorldMatrix = worldMatrix;
        }
        public Matrix GetPosition()
        {
            return Entity.WorldMatrix;
        }
    }
}
