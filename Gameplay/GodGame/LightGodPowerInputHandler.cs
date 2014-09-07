using MHGameWork.TheWizards.GodGame.Internal;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame
{
    public class LightGodPowerInputHandler : IPlayerTool
    {

        public LightGodPowerInputHandler()
        {
        }

        public string Name { get { return "Let there be light!"; } }
        public void OnLeftClick(GameVoxel voxel)
        {

        }

        private GameVoxel prevVoxel;

        public void OnRightClick(GameVoxel voxel)
        {
            int radius = 3;
            if (prevVoxel != null)
            {
                new IVoxelHandle(prevVoxel).GetRangeCircle(radius).ForEach(v => v.Data.MagicLevel = 0);
            }

            var handle = new IVoxelHandle(voxel);
            handle.GetRangeCircle(radius).Where(v => v.Type is InfestationVoxelType).ForEach(removeInfestation);
            prevVoxel = voxel;
        }

        private void removeInfestation(IVoxelHandle obj)
        {
            GameVoxelType.Infestation.CureInfestation(obj);
        }
    }
}