using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Rendering;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class HoleType : GameVoxelType
    {
        private FourWayModelBuilder meshBuilder;

        public HoleType()
            : base("Hole")
        {
            meshBuilder = new FourWayModelBuilder
            {
                BaseMesh = datavalueMeshes.ContainsKey(0) ? datavalueMeshes[0] : null,
                WayMesh = datavalueMeshes.ContainsKey(1) ? datavalueMeshes[1] : null,
                NoWayMesh = datavalueMeshes.ContainsKey(2) ? datavalueMeshes[2] : null
            };
        }

        public override IMesh GetMesh(IVoxelHandle handle)
        {
            var conn = handle.Get4Connected().ToArray();
            return meshBuilder.CreateMesh(isConnectedType(conn[0]), isConnectedType(conn[1]), isConnectedType(conn[2]), isConnectedType(conn[3]));
        }

        private bool isConnectedType(IVoxelHandle handle)
        {
            return handle.Type is HoleType || handle.Type is WaterType;
        }
    }
}
