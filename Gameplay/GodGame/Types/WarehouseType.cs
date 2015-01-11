using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class WarehouseType : GameVoxelType
    {
        public WarehouseType()
            : base("Warehouse")
        {
        }
        public override void Tick(IVoxelHandle handle)
        {
            handle.Data.Inventory.ChangeCapacity(25);

        }

        public override bool CanAcceptItemType(IVoxelHandle voxelHandle, ItemType type)
        {
            return false;
            return voxelHandle.Data.Inventory.AvailableSlots > 0;
        }

        public override System.Collections.Generic.IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
        {
            yield break;
        }

        public override System.Collections.Generic.IEnumerable<IRenderable> GetCustomVisualizers(IVoxelHandle handle)
        {
            var inv = new InventoryVisualizer(handle);
            inv.ItemRelativeTransformationProvider = i =>
                {
                    var size = 5;
                    var x = (float)(i % size);
                    var y = (float)(i / size);
                    var tileSize = 10;

                    var cellSize = tileSize / (float)size;

                    x -= size / 2f;
                    y -= size / 2f;


                    return Matrix.Scaling(MathHelper.One * 0.5f) *
                           Matrix.Translation(cellSize * (x + 0.5f), 1f, cellSize * (y + 0.5f));
                };
            yield return inv;
        }


    }
}