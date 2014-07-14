using System.Drawing;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class WaterType : GameVoxelType
    {
        public WaterType() : base("Water")
        {
            Color = Color.Aqua;
        }

        public override void Tick(Internal.IVoxelHandle handle)
        {
            if (handle.Seeder.NextFloat(0, 20) > 1) return;
            var possible = handle.Get4Connected().Where(e => e.Type == GameVoxelType.Hole).ToArray();
            if (possible.Length == 0) return;

            foreach (var b in possible)
            {
                b.ChangeType(GameVoxelType.Water);
            }
        }
    }
}
