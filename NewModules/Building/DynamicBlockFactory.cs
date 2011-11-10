using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for creating DynamicBlocks and keeping track of them (storing them in a list)
    /// </summary>
    public class DynamicBlockFactory
    {
        private readonly DeferredRenderer renderer;
        public List<DynamicBlock> BlockList = new List<DynamicBlock>();

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
            for (int i = 0; i < BlockList.Count; i++)
            {
                var cBlock = BlockList[i];
                if (cBlock.Position == pos)
                    return cBlock;
            }
            return null;
        }

        public Point3 CalculateBlockPos(Vector3 vPos)
        {
            
            return new Point3((int)Math.Round(vPos.X), (int)Math.Round(vPos.Y), (int)Math.Round(vPos.Z));
        }

       
        public DynamicBlock CreateNewDynamicBlock(Point3 pos)
        {
            var ret = new DynamicBlock(pos, renderer);
            BlockList.Add(ret);
            return ret;}

        internal void RemoveBlock(DynamicBlock b)
        {
            BlockList.Remove(b);
            b = null;
        }

        public DynamicBlock GetBlockAtPosition(Vector3 pos)
        {
            Point3 p = new Point3((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y), (int)Math.Floor(pos.Z));
            return GetBlockAtPosition(p);
        }
    }
}
