using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for creating DynamicBlocks and keeping track of them (storing them in a list)
    /// </summary>
    public class DynamicBlockFactory
    {
        private readonly DeferredRenderer renderer;
        private List<DynamicBlock> blockList = new List<DynamicBlock>();

        public DynamicBlockFactory(DeferredRenderer renderer)
        {
            this.renderer = renderer;
        }

        /// <summary>
        /// returns the DynamicBlock at given position, null if there isn't a lock at this position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public DynamicBlock GetBlockAtPosition(Point3 pos)
        {
            for (int i = 0; i < blockList.Count; i++)
            {
                var cBlock = blockList[i];
                if (cBlock.Position == pos)
                    return cBlock;
            }
            return null;
        }

       
        public DynamicBlock CreateNewDynamicBlock(Point3 pos)
        {
            var ret = new DynamicBlock(pos, renderer);
            blockList.Add(ret);
            return ret;}
    }
}
