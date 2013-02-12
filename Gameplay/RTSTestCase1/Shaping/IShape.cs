using System.Collections.Generic;
using DirectX11;

namespace MHGameWork.TheWizards.RTSTestCase1.Shaping
{
    public interface IShape
    {
        Dictionary<IShape, Point3> GetShape();
    }
}