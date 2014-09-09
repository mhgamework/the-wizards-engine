using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Rendering;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class WaterType : GameVoxelType
    {
        private const int maxDataVal = 4;
        private List<FourWayModelBuilder> meshBuilders = new List<FourWayModelBuilder>();

        public WaterType()
            : base("Water")
        {
            Color = Color.Aqua;

            for (int i = 0; i <= maxDataVal; i++)
            {
                meshBuilders.Add(new FourWayModelBuilder { BaseMesh = MeshBuilder.Transform(mesh, Matrix.Translation(0, -0.9f + (0.9f / maxDataVal) * i, 0)), /*NoWayMesh = datavalueMeshes[999]*/ });
            }
        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.EachRandomInterval(2, () =>
            {
                var possible = handle.Get4Connected().Where(e => e.Type == GameVoxelType.Hole).ToArray();
                if (possible.Length == 0) return;

                foreach (var b in possible)
                {
                    b.ChangeType(GameVoxelType.Water);
                    b.Data.DataValue = 0;
                }
            });

            if (handle.Data.DataValue >= maxDataVal) return;
            handle.EachRandomInterval(0.5f, () => { handle.Data.DataValue++; });
        }

        public override IMesh GetMesh(IVoxelHandle handle)
        {
            var conn = handle.Get4Connected().ToArray();
            var index = handle.Data.DataValue;
            index = index > meshBuilders.Count - 1 ? meshBuilders.Count - 1 : index;
            return meshBuilders[index].CreateMesh(isConnectedType(conn[0]), isConnectedType(conn[1]), isConnectedType(conn[2]), isConnectedType(conn[3]));
        }

        private bool isConnectedType(IVoxelHandle handle)
        {
            return handle.Type is HoleType || handle.Type is WaterType;
        }

    }
}
