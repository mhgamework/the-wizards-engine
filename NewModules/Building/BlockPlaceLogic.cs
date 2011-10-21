using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Tests.Building;
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
                     
                    if (canHaveAsType(currentBlock, currentType))
                        currentBlock.ChangeTypeTo(currentType); break;
                }

                /*
                var n = new Point3(currentBlockPos + Vector3.UnitX * -1);
                var s = new Point3(currentBlockPos + Vector3.UnitX);
                var e = new Point3(currentBlockPos + Vector3.UnitZ);
                var w = new Point3(currentBlockPos + Vector3.UnitZ * -1);

                

                if(hasBlock(n))
                {
                    currentBlock.ChangeTypeTo(blockTypeFactory.TypeList[0], Matrix.RotationY((float)Math.PI * -0.5f));
                    
                    if(hasBlock(w))
                    {
                        currentBlock.ChangeTypeTo(blockTypeFactory.TypeList[1], Matrix.RotationY((float)Math.PI * -0.5f));

                        if(hasBlock(s))
                        {
                            currentBlock.ChangeTypeTo(blockTypeFactory.TypeList[2], Matrix.RotationY((float)Math.PI * -0.5f));

                            if(hasBlock(e))
                            {
                                currentBlock.ChangeTypeTo(blockTypeFactory.TypeList[3], Matrix.RotationY(0));
                            }
                        }

                        if(hasBlock(e))
                        {
                            currentBlock.ChangeTypeTo(blockTypeFactory.TypeList[2], Matrix.RotationY((float)Math.PI * 0.5f));
                        }
                    }

                    if(hasBlock(e))
                    {
                        currentBlock.ChangeTypeTo(blockTypeFactory.TypeList[2], Matrix.RotationY((float)Math.PI * 0.5f));
                        
                    }
                }
                */
            }
        }

        private bool hasBlock(Point3 pos)
        {
            return blockFactory.hasBlockAtPosition(pos);
        }

        private bool canHaveAsType(Block b, BlockType t)
        {
            var worldLayout = calculateWorldLayout(b);

            return (~t.Layout.Layout & worldLayout.Layout) == 0;
        }

        private BlockLayout calculateWorldLayout(Block block)
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
                if (hasBlock(new Point3((block.Position.X + c.X), (block.Position.Y + c.Y), (block.Position.Z + c.Z))))
                    ret.Layout |= BlockLayout.GetMask(c);
            }

            return ret;
        }
    }
}
