using System.Drawing;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class ForestType : GameVoxelType
    {
        public ForestType() : base("Forest")
        {
            Color = Color.Green;
        }
        public override void Tick(GameVoxel v, ITickHandle handle)
        {
            if (v.DataValue == 5) return;
            handle.EachRandomInterval(5, () => { v.DataValue++; });

        }


        
    }
}