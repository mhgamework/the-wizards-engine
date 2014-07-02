using System.Drawing;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame
{
    public class InfestationVoxelType : GameVoxelType
    {
        public InfestationVoxelType()
            : base("InfestationVoxelType")
        {
            Color = Color.Red;
        }
        public override void Tick(GameVoxel v, ITickHandle handle)
        {
            if (handle.Seeder.NextFloat(0, 100) > 1) return;
            var possible = handle.Get8Connected(v).Where(isInfecteable).ToArray();
            if (possible.Length == 0) return;
            var i = handle.Seeder.NextInt(0, possible.Length - 1);

            possible[i].ChangeType(GameVoxelType.Infestation);
        }

        private bool isInfecteable(GameVoxel arg)
        {
            if (arg == null) return false;
            if (arg.Type == null) return false;
            if (arg.Type is InfestationVoxelType) return false;
            if (arg.Type == GameVoxelType.Air) return false;
            return true;
        }
    }
}