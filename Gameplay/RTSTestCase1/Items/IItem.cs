using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;

namespace MHGameWork.TheWizards.RTSTestCase1.Items
{
    [ModelObjectChanged]
    public class ItemPart : EngineModelObject, IObjectPart
    {
        public ItemPart()
        {
            Free = true;
        }
        /// <summary>
        /// This is true when noone is holding this object.
        /// </summary>
        public bool Free { get; set; }
    }
}