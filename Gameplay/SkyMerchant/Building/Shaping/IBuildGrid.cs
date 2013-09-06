using System.Collections.Generic;
using DirectX11;

namespace MHGameWork.TheWizards.SkyMerchant.Building.Shaping
{
    public interface IBuildGrid
    {
        bool HasItemAt(Point3 pos);
        SimpPart GetItemAt(Point3 pos);
        void AddItemAt(Point3 point3, SimpPart zeroPart);
        bool RemoveItem(SimpPart block);
        Dictionary<Point3,SimpPart> GetAllBlocks();
    }
}