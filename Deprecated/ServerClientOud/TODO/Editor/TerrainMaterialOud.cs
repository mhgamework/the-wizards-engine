using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class TerrainMaterial
    {
        private ServerClientMain engine;
        private List<TerrainBlock> queuedBlocks = new List<TerrainBlock>();


        public TerrainMaterial(ServerClientMain nEngine)
        {
            engine = nEngine;
        }

        public void AddToQueue(TerrainBlock block)
        {
            queuedBlocks.Add( block );
        }

        public void Render()
        {
            for ( int i = 0; i < queuedBlocks.Count; i++ )
            {
                //queuedBlocks[ i ].Draw( engine.XNAGame.GraphicsDevice );
            }
        }

    }
}
