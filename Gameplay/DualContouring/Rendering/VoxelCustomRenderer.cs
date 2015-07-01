using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.DualContouring.Rendering
{
    /// <summary>
    /// GBuffer renderer for surfaces outputted by dualcontouring
    /// Intended features: 
    ///  - multi material rendering
    ///  - triplanar texture mapping
    ///  - Lod transition blending
    /// </summary>
    public class VoxelCustomRenderer : ICustomGBufferRenderer
    {


        public void Dispose()
        {
            
        }

        public void Draw()
        {
        }
    }
}