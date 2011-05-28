using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.TileEngine.SnapEngine
{
    public interface ISnappableWorldTarget
    {
        SnapInformation SnapInformation { get; }
        Transformation Transformation { get; }
    }
}
