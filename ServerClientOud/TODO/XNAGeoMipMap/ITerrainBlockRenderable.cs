using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
    public interface ITerrainBlockRenderable : Common.GeoMipMap.ITerrainBlock
    {
        int DrawPrimitives();
        void RenderBatched();
    }
}
