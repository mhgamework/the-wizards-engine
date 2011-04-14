using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
    public class TerrainTexture
    {
        private Engine.Texture diffuseMap;

        public Engine.Texture DiffuseMap
        {
            get { return diffuseMap; }
            set { diffuseMap = value; }
        }
    }
}
