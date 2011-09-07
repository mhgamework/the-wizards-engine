using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.Building
{
    public class BlockFactory
    {
        private readonly DeferredRenderer renderer;
        public List<Block> BlockList = new List<Block>();

        public BlockFactory(DeferredRenderer renderer)
        {
            this.renderer = renderer;
        }


        public void AddBlock(Block block)
        {
            BlockList.Add(block);
        }


        public Block CreateBlock(Rendering.IMesh mesh, Point3 ghostPos)
        {
            for(int i = 0; i < BlockList.Count; i++)
            {
                if (BlockList[i].Position == ghostPos)
                    throw new InvalidOperationException();
            }

            var block = new Block(renderer, mesh, ghostPos);

            return block;
        }

       

    }
}
