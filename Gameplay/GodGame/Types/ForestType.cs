using System.Drawing;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class ForestType : GameVoxelType
    {
        public ForestType()
            : base("Forest")
        {
            Color = Color.Green;

            for (int i = 0; i < 5; i++)
            {
                var scale = 0.1f + (i/5f * 0.9f);
                datavalueMeshes[i] = MeshBuilder.Transform(mesh, Matrix.Scaling(1, scale, 1));
            }

        }
        public override void Tick(IVoxelHandle handle)
        {
            if (handle.CurrentVoxel.DataValue >= 5)
            {
                handle.CurrentVoxel.DataValue = 5;
                return;
            }
            handle.EachRandomInterval(5, () => { handle.CurrentVoxel.DataValue++; });

        }



    }
}