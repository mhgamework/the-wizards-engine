using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.ServerClient.Terrain;

namespace MHGameWork.TheWizards.Terrain.Client
{
    public class TerrainClientPhysics
    {
        private TerrainFullData terrainData;
        private TerrainBlockHeightfieldBuilder builder;

        public TerrainClientPhysics(TerrainFullData terrainData)
        {
            this.terrainData = terrainData;
            this.builder = new TerrainBlockHeightfieldBuilder();
        }

        public void LoadInClientPhysics( StillDesign.PhysX.Scene scene, ClientPhysicsQuadTreeNode root )
        {
            for ( int ix = 0; ix < terrainData.NumBlocksX; ix++ )
            {
                for ( int iz = 0; iz < terrainData.NumBlocksZ; iz++ )
                {

                    TerrainBlockClientPhysics block;

                    float blockHeight = 400 ; //TODO: this should be loaded from the preprocesser or somewhere else

                    block = new TerrainBlockClientPhysics( scene, terrainData, ix, iz, blockHeight, builder );
                    root.OrdenObject( block );

                }
            }

            
        }
    }
}
