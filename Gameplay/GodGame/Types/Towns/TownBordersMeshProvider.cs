using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Rendering;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    /// <summary>
    /// Adds town border displaying to 
    /// </summary>
    public class TownBordersMeshProvider : IMeshProvider
    {
        private readonly IMeshProvider provider;
        private readonly TownCenterService townCenterService;
        private FourWayModelBuilder builder;

        public TownBordersMeshProvider(IMeshProvider provider, TownCenterService townCenterService)
        {
            this.provider = provider;
            this.townCenterService = townCenterService;
            builder = new FourWayModelBuilder();

            var mBuilder = new MeshBuilder();
            mBuilder.AddBox(new Vector3(0.4f, 0, -0.5f), new Vector3(0.5f, 0.1f, 0.5f));

            builder.WayMesh = new RAMMesh();
            builder.NoWayMesh = mBuilder.CreateMesh();
        }

        public IMesh GetMesh(IVoxel gameVoxel)
        {
            var baseMesh = provider.GetMesh(gameVoxel);
            var town = townCenterService.GetTownForVoxel(gameVoxel);
            if (town == null) return baseMesh;

            var builder = new MeshBuilder();
            builder.AddMesh(baseMesh, Matrix.Identity);
            builder.AddMesh(this.builder.CreateMesh(p => town == townCenterService.GetTownForVoxel(gameVoxel.GetRelative(p))), Matrix.Identity);
            return builder.CreateMesh();
        }
    }
}