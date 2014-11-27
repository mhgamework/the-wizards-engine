using MHGameWork.TheWizards.GodGame.Internal;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public class LightGodPowerTool : PlayerTool
    {
        private readonly InfestationVoxelType infestationType;

        public LightGodPowerTool(InfestationVoxelType infestationType)
            : base("Let there be light!")
        {
            this.infestationType = infestationType;
        }

        public override void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {

        }

        private GameVoxel prevVoxel;

        public override void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
            int radius = 3;
            if (prevVoxel != null)
            {
                prevVoxel.GetRangeCircle(radius).ForEach(v => v.Data.MagicLevel = 0);
            }

            var handle = voxel;
            handle.GetRangeCircle(radius).Where(v => v.Type is InfestationVoxelType).ForEach(removeInfestation);
            prevVoxel = voxel.GetInternalVoxel();
        }

        public override void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {
            
        }

        private void removeInfestation(IVoxelHandle obj)
        {
            infestationType.CureInfestation(obj);
        }
    }
}