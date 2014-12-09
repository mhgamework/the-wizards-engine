using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.SkyMerchant._Tests.Ideas;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class OreType : GameVoxelType
    {
        private ItemType oreItemType;
        public OreType(ItemTypesFactory factory)
            : base("Ore")
        {
            oreItemType = factory.CrystalType;
            ReceiveCreationEvents = true;

        }

        public override void OnCreated(IVoxel handle)
        {
            handle.SetPart(new Ore() { ResourceCount = 1 });
            base.OnCreated(handle);
        }

        public override void OnDestroyed(IVoxel handle)
        {
            handle.SetPart<Ore>(null);
            base.OnDestroyed(handle);
        }

        public override void Tick(IVoxelHandle handle)
        {
            if (handle.GetInternalVoxel().GetPart<Ore>().IsDepleted())
                handle.ChangeType(Land);
        }


        public ItemType GetOreItemType(IVoxelHandle target)
        {
            return oreItemType;
        }

    }
}