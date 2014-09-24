using MHGameWork.TheWizards.GodGame.Internal;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public class LightGodPowerTool : IPlayerTool
    {
        private readonly InfestationVoxelType infestationType;

        public LightGodPowerTool(InfestationVoxelType infestationType)
        {
            this.infestationType = infestationType;
        }

        public string Name { get { return "Let there be light!"; } }
        public void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {

        }

        private GameVoxel prevVoxel;

        public void OnRightClick(PlayerState player, IVoxelHandle voxel)
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

        public void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {
            
        }

        private void removeInfestation(IVoxelHandle obj)
        {
            infestationType.CureInfestation(obj);
        }
    }
}