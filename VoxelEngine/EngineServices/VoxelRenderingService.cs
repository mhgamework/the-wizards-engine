using System;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.VoxelEngine.EngineServices
{
    /// <summary>
    /// Initializes and provides a VoxelCustomRenderer into the engine
    /// Only one instance allowed per engine
    /// </summary>
    public class VoxelRenderingService
    {
        private static bool initialized = false;
        public VoxelCustomRenderer VoxelRenderer { get; private set; }

        public VoxelRenderingService(GraphicsWrapper graphics)
        {
            if ( initialized ) throw new InvalidOperationException( "Service was already created!" );
            initialized = true;

            VoxelRenderer = VoxelCustomRenderer.CreateDefault(TW.Graphics);
            graphics.AcquireRenderer().AddCustomGBufferRenderer(VoxelRenderer);
        }
    }
}