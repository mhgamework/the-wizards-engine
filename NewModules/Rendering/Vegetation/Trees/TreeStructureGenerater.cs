using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Common.Core;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Vegetation.Trees
{
    public class TreeStructureGenerater
    {
        TreeStructure treeStructure;
        public TreeTypeData TreeTypeData;
        //public Vector3 Position;
        //public int Seed;
        private DXSeeder seeder;
        public DXSeeder leafSeeder=new DXSeeder(10);// temp for test

        public TreeStructureGenerater()
        {

        }
        public TreeStructure GenerateTree(TreeTypeData treeTypeData, int seed)
        {
            TreeTypeData = treeTypeData;
            treeStructure = new TreeStructure();
            seeder = new DXSeeder(seed);
            leafSeeder = new DXSeeder(seed);
            TreeStructureSegment seg = new TreeStructureSegment();
            seg.Length = 0;

            seg.Radius = seeder.NextFloat(treeTypeData.Levels[0].BranchStartDiameterFactor.Min,treeTypeData.Levels[0].BranchStartDiameterFactor.Max);
            treeStructure.AmountOfLevels = treeTypeData.Levels.Count;
            treeStructure.Base = seg;
            treeStructure.Textures = new List<string>();
            treeStructure.Textures.Add(TreeTypeData.TextureBark.GetCoreData().DiskFilePath);
            treeStructure.TextureHeight = treeTypeData.TextureHeight;
            treeStructure.TextureWidth = treeTypeData.TextureWidth;
            //if (treeTypeData.BumpTexture == null)
            //{
            //    treeStructure.Bumps.Add("null");
            //}
            //else { treeStructure.Bumps.Add(treeTypeData.BumpTexture); }

            GenerateBranch(treeStructure.Base, 0, 0, 0,0);

            //for (int i = 0; i < (treeTypeData.Levels.Count - lodLevel); i++)
            //{
            //    treeStructure.UVMaps.Add(treeTypeData.Levels[i].UVMap);
            //}



            return treeStructure;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="iLevel"></param>
        /// <param name="numParentChildren">This is the number of child branches that the parent has of this level</param>
        private void GenerateBranch(TreeStructureSegment parent, int iLevel, int parentChildIndex, int numParentChildren,float PositionIndex)
        {
            TreeTypeLevel level = TreeTypeData.Levels[iLevel];
            TreeTypeLevel parentLevel = null;
            float length;
            length = seeder.NextFloat(level.BranchLength.Min,level.BranchLength.Max);
            length += -PositionIndex*seeder.NextFloat(level.BranchLenghtDegradation.Min,level.BranchLenghtDegradation.Max)*length;
            float dropAngle = 0;
            float axialSplit = 0;
            Directions branchDirections = new Directions(Vector3.UnitY, Vector3.UnitZ);
            bool textureAlreadyIn = false;
            //leaves
            List<TreeLeafType> leafTypes = level.LeafType;
            int leafTypesCount;
            if (leafTypes == null)
            {
                leafTypesCount = 0;
            }
            else
            {
                leafTypesCount = leafTypes.Count;
                // put all the textures in the texture array
                for (int i = 0; i < leafTypesCount; i++)
                {
                    for (int j = 0; j < treeStructure.Textures.Count; j++)
                    {
                        if (leafTypes[i].Texture.GetCoreData().DiskFilePath == treeStructure.Textures[j])
                        {
                            textureAlreadyIn = true;
                        }
                    }
                    if (textureAlreadyIn == false)
                    {
                        treeStructure.Textures.Add(leafTypes[i].Texture.GetCoreData().DiskFilePath);
                        //if (leafTypes[i].BumpTexture != null)
                        //{
                        //    treeStructure.Bumps.Add(leafTypes[i].BumpTexture);
                        //}
                        //else
                        //{ treeStructure.Bumps.Add("null"); }
                    }
                    textureAlreadyIn = false;
                }

            }
        


            if (iLevel != 0)
            {
                dropAngle = seeder.NextFloat(level.BranchDropAngle.Min, level.BranchDropAngle.Max);
                axialSplit = NextFloat(level.BranchAxialSplit, parentChildIndex, numParentChildren);// i can put here the amount of 
                branchDirections = Directions.DirectionsFromAngles(parent.Direction, dropAngle, axialSplit);
            }
            if (iLevel != 0)
            {
                parentLevel = TreeTypeData.Levels[iLevel - 1];
             
            }


            float startRadius;
            float endRadius;
            startRadius = seeder.NextFloat(level.BranchStartDiameterFactor.Min,level.BranchStartDiameterFactor.Max) * parent.Radius;
            endRadius = seeder.NextFloat(level.BranchEndDiameterFactor.Min,level.BranchEndDiameterFactor.Max) * startRadius;




            float numSegments = (int)(length / level.BranchMaxSegmentLength);

            LinkedList<TreeStructureSegment> segments = new LinkedList<TreeStructureSegment>();

            TreeStructureSegment prevSegment = parent;

            //leaves
            List<int> leafCount = new List<int>();
            List<List<float>> relativeLeafPosition = new List<List<float>>();
            List<List<float>> dropAngleleaves = new List<List<float>>();

            for (int i = 0; i < leafTypesCount; i++)
            {
                int count = (int)leafSeeder.NextFloat(leafTypes[i].LeafCount.Min, leafTypes[i].LeafCount.Max);
                leafCount.Add(count);
                relativeLeafPosition.Add(new List<float>());
                for (int j = 0; j < leafCount[i]; j++)
                {
                    float pos = length * NextFloat(leafTypes[i].RelativePosition, j , leafCount[i]);
                    relativeLeafPosition[i].Add(pos);
                }
            }
            for (int i = 0; i < leafTypesCount; i++)
            {
                relativeLeafPosition[i].Sort();
            }
            for (int i = 0; i < leafTypesCount; i++)
            {
                dropAngleleaves.Add(new List<float>());
                for (int j = 0; j < relativeLeafPosition[i].Count; j++)
                {
                    dropAngleleaves[i].Add(leafSeeder.NextFloat(leafTypes[i].DropAngle.Min, leafTypes[i].DropAngle.Max));
                }
            }




            float totalAngle = dropAngle;
            for (int i = 0; i < numSegments; i++)
            {
                List<TreeStructureLeaf> leaves = new List<TreeStructureLeaf>();

                float localBranchLenght = (i + 1) * level.BranchMaxSegmentLength;
                //Wobble
                //seeder = new Seeder(seeder.Seed * i);
                float wobbleDropAngle = seeder.NextFloat(level.BranchWobbleDropAngle.Min, level.BranchWobbleDropAngle.Max);
                float wobbleAxialSplit = seeder.NextFloat(level.BranchWobbleAxialSplit.Min, level.BranchWobbleAxialSplit.Max);

                branchDirections = Directions.DirectionsFromAngleDown(branchDirections, wobbleDropAngle);// not so good if the branch hang over to much they won't wobble
                branchDirections = Directions.DirectionsFromAngles(branchDirections, 0, wobbleAxialSplit);
                float bendingAngle;
                bendingAngle = level.BranchBendingStrenght * (1 + level.BranchBendingFlexibility * level.BranchBendingFlexibility * i * i);
                if (totalAngle + bendingAngle < MathHelper.Pi)
                {

                    branchDirections = Directions.DirectionsFromAngleDown(branchDirections, bendingAngle);

                }
                else { branchDirections.Heading = -Vector3.UnitY; branchDirections = Directions.DirectionsFromAngles(branchDirections, wobbleDropAngle, 0); }

                //create the leaves and positions them in the rightway
                for (int l = 0; l < leafTypesCount; l++)
                {
                    int index = 0;
                    while (treeStructure.Textures[index] != leafTypes[l].Texture.GetCoreData().DiskFilePath)
                    {
                        index++;
                    }


                    for (int j = 0; j < leafCount[l]; j++)
                    {
                        if (relativeLeafPosition[l][j] < localBranchLenght && relativeLeafPosition[l][j] > localBranchLenght - level.BranchMaxSegmentLength)
                        {

                            leaves.Add(CreateLeave(leafTypes[l], branchDirections,dropAngleleaves[l][j],NextFloat(leafTypes[l].AxialSplitOrientation,l,leafCount[l]) ,NextFloat(leafTypes[l].AxialSplitPosition, j, leafCount[l]),
                            localBranchLenght - relativeLeafPosition[l][j], index,iLevel));
                        }
                        else { if (relativeLeafPosition[l][j] > localBranchLenght) { j = leafCount[l]; } }
                    }
                }



                TreeStructureSegment segment = CreateSegment(level, iLevel, prevSegment, branchDirections,0, leaves);
                //Radius

                float positionPercent = i * level.BranchMaxSegmentLength / length;
                segment.Radius = MathHelper.Lerp(startRadius, endRadius, positionPercent);


                segments.AddLast(segment);
                prevSegment = segment;
                totalAngle += bendingAngle;


               


            }
            //TODO: laatste segment
            if (length - numSegments * level.BranchMaxSegmentLength > 0.01f)
            {
                TreeStructureSegment segment = CreateSegment(level, iLevel, prevSegment, branchDirections,0, null);
                segment.Radius = endRadius;
                segments.AddLast(segment);
            }



            GenerateBranchChildren(iLevel, length, segments);






        }

        private void GenerateBranchChildren(int iLevel, float length, LinkedList<TreeStructureSegment> segments)
        {
            if (!((TreeTypeData.Levels.Count) > iLevel + 1)) return;

            List<TreeStructureSegment> childSegments = new List<TreeStructureSegment>();

            TreeStructureSegment segmentSplit = null;
            TreeTypeLevel childLevel = TreeTypeData.Levels[iLevel + 1];
            int numChilderen = (int)seeder.NextFloat(childLevel.BranchCount.Min, childLevel.BranchCount.Max);
            float relativePosition;
            List<float> relativePositions = new List<float>();
            float absolutePosition=0;
            int amountOfSteps = 0;
            List<int> branchesPerStep=new List<int>();
            List<float> positionPerStep = new List<float>();
            if (childLevel.Steps)
            {
                amountOfSteps = (int)(((childLevel.BranchPositionFactor.Max-childLevel.BranchPositionFactor.Min)*length)*childLevel.StepsPerMeter);
                if (amountOfSteps==0)
                {
                    amountOfSteps = 1;
                }
               // branchesPerStep = new int[amountOfSteps];
                //positionPerStep = new float[amountOfSteps];
                int tempBranchPerStep = numChilderen/amountOfSteps;
                float tempDistributionCoof = (2.0f/amountOfSteps);
                for (int i = 0; i <amountOfSteps; i++)
                {
                    branchesPerStep.Add((int)(tempBranchPerStep +tempBranchPerStep*(childLevel.BranchDistributionPercentage*(1 - tempDistributionCoof*i))));
                    positionPerStep.Add(MathHelper.Clamp(NextFloat(childLevel.BranchPositionFactor, i + 1, amountOfSteps), 0.001f, 1 - 0.001f));
                }

            }
         
            if (childLevel.Steps)
            {
                for (int j = 0; j < amountOfSteps; j++)
                {
                    for (int k = 0; k < branchesPerStep[j]; k++)
                    {
                        absolutePosition = length * (positionPerStep[j] + seeder.NextFloat(childLevel.BranchStepSpreading.Min, childLevel.BranchStepSpreading.Max) * 0.1f);

                         float location = 0;
                         LinkedListNode<TreeStructureSegment> currentSegment = segments.First;
                         float currentLength = 0;
                         //bool segmentFound = false;
                         while (true)
                         {
                             currentLength += currentSegment.Value.Length;
                             //Optimize
                             if (absolutePosition < currentLength)
                             {
                                 //segmentFound = true;
                                 segmentSplit = currentSegment.Value;
                                 location = (absolutePosition - (currentLength - segmentSplit.Length)) / segmentSplit.Length;
                                 break;
                             }
                             if (absolutePosition > length - 0.01f)
                             {
                                 segmentSplit = segments.Last.Value;
                                 location = (absolutePosition - (currentLength - segmentSplit.Length)) / segmentSplit.Length;
                                 break;
                             }
                             if (currentSegment.Next == null) throw new Exception("This is not possible, algorithm error");
                             currentSegment = currentSegment.Next;
                         }

                         segments.AddAfter(currentSegment, segmentSplit.Split(location));

                         childSegments.Add(segmentSplit);
                    }
                }
            }
            else
            {
                for (int j = 0; j < numChilderen; j++)
                {
                      relativePosition = NextFloat(childLevel.BranchPositionFactor, j + 1, numChilderen);
                    relativePosition = MathHelper.Clamp(relativePosition, 0.001f, 1 - 0.001f);
                    relativePositions.Add(relativePosition);
                  absolutePosition = relativePosition*length;
                    
                    float location = 0;
                LinkedListNode<TreeStructureSegment> currentSegment = segments.First;
                float currentLength = 0;
                //bool segmentFound = false;
                while (true)
                {
                    currentLength += currentSegment.Value.Length;
                    //Optimize
                    if (absolutePosition < currentLength)
                    {
                        //segmentFound = true;
                        segmentSplit = currentSegment.Value;
                        location = (absolutePosition - (currentLength - segmentSplit.Length)) / segmentSplit.Length;
                        break;
                    }
                    if (absolutePosition > length - 0.01f)
                    {
                        segmentSplit = segments.Last.Value;
                        location = (absolutePosition - (currentLength - segmentSplit.Length)) / segmentSplit.Length;
                        break;
                    }
                    if (currentSegment.Next == null) throw new Exception("This is not possible, algorithm error");
                    currentSegment = currentSegment.Next;
                }

                segments.AddAfter(currentSegment, segmentSplit.Split(location));

                childSegments.Add(segmentSplit);
                }
            }




            if (childLevel.Steps)
            {
                int i = 0;
                for (int j = 0; j < amountOfSteps; j++)
                {
                    for (int k = 0; k < branchesPerStep[j]; k++)
                    {
                        GenerateBranch(childSegments[i], iLevel + 1, i, branchesPerStep[j],positionPerStep[j]);
                        i++;
                    }
                }
            }
            else
            {

                for (int i = 0; i < childSegments.Count; i++)
                {
                    GenerateBranch(childSegments[i], iLevel + 1, i, numChilderen,relativePositions[i]);

                }
            }
            relativePositions=null;
        }



        private static TreeStructureSegment CreateSegment(TreeTypeLevel level, int levelIndex, TreeStructureSegment prevSegment, Directions direction,int textureIndex, List<TreeStructureLeaf> leaves)
        {
            TreeStructureSegment segment = new TreeStructureSegment();
            segment.Length = level.BranchMaxSegmentLength;
            segment.Direction = direction;
            segment.Leaves = leaves;
            //segment.LevelIndex = levelIndex;
            segment.LevelTextureData = new LevelAndTexureData(levelIndex, textureIndex);
            prevSegment.Children.Add(segment);
            return segment;
        }

        //temp public
        public TreeStructureLeaf CreateLeave(TreeLeafType leafType, Directions dir, float relativeDropAngle, float orientationAxialSplit, float positionAxialSplit, float distanceInSegment, int index,int iLevel)
        {
            TreeStructureLeaf leaf = new TreeStructureLeaf();
            leaf.Length = leafSeeder.NextFloat(leafType.Length.Min,leafType.Length.Max);
            leaf.Width = leafSeeder.NextFloat(leafType.Width.Min, leafType.Width.Max);
            leaf.DistanceFromTrunk = leafSeeder.NextFloat(leafType.DistanceFromTrunk.Min, leafType.DistanceFromTrunk.Max);
            Directions direction = Directions.DirectionsFromAngles(dir, relativeDropAngle, positionAxialSplit);//turns the leaf around the tree and  drops the leaf according to the tree
            //direction = Directions.DirectionsFromAngles(direction, 0, orientationAxialSplit);//rotates the leaf for a good looking orientation
            leaf.Direction.Add(direction);
            leaf.AxialSplit = positionAxialSplit;
            
            leaf.Position = distanceInSegment;
            //leaf.Index = index;

            //volumetric
            leaf.volumetricLeave = leafType.VolumetricLeaves;
            leaf.BillboardLeaf = leafType.BillBoardLeaf;
            if (leaf.volumetricLeave)
            {
                //leaf.Flower = leafType.Flower;
                leaf.FaceCountWidth = leafType.FaceCountWidth;
                leaf.BendingWidth = new List<float>();
                float bendingWidth, bendingLength;
                bendingWidth = leafSeeder.NextFloat(leafType.BendingWidth.Min, leafType.BendingWidth.Max) / (leafType.FaceCountWidth * 0.5f);
                bendingLength = leafSeeder.NextFloat(leafType.BendingLength.Min, leafType.BendingLength.Max) / (leafType.FaceCountLength - 1);
                for (int i = 0; i < leaf.FaceCountWidth/2; i++)
                {
                    leaf.BendingWidth.Add(leafSeeder.NextFloat(leafType.BendingWidth.Min, leafType.BendingWidth.Max));
                }
                Directions directionsForLength = leaf.Direction[0];
                for (int i = 0; i < leafType.FaceCountLength; i++) // make some more realistic gravity bending
                {
                    //dir = Directions.DirectionsFromAngles(dir, 0, positionAxialSplit);
                    dir = Directions.DirectionsFromAngles(directionsForLength,
                                                          leafSeeder.NextFloat(leafType.BendingLength.Min, leafType.BendingLength.Max), 0);
                    leaf.Direction.Add(dir);
                    directionsForLength = dir;
                }
            }
            leaf.LevelTextureData = new LevelAndTexureData(iLevel, index);
            return leaf;

        }

        public float NextFloat(RangeSpreading spreading, int index, int count)
        {
            if (count == 1)
            {
                return seeder.NextFloat(spreading.Min, spreading.Max);
            }
            float oneOverCount = 1f / count;
            float deviation = seeder.NextFloat(-oneOverCount * spreading.Deviation, oneOverCount * spreading.Deviation);
            return (index * oneOverCount + deviation) * (spreading.Max - spreading.Min) + spreading.Min;

        }
    }
}
