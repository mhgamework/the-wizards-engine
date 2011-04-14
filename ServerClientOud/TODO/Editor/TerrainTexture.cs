using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class TerrainTexture
    {
        private ServerClientMainOud engine;
        private Engine.Texture diffuseMap;
        private Common.GeoMipMap.Buffer2D alphaMap;

        public Common.GeoMipMap.Buffer2D AlphaMap
        {
            get { return alphaMap; }
            set { alphaMap = value; }
        }

        public Engine.Texture DiffuseMap
        {
            get { return diffuseMap; }
            set { diffuseMap = value; }
        }

        public TerrainTexture( ServerClientMainOud nEngine )
        {
            engine = nEngine;
        }

    }
}

