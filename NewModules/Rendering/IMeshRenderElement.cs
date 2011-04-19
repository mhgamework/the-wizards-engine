namespace MHGameWork.TheWizards.Rendering
{
    public interface IMeshRenderElement
    {
        IMesh Mesh { get; }
        Microsoft.Xna.Framework.Matrix WorldMatrix { get; set; }
        bool IsDeleted { get; }

        /// <summary>
        /// Removes this element from the renderer
        /// </summary>
        void Delete();
    }
}