namespace MHGameWork.TheWizards.Scattered.Rendering
{
    /// <summary>
    /// Interface used to identify objects that manage the renderer scope (eg the RendererScene)
    /// 
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// Updates the renderer state so that all information available for this object,
        ///  which affects the renderer is set to the renderer
        /// </summary>
        void UpdateRenderState();
    }
}