using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class CropType : GameVoxelType
    {
        public static int HarvestDataVal
        {
            get { return 3; }
        }
        private ItemType cropItemType;

        public CropType()
            : base("Crop")
        {
            cropItemType = new ItemType() { Name = "Crop" };
            cropItemType.Mesh = UtilityMeshes.CreateBoxColored(Color.Yellow, new Vector3(1));
        }

        public override void Tick(Internal.IVoxelHandle handle)
        {
            handle.EachRandomInterval(2f, () =>
                {
                    if (!checkWaterInRange(handle))
                    {
                        handle.Data.DataValue = 0;
                    }
                    else
                    {
                        if (handle.Data.DataValue < HarvestDataVal)
                            handle.Data.DataValue++;
                    }
                });
        }

        public override IMesh GetMesh(IVoxelHandle gameVoxel)
        {
            return datavalueMeshes[gameVoxel.Data.DataValue];
        }

        private bool checkWaterInRange(IVoxelHandle handle)
        {
            return handle.Get8Connected().Select(h => h.Get4Connected()).Any(conn => conn.Any(h01 => h01.Type is WaterType));
        }

        public ItemType GetCropItemType()
        {
            return cropItemType;
        }

        public void Harvest(IVoxelHandle target, Inventory inventory)
        {
            if (target.Data.DataValue < HarvestDataVal) return;
            var type = GetCropItemType();

            var numAdded = inventory.AddNewItems(type, 1);
            target.Data.DataValue = numAdded > 0 ? 0 : target.Data.DataValue;
        }
    }
}
