using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class OreType : GameVoxelType
    {
        private ItemType oreItemType;
        public OreType()
            : base("Ore")
        {
            oreItemType = new ItemType() { Name = "Crystal" };
            oreItemType.Mesh = UtilityMeshes.CreateBoxColored(Color.LightBlue, new Vector3(1));
        }


        public ItemType GetOreItemType(IVoxelHandle target)
        {
            return oreItemType;
        }

        public void MineResource(IVoxelHandle target, Inventory inventory)
        {
            if (target.Data.DataValue > 0)
            {
                var type = GetOreItemType(target);
                if (inventory.CanAdd(type, 1))
                {
                    inventory.AddNewItems(type, 1);
                    target.Data.DataValue -= 1;    
                }
                
            }

            if (target.Data.DataValue == 0)
                target.ChangeType(Land);
        }

    }
}