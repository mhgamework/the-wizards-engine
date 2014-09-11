using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
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

        public CropType(ItemTypesFactory factory)
            : base("Crop")
        {
            cropItemType = factory.CropType;
        }

        public override void Tick(IVoxelHandle handle)
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

        public override IMesh GetMesh(IVoxelHandle handle)
        {
            IMesh tmp = datavalueMeshes[handle.Data.DataValue];

            var meshBuilder = new MeshBuilder();
            meshBuilder.AddMesh(tmp, Matrix.Identity);
            var groundMesh = GetDefaultGroundMesh(handle.Data.Height);
            if (groundMesh == null) return tmp;
            meshBuilder.AddMesh(groundMesh, Matrix.Identity);
            return meshBuilder.CreateMesh();
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

            if (inventory.CanAdd(type, 1))
            {
                inventory.AddNewItems(type, 1);
                target.Data.DataValue = 0;
            }

        }
    }
}
