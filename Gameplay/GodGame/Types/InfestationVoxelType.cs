using System.Drawing;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class InfestationVoxelType : GameVoxelType
    {
        public InfestationVoxelType()
            : base("InfestationVoxelType")
        {
            ColoredBaseMesh = false;
            datavalueMeshes[0] = createColoredMesh(Color.FromArgb(5, 5, 5));
            datavalueMeshes[1] = createColoredMesh(Color.FromArgb(5, 5, 5));
            datavalueMeshes[2] = createColoredMesh(Color.FromArgb(20, 20, 20));
            datavalueMeshes[3] = createColoredMesh(Color.FromArgb(25, 25, 25));
            datavalueMeshes[4] = createColoredMesh(Color.FromArgb(50, 50, 50));

        }
        public override void Tick(IVoxelHandle handle)
        {
            
                
            if (handle.Seeder.NextFloat(0, 100) > 1) return;
            handle.Data.DataValue = handle.Seeder.NextInt(1, 4);
            var possible = handle.Get8Connected().Where(isInfecteable).ToArray();
            if (possible.Length == 0) return;
            var i = handle.Seeder.NextInt(0, possible.Length - 1);

            var target = possible[i];
            if (target.Data.MagicLevel > 10) return;
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