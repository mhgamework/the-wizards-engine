using SlimDX;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Culling
{
    public interface ICullable
    {
        BoundingBox BoundingBox { get;}
        /// <summary>
        /// This holds the count of howmany objects require this cullable to be visible
        /// </summary>
        int VisibleReferenceCount { get;set;}
    }
}