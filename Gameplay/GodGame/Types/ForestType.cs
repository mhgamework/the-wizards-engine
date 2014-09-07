using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class ForestType : GameVoxelType
    {
        public const int HarvestDataValue = 5;
        public const int ResetDataValue = 0;
        private ItemType woodItem;

        public ForestType()
            : base("Forest")
        {
            Color = Color.Green;

            woodItem = new ItemType { Name = "Wood", Mesh = UtilityMeshes.CreateBoxColored(Color.SaddleBrown, new Vector3(1)) };

            for (int i = 0; i < HarvestDataValue; i++)
            {
                var scale = 0.1f + (i / (float)HarvestDataValue * 0.9f);
                datavalueMeshes[i] = MeshBuilder.Transform(mesh, Matrix.Scaling(1, scale, 1));
            }

        }

        public override void Tick(IVoxelHandle handle)
        {
            if (handle.Data.DataValue >= HarvestDataValue)
            {
                handle.Data.DataValue = HarvestDataValue;
                return;
            }
            handle.EachRandomInterval(5, () => { handle.Data.DataValue++; });
        }

        public ItemType GetWoodItemType()
        {
            return woodItem;
        }



    }
}