using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    /// <summary>
    /// Responsible for creation of all RTS related items.
    /// Should become and interface + implementation
    /// 
    /// There is no need for this to be a modelobject!!! investigate why or why it shouldn't be
    /// </summary>
    public class IRTSFactory  :EngineModelObject
    {
        public DroppedThing CreateDroppedThing(Thing thing,Vector3 position)
        {
            var t = new DroppedThing() { Thing = thing };
            t.Physical.WorldMatrix = Matrix.Translation(position);

            return t;
        }
        public DroppedThing CreateDroppedThing(ResourceType resource, Vector3 position)
        {
            return CreateDroppedThing(new Thing() {Type = resource}, position);
        }
    }
}