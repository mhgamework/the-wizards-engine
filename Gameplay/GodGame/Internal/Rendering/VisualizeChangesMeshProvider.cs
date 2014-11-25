using System;
using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.GodGame.Internal.Rendering
{
    public class VisualizeChangesMeshProvider : IMeshProvider
    {
        private readonly IMeshProvider provider;
        private readonly Seeder r = new Seeder((new Random()).Next());

        public VisualizeChangesMeshProvider(IMeshProvider provider)
        {
            this.provider = provider;
        }

        public IMesh GetMesh(IVoxel gameVoxel)
        {
            return provider.GetMesh(gameVoxel);
            return VoxelMeshBuilder.createColoredMesh(Color.FromArgb(r.NextByte(0, 255), r.NextByte(0, 255), r.NextByte(0, 255)));
        }
    }
}