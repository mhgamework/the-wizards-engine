using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Items
{
    /// <summary>
    /// Should be renamed Item? should be a component instead of an imodelobject?
    /// </summary>
    [ModelObjectChanged]
    public class DroppedThing : EngineModelObject,IPhysical,IItem
    {
        public DroppedThing()
        {
            Physical = new Physical();
            Item = new ItemPart();
        }

        public Thing Thing { get; set; }

        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            var ent = Physical;
            if (ent.Mesh == null)
                ent.Mesh = Thing.CreateMesh();

            ent.Solid = true;
            ent.Static = false;

            ent.Solid = false;

        }

        

        public ItemPart Item { get; set; }
    }
}