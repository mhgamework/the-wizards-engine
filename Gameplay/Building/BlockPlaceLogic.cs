using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for deciding which blocktype should be used in a position in the world, based on the adjacent blocks.
    /// </summary>
    public class BlockPlaceLogic
    {
        private readonly BlockTypeFactory blockTypeFactory;
        private readonly BlockFactory blockFactory;

        public BlockPlaceLogic(BlockTypeFactory blockTypeFactory, BlockFactory blockFactory)
        {
            this.blockTypeFactory = blockTypeFactory;
            this.blockFactory = blockFactory;
        }

        public void CalulateBlocks()
        {
            for (int i = 0; i < blockFactory.BlockList.Count(); i++)
            {
                var currentBlock = blockFactory.BlockList[i];

                for(int j = 0; j< blockTypeFactory.TypeList.Count(); j++)
                {
                    var currentType = blockTypeFactory.TypeList[j];
                     
                    if (canHaveAsType(currentBlock.Position, currentType))
                    { currentBlock.ChangeTypeTo(currentType); break;}
                }
                
            }
        }

        private bool hasBlock(Point3 pos)
        {
            return blockFactory.hasBlockAtPosition(pos);
        }

        private bool canHaveAsType(Point3 pos, BlockType t)
        {
            var worldLayout = calculateWorldLayout(pos);


            return (~t.Layout.Layout & worldLayout.Layout) == 0;
        }

        private BlockLayout calculateWorldLayout(Point3 pos)
        {
            var coords = new List<Point3>();

            var numbers = new int[3];
            numbers[0] = -1;
            numbers[1] = 0;
            numbers[2] = 1;

            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = 0; j < numbers.Length; j++)
                {
                    for (int k = 0; k < numbers.Length; k++)
                    {
                        coords.Add(new Point3(numbers[i], numbers[j], numbers[k]));
                    }   
                }
            }
            
            var ret = new BlockLayout();

            for (int i = 0; i < coords.Count; i++)
            {
                var c = coords[i];
                if (hasBlock(new Point3((pos.X + c.X), (pos.Y + c.Y), (pos.Z + c.Z))))
                    ret.Layout |= BlockLayout.GetMask(c);
            }

            return ret;
        }
    }
}
