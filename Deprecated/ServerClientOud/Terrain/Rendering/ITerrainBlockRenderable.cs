using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public interface ITerrainBlockRenderable : Common.GeoMipMap.ITerrainBlock
    {
        int DrawPrimitives();
        void RenderBatched();
    }
}
