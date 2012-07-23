//using System;
//using System.Collections.Generic;
//using System.Text;
//using TreeGenerator.help;
//using Microsoft.Xna.Framework;

//namespace TreeGenerator
//{
//    class TreeStructureLeafGenerater
//    {
//        TreeStructureSegment BaseSegment;
//        TreeTypeData TreeTypeData;
//        public List<TreeStructureLeaf> StructureLeaves;
//        Seeder seeder;

//        public TreeStructureLeafGenerater(TreeStructure treeStructure,TreeTypeData TreeTypeData)
//        {
//            BaseSegment = treeStructure.Base;
//            seeder = new Seeder(66946);
//        }

//        public List<TreeStructureLeaf> GenerateLeaves()
//        {
//            List<TreeStructureLeaf> leaves = new List<TreeStructureLeaf>();
//            TreeTypeLevel currentLevel;
//            List<List<TreeStructureSegment>> branches;
//            for (int i = 0; i < TreeTypeData.Levels.Count; i++)
//            {
//                currentLevel=TreeTypeData.Levels[i];
//                if (currentLevel.LeafType.Count!=0)
//                {
//                    for (int j = 0; j < currentLevel.LeafType.Count; j++)
//                    {
//                        TreeLeafType leafType=currentLevel.LeafType[j];
//                        int leafCount=seeder.NextInt(currentLevel.LeafType[j].LeafCount);
//                        branches = FindBranches(i);

//                        for (int k = 0; k < branches.Count; k++)
//                        {
//                            float branchLenght=0;
//                            for (int l = 0; l < branches[k].Count; l++)
//                            {
//                                 branchLenght+=branches[k][l].Length;
//                            }
                           
//                            for (int l = 0; l < leafCount; l++)
//                            {
//                                float relativePosition = seeder.NextFloat(leafType.RelativePosition,l,leafCount);
//                                float relativeDropAngle = seeder.NextFloat(leafType.DropAngle, l, leafCount);
//                                float relativeAxialSplit = seeder.NextFloat(leafType.AxialSplit, l, leafCount);
//                                float absolutePosition=0;
//                                int index=0;
//                                while (branchLenght*relativePosition>=absolutePosition)
//                                {
//                                    absolutePosition += branches[k][index].Length;
//                                    index++;
//                                }
//                                //leaves.Add(GenerateLeave(leafType, branches[k][index], relativeDropAngle, relativeAxialSplit,absolutePosition-branchLenght*relativePosition));
                             
//                            }

//                        }
                        
//                    }
//                }
//            }
//        }

//       /* public TreeStructureLeaf GenerateLeave(TreeLeafType leafType,TreeStructureSegment segment,float relativeDropAngle,float relativeAxialSplit,float distanceInSegment)
//        {
//            TreeStructureLeaf leaf=new TreeStructureLeaf();
//            leaf.Length = seeder.NextFloat(leafType.Length);
//            leaf.Width = seeder.NextFloat(leafType.width);
//            leaf.Direction=Directions.DirectionsFromAngles(segment.Direction,relativeDropAngle,relativeAxialSplit);
//            leaf.Position = distanceInSegment;

//        }*/

//        public List<List<TreeStructureSegment>> FindBranches(int ilevel)
//        {
//            TreeStructureSegment NextSegment=BaseSegment;
//            List<List<TreeStructureSegment>> branches;
//            List<TreeStructureSegment> branch;
//            int levelCount=0;
//            if (ilevel == 0)
//            {
//                while(NextSegment.Children !=null)
//                {
//                    branch.Add(NextSegment);
//                    NextSegment = NextSegment.Children[0];
//                }
//                branches.Add(branch);
//                return branches;
//            }
//            else
//            {
//                TreeStructureSegment nextSegment=NextSegment;
//                TreeStructureSegment tempSeg;
//                while (NextSegment.Children != null && levelCount=0)
//                {
//                    if (NextSegment.Children.Count>=2)
//                    {
//                        levelCount += 1;
//                        if (levelCount = ilevel)
//                        {
//                            tempSeg = NextSegment;
//                            while (tempSeg.Children != null)
//                            {
//                                branch.Add(tempSeg);
//                                tempSeg = tempSeg.Children[0];
//                            }
//                            branches.Add(branch);
//                            levelCount = 0;


//                            NextSegment = nextSegment;
//                        }
//                        else
//                        {
//                            nextSegment = NextSegment;
//                            NextSegment = NextSegment.Children[1];
//                        }

//                    }
//                    NextSegment = NextSegment.Children[0];

//                    if (NextSegment.Children != null && levelCount=0)
//                    {
//                        return branches;
//                    }
//                }
//            }
//        }
//    }
//}
