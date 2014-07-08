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
        public override void Tick(IVoxelHandle handle)
        {
            if (handle.Seeder.NextFloat(0, 100) > 1) return;
            var possible = handle.Get8Connected().Where(isInfecteable).ToArray();
            if (possible.Length == 0) return;
            var i = handle.Seeder.NextInt(0, possible.Length - 1);

            var target = possible[i];

            target.Data.MagicLevel--;
            if (target.Data.MagicLevel < 0)
            {
                target.Data.MagicLevel = 0;
                target.ChangeType(GameVoxelType.Infestation);
            }
        }

        private bool isInfecteable(IVoxelHandle arg)
        {
            if (arg == null) return false;
            if (arg.Type == null) return false;
            if (arg.Type is InfestationVoxelType) return false;
            if (arg.Type == GameVoxelType.Air) return false;
            return true;
        }
    }
}