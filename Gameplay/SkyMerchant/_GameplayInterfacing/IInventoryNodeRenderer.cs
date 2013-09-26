using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Provides a visualisation for a inventory node, where the visualiation is done within given bounding box.
    /// Note that this interface is implemented as a decorated, so it is possible that an implementation is unable to render certain nodes.
    /// </summary>
    public interface IInventoryNodeRenderer
    {
        /// <summary>
        /// Returns true when rendering was successfull.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="bb"></param>
        /// <returns></returns>
        bool MakeVisible(IInventoryNode item, BoundingBox bb);
    }
}