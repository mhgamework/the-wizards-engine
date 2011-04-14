using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    public class TerrainTexture
    {
        private TWTexture diffuseMap;

        public TWTexture DiffuseMap
        {
            get { return diffuseMap; }
            set { diffuseMap = value; }
        }
    }
}