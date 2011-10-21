using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for creating new blocks in the world.
    /// </summary>
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


        public Block CreateBlock(BlockType type, Point3 ghostPos)
        {
            for(int i = 0; i < BlockList.Count; i++)
            {
                if (BlockList[i].Position == ghostPos)
                    throw new InvalidOperationException();
            }

            var block = new Block(renderer, type, ghostPos);

            return block;
        }


        public bool hasBlockAtPosition(Point3 pos)
        {
            for(int i =0; i<BlockList.Count;i++)
            {
                if (BlockList[i].Position == pos) return true;
            }

            return false;
        }
    }
}
