using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class CropType : GameVoxelType
    {
        private int harvestDataVal = 3;

        public CropType()
            : base("Crop")
        {
            Color = Color.DarkKhaki;
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
                        if (handle.Data.DataValue < harvestDataVal)
                            handle.Data.DataValue++;
                    }
                });
        }

        public override Rendering.IMesh GetMesh(IVoxelHandle gameVoxel)
        {
            return datavalueMeshes[gameVoxel.Data.DataValue];
        }

        private bool checkWaterInRange(IVoxelHandle handle)
        {
            return handle.Get8Connected().Select(h => h.Get4Connected()).Any(conn => conn.Any(h01 => h01.Type is WaterType));
        }


    }
}
