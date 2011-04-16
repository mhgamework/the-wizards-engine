using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards;
using Microsoft.Xna.Framework;
using TreeGenerator.NoiseGenerater;

namespace TreeGenerator.TerrrainGeneration
{
    public class ProceduralHeigthGenerater
    {
        private PerlinNoiseGenerater noise = new PerlinNoiseGenerater();
        public ProceduralHeigthGenerater(int octaves, float persistance)
        {
            noise.NumberOfOctaves = octaves;
            noise.persistance = persistance;
        }

        public float[,] GenerateErosion(float[,] HeightData, float initerations, float smoothness)
        {
            int width = HeightData.GetLength(0);
            int length = HeightData.GetLength(1);
            for (int i = 0; i < initerations; i++)
            {
                HeightData = singleErosionCycle(HeightData, width, length, smoothness);

            }

            return HeightData;
        }

        private float[,] singleErosionCycle(float[,] HeightData, int width, int length, float smoothness)
        {
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < length - 1; j++)
                {
                    float d_max = 0.0f;
                    int[] match = { 0, 0 };

                    for (int u = -1; u <= 1; u++)
                    {
                        for (int v = -1; v <= 1; v++)
                        {
                            if (Math.Abs(u) + Math.Abs(v) > 0)
                            {
                                float d_i = HeightData[i, j] - HeightData[i + u, j + v];
                                if (d_i > d_max)
                                {
                                    d_max = d_i;
                                    match[0] = u; match[1] = v;
                                }
                            }
                        }
                    }
                    if (0 < d_max && d_max <= (smoothness / (float)width))
                    {
                        float d_h = 0.5f * d_max;
                        HeightData[i, j] -= d_h;
                        HeightData[i + match[0], j + match[1]] += d_h;
                    }
                }
            }
            return HeightData;
        }
        public float[,] GenerateHydrolicErrosion(float[,] heightData, int initerations, int width, int length)
        {
            float[,] data = heightData;
            Seeder seeder = new Seeder(123);
            for (int i = 0; i < initerations; i++)
            {
                data = singleWaterCycle(data, seeder.NextInt(1, width - 1), seeder.NextInt(1, length - 1), width, length, 50);
            }
            return data;
        }
        public float[,] GenerateHydrolicErrosion(float[,] heightData, int initerations, float lifeTime, int width, int length)
        {
            float[,] data = heightData;
            Seeder seeder = new Seeder(123);
            for (int i = 0; i < initerations; i++)
            {
                data = singleWaterCycle(data, seeder.NextInt(1, width - 1), seeder.NextInt(1, length - 1), width, length, lifeTime);
            }
            return data;
        }
        public float[,] GenerateHydrolicErrosion(float[,] heightData, int initerations, float lifeTime, int width, int length, float maximumHeight, float minimumHeight)
        {
            float[,] data = heightData;
            Seeder seeder = new Seeder(123);
            for (int i = 0; i < initerations; i++)
            {
                int x = seeder.NextInt(1, width - 1);
                int y = seeder.NextInt(1, length - 1);
                if (heightData[x, y] > minimumHeight && heightData[x, y] < maximumHeight)
                {
                    data = singleWaterCycle(data, x, y, width, length, lifeTime);
                }
                else
                {

                }

            }
            return data;
        }
        private float[,] singleWaterCycle(float[,] heightData, int startX, int startY, int width, int length, float lifeTime)
        {

            float sedementPickUp = 0.03f;
            IndexStruct lowest = new IndexStruct(startX, startY);
            IndexStruct Currentlowest = new IndexStruct(startX, startY);
            float previousHeigthDifference = 0;
            float heightDifference = 0;
            float sediment = 0;
            float sedimentsolved = 0;
            for (int i = 0; i < lifeTime; i++)
            {
                if (Currentlowest.X > 0 && Currentlowest.Y > 0 && Currentlowest.X < width - 1 && Currentlowest.Y < length - 1)
                {
                    Currentlowest = findlowest(heightData, lowest);
                    heightDifference = heightData[lowest.X, lowest.Y] - heightData[Currentlowest.X, Currentlowest.Y];
                    sediment = heightDifference * sedementPickUp * ((lifeTime - i) / (lifeTime)) - sedimentsolved;
                    sedimentsolved += sediment;
                    heightData[lowest.X, lowest.Y] += -sediment;
                    lowest = Currentlowest;
                    previousHeigthDifference = heightDifference;
                }
                else { return heightData; }
            }
            return heightData;
        }
        public void riverSimulation(float[,] heightData, List<IndexStruct> startPoints, int initerations)
        {
            for (int i = 0; i < initerations; i++)
            {
                for (int j = 0; j < startPoints.Count; j++)
                {
                    riverCycle(heightData, startPoints[j]);
                }
            }
        }
        private void riverCycle(float[,] heightData, IndexStruct index)
        {
            float sedementPickUp = 0.03f;
            IndexStruct lowest = index;
            IndexStruct Currentlowest = index;
            float previousHeigthDifference = 0;
            float heightDifference = 0;
            float sediment = 0;
            float sedimentsolved = 0;
            List<IndexStruct> surroundingLowest;
            while (heightData[lowest.X, lowest.Y] > 0)
            {
                surroundingLowest = findlowestForRiver(heightData, lowest, 0.01f);
                for (int i = 1; i < surroundingLowest.Count; i++)
                {
                    riverCycle(heightData, surroundingLowest[i]);
                }
                Currentlowest = surroundingLowest[0];
                heightDifference = heightData[lowest.X, lowest.Y] - heightData[Currentlowest.X, Currentlowest.Y];
                sediment = heightDifference * sedementPickUp - sedimentsolved;
                sedimentsolved += sediment;
                heightData[lowest.X, lowest.Y] += -sediment;
                lowest = Currentlowest;
            }
        }
        private IndexStruct findlowest(float[,] heightData, IndexStruct XY)
        {
            float lowest = heightData[XY.X, XY.Y];
            IndexStruct lowestXY = XY;
            if (lowest > heightData[XY.X + 1, XY.Y])
            {
                lowest = heightData[XY.X + 1, XY.Y];
                lowestXY = new IndexStruct(XY.X + 1, XY.Y);
            }
            if (lowest > heightData[XY.X, XY.Y + 1])
            {
                lowest = heightData[XY.X, XY.Y + 1];
                lowestXY = new IndexStruct(XY.X, XY.Y + 1);
            }
            if (lowest > heightData[XY.X - 1, XY.Y])
            {
                lowest = heightData[XY.X - 1, XY.Y];
                lowestXY = new IndexStruct(XY.X - 1, XY.Y);
            }
            if (lowest > heightData[XY.X - 1, XY.Y - 1])
            {
                lowest = heightData[XY.X - 1, XY.Y - 1];
                lowestXY = new IndexStruct(XY.X - 1, XY.Y - 1);
            }
            if (lowest > heightData[XY.X + 1, XY.Y + 1])
            {
                lowest = heightData[XY.X + 1, XY.Y + 1];
                lowestXY = new IndexStruct(XY.X + 1, XY.Y + 1);
            }
            if (lowest > heightData[XY.X + 1, XY.Y - 1])
            {
                lowest = heightData[XY.X + 1, XY.Y - 1];
                lowestXY = new IndexStruct(XY.X + 1, XY.Y - 1);
            }
            if (lowest > heightData[XY.X - 1, XY.Y + 1])
            {
                lowest = heightData[XY.X - 1, XY.Y + 1];
                lowestXY = new IndexStruct(XY.X - 1, XY.Y + 1);
            }
            if (lowest > heightData[XY.X, XY.Y - 1])
            {
                lowest = heightData[XY.X, XY.Y - 1];
                lowestXY = new IndexStruct(XY.X, XY.Y - 1);
            }

            return lowestXY;
        }
        private List<IndexStruct> findlowestForRiver(float[,] heightData, IndexStruct XY, float maxDifference)
        {
            List<IndexStruct> indices = new List<IndexStruct>();
            float lowest = heightData[XY.X + 1, XY.Y];
            IndexStruct lowestXY = new IndexStruct(XY.X + 1, XY.Y);
            if (lowest > heightData[XY.X + 1, XY.Y])
            {
                lowest = heightData[XY.X + 1, XY.Y];
                lowestXY = new IndexStruct(XY.X + 1, XY.Y);
            }
            if (lowest > heightData[XY.X, XY.Y + 1])
            {
                lowest = heightData[XY.X, XY.Y + 1];
                lowestXY = new IndexStruct(XY.X, XY.Y + 1);
            }
            if (lowest > heightData[XY.X - 1, XY.Y])
            {
                lowest = heightData[XY.X - 1, XY.Y];
                lowestXY = new IndexStruct(XY.X - 1, XY.Y);
            }
            if (lowest > heightData[XY.X - 1, XY.Y - 1])
            {
                lowest = heightData[XY.X - 1, XY.Y - 1];
                lowestXY = new IndexStruct(XY.X - 1, XY.Y - 1);
            }
            if (lowest > heightData[XY.X + 1, XY.Y + 1])
            {
                lowest = heightData[XY.X + 1, XY.Y + 1];
                lowestXY = new IndexStruct(XY.X + 1, XY.Y + 1);
            }
            if (lowest > heightData[XY.X + 1, XY.Y - 1])
            {
                lowest = heightData[XY.X + 1, XY.Y - 1];
                lowestXY = new IndexStruct(XY.X + 1, XY.Y - 1);
            }
            if (lowest > heightData[XY.X - 1, XY.Y + 1])
            {
                lowest = heightData[XY.X - 1, XY.Y + 1];
                lowestXY = new IndexStruct(XY.X - 1, XY.Y + 1);
            }
            if (lowest > heightData[XY.X, XY.Y - 1])
            {
                lowest = heightData[XY.X, XY.Y - 1];
                lowestXY = new IndexStruct(XY.X, XY.Y - 1);
            }

            indices.Add(lowestXY);
            // now we have the lowest
            if (lowest > heightData[XY.X + 1, XY.Y] + maxDifference && lowest != heightData[XY.X + 1, XY.Y])
            {
                indices.Add(new IndexStruct(XY.X + 1, XY.Y));
            }
            if (lowest > heightData[XY.X, XY.Y + 1] + maxDifference && lowest != heightData[XY.X + 1, XY.Y])
            {
                indices.Add(new IndexStruct(XY.X, XY.Y + 1));

            }
            if (lowest > heightData[XY.X - 1, XY.Y] + maxDifference && lowest != heightData[XY.X + 1, XY.Y])
            {
                indices.Add(new IndexStruct(XY.X - 1, XY.Y));

            }
            if (lowest > heightData[XY.X - 1, XY.Y - 1] + maxDifference && lowest != heightData[XY.X + 1, XY.Y])
            {
                indices.Add(new IndexStruct(XY.X - 1, XY.Y - 1));

            }
            if (lowest > heightData[XY.X + 1, XY.Y + 1] + maxDifference && lowest != heightData[XY.X + 1, XY.Y])
            {
                indices.Add(new IndexStruct(XY.X + 1, XY.Y + 1));

            }
            if (lowest > heightData[XY.X + 1, XY.Y - 1] + maxDifference && lowest != heightData[XY.X + 1, XY.Y])
            {
                indices.Add(new IndexStruct(XY.X + 1, XY.Y - 1));

            }
            if (lowest > heightData[XY.X - 1, XY.Y + 1] + maxDifference && lowest != heightData[XY.X + 1, XY.Y])
            {
                indices.Add(new IndexStruct(XY.X - 1, XY.Y + 1));

            }
            if (lowest > heightData[XY.X, XY.Y - 1] + maxDifference && lowest != heightData[XY.X + 1, XY.Y])
            {
                indices.Add(new IndexStruct(XY.X, XY.Y - 1));

            }
            return indices;
        }
        public float IslandFactor(float x, float y, Vector2 center, float noiseFactor, float size)
        {
            float distance = Vector2.DistanceSquared(center, noise.Perturb(x, y, 1, noiseFactor));
            float heigth = (size*size - distance) / (size*size);
            return heigth;

        }
        public float[,] IslandFilter(float[,] heightData, float lowest, int initerations)
        {
            for (int i = 0; i < heightData.GetLength(0); i++)
            {
                for (int j = 0; j < heightData.GetLength(1); j++)
                {
                    if (heightData[i, j] < lowest)
                    {
                        heightData[i, j] = lowest;
                    }
                }
            }
            return GenerateHydrolicErrosion(heightData, initerations, 50, heightData.GetLength(0), heightData.GetLength(1), 0, lowest);

        }



        //water simulation, for river and water hydrolic generation
        public float[,] heightMap;
        private float[,] waterHeight;
         private float[,] waterHeight2;          
          
        private List<Spring> springs;
        int width, length;
        Seeder seeder;
        public void NewWaterSimulation(float[,] heightmap)
        {
            seeder = new Seeder(156);
            springs = new List<Spring>();
            heightMap = heightmap;
            width = heightmap.GetLength(0);
            length = heightmap.GetLength(1);
            waterHeight = new float[width, length];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j <length ; j++)
                {
                    if (heightmap[i,j]<0)
                    {
                        waterHeight[i, j] = -1;
                    }
                }
            }
        }
        public void WaterCycle()
        {
            AddSpringWater();
            for (int i = 1; i < width-1; i++)
            {
                for (int j = 1; j < length-1; j++)
                {
                    if (waterHeight[i,j]>0.000001f)
                    {
                        watergone=0;
                        DistributeWater(new IndexStruct(i, j));
                        waterHeight[i, j] -=watergone;
                    }
                    
                }
            }
        }
        public void AddRandomWaterDrops(int amountOfDrops, float debiet)
        {
            for (int i = 0; i < amountOfDrops; i++)
            {
                GetOldWaterMap()[seeder.NextInt(0, width), seeder.NextInt(0, length)] += debiet;
            }
        }
        private void AddSpringWater()
        {
            float maxWater=gridSize*gridSize*constantParam;
            for (int i = 0; i < springs.Count; i++)
            {
                Vector4 vec = getSurroundingHeight(GetOldWaterMap(), springs[i].Position) + getSurroundingHeight(heightMap, springs[i].Position);
                float waterOutput = springs[i].Debiet-GetOldWaterMap()[springs[i].Position.X,springs[i].Position.Y]; //+ heightMap[springs[i].Position.X,springs[i].Position.Y] - (vec.X + vec.Y + vec.Z + vec.W) * 0.25f;
                float waterOutFactor = waterOutput * TimeStep;
                if ((waterOutput*TimeStep >maxWater))
                {
                    waterOutFactor = maxWater / waterOutFactor;
                    if (waterOutFactor > 1.0) waterOutFactor = 1.0f;
                    waterOutput = waterOutput * waterOutFactor;
                }
                float waterOut = waterOutput / (springs[i].Radius * springs[i].Radius);
                    for (int k = -springs[i].Radius; k < springs[i].Radius; k++)
			        {
                        for (int l = -springs[i].Radius; l < springs[i].Radius; l++)
			            {
			                GetNewWaterMap()[springs[i].Position.X+k, springs[i].Position.Y+l] += waterOut*TimeStep;
                            GetNewErrosionMap()[springs[i].Position.X+k, springs[i].Position.Y+l].Y += waterOut * TimeStep * getDissolutionConstant(springs[i].Position) * velocityMap[springs[i].Position.X, springs[i].Position.Y].Length() * computeTiltAngle(springs[i].Position) * 0.75f;

			            }
			            
			        }
                
            }
        }
        public void AddSpring(IndexStruct Position, float debiet)
        {
            springs.Add(new Spring(Position, debiet));
        }
        public void AddSpring(IndexStruct Position, float debiet, int radius)
        {
            springs.Add(new Spring(Position, debiet,radius));
        }
        private void DistributeWater(IndexStruct index)
        {
            calculateSurroundingValues(index);
            singleDistribution(index, new IndexStruct(index.X + 1, index.Y));
            singleDistribution(index, new IndexStruct(index.X, index.Y + 1));
            singleDistribution(index, new IndexStruct(index.X - 1, index.Y));
            singleDistribution(index, new IndexStruct(index.X, index.Y - 1));
            singleDistribution(index, new IndexStruct(index.X + 1, index.Y + 1));
            singleDistribution(index, new IndexStruct(index.X - 1, index.Y - 1));
            singleDistribution(index, new IndexStruct(index.X + 1, index.Y - 1));
            singleDistribution(index, new IndexStruct(index.X - 1, index.Y + 1));
        }
        float watergone = 0;
        private void singleDistribution(IndexStruct indexFrom, IndexStruct indexTo)
        {
           
            if ((heightMap[indexFrom.X,indexFrom.Y]+waterHeight[indexFrom.X,indexFrom.Y])>(heightMap[indexTo.X,indexTo.Y]+waterHeight[indexTo.X,indexTo.Y]) && waterHeight[indexTo.X,indexTo.Y]!=-1)
            {
                waterHeight[indexTo.X, indexTo.Y] += waterHeight[indexFrom.X, indexFrom.Y] / lower;// + (((heightMap[indexFrom.X, indexFrom.Y] + waterHeight[indexFrom.X, indexFrom.Y]) - (heightMap[indexTo.X, indexTo.Y] + waterHeight[indexTo.X, indexTo.Y])) - meanLowest);
                watergone += waterHeight[indexFrom.X, indexFrom.Y] / lower;
            }

        }
        int lower;
        float meanLowest;
        private void calculateSurroundingValues(IndexStruct XY)
        {
             lower=0;;
             float totallowest=0;
            float lowest = heightMap[XY.X, XY.Y]+waterHeight[XY.X, XY.Y];
            if (lowest>(heightMap[XY.X+1,XY.Y]+waterHeight[XY.X+1, XY.Y]))
            {
               lower++;
                totallowest+=lowest-heightMap[XY.X+1,XY.Y]+waterHeight[XY.X+1, XY.Y];
            }
            if (lowest > heightMap[XY.X, XY.Y+1]+waterHeight[XY.X, XY.Y+1])
            {
                 lower++;
                totallowest+=lowest-heightMap[XY.X,XY.Y+1]+waterHeight[XY.X, XY.Y+1];
            }
            if (lowest > heightMap[XY.X-1, XY.Y]+waterHeight[XY.X-1, XY.Y])
            {
                lower++;
                totallowest+=lowest-heightMap[XY.X-1,XY.Y]+waterHeight[XY.X-1,XY.Y];
            }
            if (lowest > heightMap[XY.X-1, XY.Y-1]+waterHeight[XY.X-1, XY.Y-1])
            {
                 lower++;
                totallowest+=lowest-heightMap[XY.X-1,XY.Y-1]+waterHeight[XY.X-1, XY.Y-1];
            }
            if (lowest > heightMap[XY.X+1, XY.Y+1]+waterHeight[XY.X+1, XY.Y+1])
            {
                 lower++;
                totallowest+=lowest-heightMap[XY.X+1,XY.Y+1]+waterHeight[XY.X+1, XY.Y+1];
            }
            if (lowest > heightMap[XY.X+1, XY.Y-1]+waterHeight[XY.X+1, XY.Y-1])
            {
                lower++;
                totallowest+=lowest-heightMap[XY.X+1,XY.Y-1]+waterHeight[XY.X+1, XY.Y-1];
            }
            if (lowest > heightMap[XY.X-1, XY.Y+1]+waterHeight[XY.X-1, XY.Y+1])
            {
                lower++;
                totallowest+=lowest-heightMap[XY.X-1,XY.Y+1]+waterHeight[XY.X-1, XY.Y+1];
            }
            if (lowest > heightMap[XY.X, XY.Y-1]+waterHeight[XY.X, XY.Y-1])
            {
                lower++;
                totallowest+=lowest-heightMap[XY.X,XY.Y-1]+waterHeight[XY.X, XY.Y-1];
            }
            meanLowest = totallowest / lower;
        }

        // new design as in shaderfx
        private Vector4[,] flowMap;
        private Vector4[,] flowMap1;
        private float[,] waterHeight1;
        bool currentNewMap = true;
        public Vector4[,] GetNewFlowMap()
        {
            if (currentNewMap)
            {
                return flowMap;
            }
            return flowMap1;
        }
        public Vector4[,] GetOldFlowMap()
        {
            if (!currentNewMap)
            {
                return flowMap;
            }
            return flowMap1;
        }
        public float[,] GetNewWaterMap()
        {
            if (currentNewMap)
            {
                return waterHeight2;
            }
            return waterHeight1;
        }
        public float[,] GetOldWaterMap()
        {
            if (!currentNewMap)
            {
                return waterHeight2;
            }
            return waterHeight1;
        }
        private void switchMaps()
        {
            if (currentNewMap)
            { currentNewMap = false; waterHeight1 = new float[width, length]; }
            else { currentNewMap = true; waterHeight2 = new float[width, length]; }
        }
        float gridSize;
        public float TimeStep;
        public float Damping;
        private float evaperation;
        float minEvaperation = 0.000001f;
        public float Evaperation
        {
            get
            {
                return evaperation;
            }
            set
            {
                if (value<evaperation)
                    return;
                evaperation = value;
            }
        }
        float constantParam;
        public void newWaterSystem(float[,] heightmap,float _gridSize, float timeStep, float damping, float _constantParam,float _evaperation)
        {
            //the old
            seeder = new Seeder(156);
            springs = new List<Spring>();
            heightMap = heightmap;
            width = heightmap.GetLength(0);
            length = heightmap.GetLength(1);
            waterHeight = new float[width, length];
            //the new
            gridSize = _gridSize;
            TimeStep = timeStep;
            Damping = damping;
            constantParam = _constantParam;
            flowMap = new Vector4[width, length];
            flowMap1 = new Vector4[width, length];
            waterHeight1 = new float[width, length];
            waterHeight2 = new float[width, length];
            Evaperation = _evaperation;

            velocityMap = new Vector2[width, length];
            //clouds
            clouds = new List<Cloud>();
            //errosion
            ErrosionMap = new Vector2[width, length];
            ErrosionMap2 = new Vector2[width, length];
            HeightDiff=new float[width,length];
        }
        private void computeOutflow(Vector4 diff,Vector4 water ,IndexStruct index)
        {
            Vector4 param = Vector4.Min( new Vector4(Math.Abs(diff.X),Math.Abs(diff.Y),Math.Abs(diff.Z),Math.Abs(diff.W)), (new Vector4(GetOldWaterMap()[index.X, index.Y]) + water) * 0.5f);
            param = Vector4.Min(new Vector4(gridSize), param);
            Vector4 newFlow = Damping * GetOldFlowMap()[index.X, index.Y] + constantParam * diff * param;
            newFlow = Vector4.Max(Vector4.Zero, newFlow);

            float maxWater = gridSize * gridSize * GetOldWaterMap()[index.X, index.Y];
            float waterOutFactor = (newFlow.X + newFlow.Y + newFlow.W + newFlow.Z) * TimeStep;
            if ((waterOutFactor > GetOldWaterMap()[index.X, index.Y]))//should be EROSIM_NUM_ZERO_GENERAL
            {
               
                waterOutFactor = maxWater / waterOutFactor;
                if (waterOutFactor > 1.0) waterOutFactor = 1.0f;
                newFlow = newFlow * waterOutFactor;
            }
            
           
            updateFlowMap(newFlow, index);

        }
        private void updateFlowMap(Vector4 flow, IndexStruct index)
        {
            GetNewWaterMap()[index.X,index.Y]+= MathHelper.Max(GetOldWaterMap()[index.X, index.Y]-(flow.X+flow.Y+flow.Z+flow.W) - evaperation * TimeStep, 0);
            GetNewWaterMap()[index.X + 1, index.Y] += flow.X * TimeStep;
            GetNewWaterMap()[index.X - 1, index.Y] += flow.Y * TimeStep;
            GetNewWaterMap()[index.X, index.Y + 1] += flow.Z * TimeStep;
            GetNewWaterMap()[index.X, index.Y - 1] += flow.W * TimeStep;

            GetNewFlowMap()[index.X, index.Y] = flow;
        }
        private void ComputeWaterOutFlow(IndexStruct index)
        {
            if (GetOldWaterMap()[index.X, index.Y] > 0)
            { 
            Vector4 height= getSurroundingHeight(heightMap,index);
            Vector4 water = getSurroundingHeight(GetOldWaterMap(), index);
            float heightC= heightMap[index.X,index.Y]+GetOldWaterMap()[index.X,index.Y];
            Vector4 diff = new Vector4();
            diff = new Vector4(heightC) - water - height;
            computeOutflow(diff, water, index);
            }
            
        }
        public void ComputeOneWaterCycleNew()
            {
                
                for (int i = 1; i < width - 1; i++)
                {
                    for (int j = 1; j < length - 1; j++)
                    {
                        ComputeWaterOutFlow(new IndexStruct(i,j));
                        if (heightMap[i,j]<0)
                        {
                            GetNewWaterMap()[i, j] = 0;
                        }
                    }
                }
                AddSpringWater();
                switchMaps();
            }
        public Vector2[,] velocityMap;
        public void ComputeVelocityMap()
        {
            for (int i = 1; i < width-1; i++)
            {
                for (int j = 1; j < length-1; j++)
                {
                    //if (GetOldWaterMap()[i,j]>0.001f)
                    //{
                        velocityMap[i, j] = computeVelocity(new IndexStruct(i, j));
                    //}
                }
            }
        }
        private Vector2 computeVelocity(IndexStruct position)
        {
            Vector4 flowC = GetOldFlowMap()[position.X, position.Y];
            Vector4 flowL = GetOldFlowMap()[position.X-1, position.Y];
            Vector4 flowR = GetOldFlowMap()[position.X+1, position.Y];
            Vector4 flowT = GetOldFlowMap()[position.X, position.Y-1];
            Vector4 flowB = GetOldFlowMap()[position.X, position.Y+1];

            Vector2 velocityData = new Vector2();
            velocityData.X = -(flowL.Y + flowC.Y - flowR.X - flowC.X) / 2.0f;
            velocityData.Y = -(flowT.W + flowC.W- flowB.Z - flowC.Z) / 2.0f;

            float velocityMax = gridSize * (GetNewWaterMap()[position.X, position.Y] + GetOldWaterMap()[position.X, position.Y]) * 0.5f;
            float velocityFactor=velocityData.Length();
            if (velocityFactor>velocityMax)
            {
                velocityFactor = velocityMax / velocityFactor;
                if (velocityFactor > 1.0f) velocityFactor = 1.0f;
                velocityData *= velocityFactor;
            }
            return velocityData;
        }

        //errosion calculations
        public float[,] HeightDiff;
        public void ComputeErosion()
        {
            ComputeVelocityMap();
            newHeightMap = new float[width, length];
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < length - 1; j++)
                {
                    ComputeSingleErosion(new IndexStruct(i, j));
                }
            }
            heightMap = newHeightMap;
            SwitchErrosionMaps();
        }
        private Vector2[,]ErrosionMap;
        private Vector2[,] ErrosionMap2;
        bool currentNewErrosionMap = true;
        public Vector2[,] GetNewErrosionMap()
        {
            if (currentNewErrosionMap)
            {
                return ErrosionMap;
            }
            return ErrosionMap2;
        }
        public Vector2[,] GetOldErrosionMap()
        {
            if (currentNewErrosionMap)
            {
                return ErrosionMap2;
            }
            return ErrosionMap;
        }
        public void SwitchErrosionMaps()
        {
            if (currentNewErrosionMap)
            {
                currentNewErrosionMap = false;
            }
            else { currentNewErrosionMap = true; }
        }
        private float getPushConstant(IndexStruct Pos)
        {
            return 0.01f;
        }
        private float getDissolutionConstant(IndexStruct pos)
        {

            //for (int q = 0; q < springs.Count; q++)
            //{
            //    if (pos.X==springs[q].Position.X&&pos.X==springs[q].Position.X)
            //    {
            //        return 0.001f;
            //    }
            //}
            return 0.000000001f;
        }
        float minTiltAngle=0.01f;
        float depositionC=0.01f;
        float SedimentCapacity=0.5f;
        private void computeErosionAndDeposition(IndexStruct position)// needs major clean up and implementation on  the gpu forsure
        {
            float tiltAngle=computeTiltAngle(position);
            if (tiltAngle < minTiltAngle) tiltAngle = minTiltAngle;
            float sedimentCapacityFactor = tiltAngle * velocityMap[position.X, position.Y].Length();
            float finalMaxSediment = SedimentCapacity * sedimentCapacityFactor;
            float totalSedimentDif = 0;
            float sedimentC= GetOldErrosionMap()[position.X,position.Y].Y;
            if (sedimentC > finalMaxSediment)
            {
                 totalSedimentDif -=depositionC * (sedimentC - finalMaxSediment);
            }
            else
            {
                float maxS=finalMaxSediment;
                totalSedimentDif=getDissolutionConstant(position)*(maxS-sedimentC);
            }
            
            // change the terrains height
            heightMap[position.X, position.Y] -= totalSedimentDif;
            advectSediment(position, totalSedimentDif);
        }
        float velocityFactor=1.0f;
        private void advectSediment(IndexStruct position, float sedimentDif)
        {
            float sediment = sedimentDif + GetOldErrosionMap()[position.X, position.Y].Y;
            Vector2 velocity = velocityMap[position.X, position.Y] * velocityFactor;
            if (velocity.X < 0)
            {
                GetNewErrosionMap()[position.X - 1, position.Y].Y += velocity.X * sediment;
            }
            else
            {
                GetNewErrosionMap()[position.X + 1, position.Y].Y += velocity.X * sediment;
            }
            if (velocity.Y < 0)
            {
                GetNewErrosionMap()[position.X, position.Y - 1].Y += velocity.Y * sediment;
            }
            else
            {
                GetNewErrosionMap()[position.X, position.Y + 1].Y += velocity.Y * sediment;
            }
        }

        //home brew
        private void ComputeSingleErosion(IndexStruct position)
        {
            if (velocityMap[position.X, position.Y].LengthSquared() > 0.00001f)
            {
                float tiltAngle = computeTiltAngle(position);
                float sedimentCapacityFactor = tiltAngle * velocityMap[position.X, position.Y].Length();
                // disolving
                float sedimentDisolved = sedimentCapacityFactor * getDissolutionConstant(position);
                float diff = sedimentDisolved - GetOldErrosionMap()[position.X, position.Y].Y;
                newHeightMap[position.X, position.Y] += heightMap[position.X, position.Y] - diff;
                //push
                if (tiltAngle > 0.001f)
                {
                    int klqsdf = 0;
                }
                float draggedSediment = sedimentCapacityFactor * getPushConstant(position);
                newHeightMap[position.X, position.Y] -= draggedSediment;

                distributeErosionSediment(position, sedimentDisolved, draggedSediment);
            }
            else { newHeightMap[position.X, position.Y] += heightMap[position.X, position.Y]; }
        }

        private void distributeErosionSediment(IndexStruct position, float disolvedAmount, float draggedAmount)
        {
            Vector4 flow = GetOldFlowMap()[position.X, position.Y];
            Vector2 velocity = velocityMap[position.X, position.Y];
            velocity.Normalize();
            float lost = 0f;
            if (flow.X>0)
            {
                GetNewErrosionMap()[position.X-1, position.Y].Y += disolvedAmount * flow.X;
                lost += disolvedAmount * flow.X;
            }
            if (flow.Y > 0)
            {
                GetNewErrosionMap()[position.X+1, position.Y].Y += disolvedAmount * flow.Y;
                lost += disolvedAmount * flow.Y;
               
            }
            if (flow.Z > 0)
            {
                GetNewErrosionMap()[position.X, position.Y-1].Y += disolvedAmount * flow.Z;
                lost += disolvedAmount * flow.Z;
             
            }
            if (flow.W > 0)
            {
                GetNewErrosionMap()[position.X, position.Y+1].Y += disolvedAmount * flow.W;
                lost += disolvedAmount * flow.W;
              
            }
            GetNewErrosionMap()[position.X, position.Y].Y += disolvedAmount - lost;
           
             
            if (velocity.X > 0)
            {
                newHeightMap[position.X+1, position.Y] += velocity.X * draggedAmount;
            }
            else { newHeightMap[position.X-1, position.Y] += velocity.X * draggedAmount; }
            if (velocity.Y > 0)
            {
                newHeightMap[position.X, position.Y+1] += velocity.Y * draggedAmount;
            }
            else { newHeightMap[position.X, position.Y-1] += velocity.Y * draggedAmount; }
            
            
        }
        private float computeTiltAngle(IndexStruct position)
        {
            Vector3 norm= new Vector3(heightMap[position.X-1,position.Y]-heightMap[position.X+1,position.Y],heightMap[position.X,position.Y-1]-heightMap[position.X,position.Y+1],2*gridSize);
            norm.Normalize();
            return (float)Math.Abs(Math.Sin(Math.Acos(Vector3.Dot(norm,new Vector3(0,0,1)))));
        }
        private Vector4 getSurroundingHeight(float[,] data, IndexStruct index)
        {
            Vector4 surroundings=new Vector4();
            surroundings.X = data[index.X+1, index.Y];
            surroundings.Y= data[index.X-1 , index.Y];
            surroundings.Z = data[index.X , index.Y+ 1];
            surroundings.W = data[index.X , index.Y-1];

            return surroundings;
        }
        public struct IndexStruct
        {
            public int X;
            public int Y;

            public IndexStruct(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        public struct Spring
        {
            public IndexStruct Position;
            public float Debiet;
             public int Radius;
            public Spring(IndexStruct position, float debiet)
            {
                Position = position;
                Debiet = debiet;
                Radius = 3;
            }
            public Spring(IndexStruct position, float debiet,int radius)
            {
                Position = position;
                Debiet = debiet;
                Radius = radius;
            }
        }

        public void CutOutRivers(float deepnessFactor)// further improvements might be an average waterheight map
        {
            for (int i =1; i < (width-1); i++)
            {
                for (int j = 1; j < (length-1); j++)
                {
                    if (GetOldWaterMap()[i,j]>0.000001f)
                    {
                        heightMap[i,j] -= (GetOldWaterMap()[i, j]*0.1f + velocityMap[i, j].Length()*0.9f) * deepnessFactor*getPushConstant(new IndexStruct(i,j));

                    }
                }
            }
        }
        
        // rainSimulation
        Seeder seed = new Seeder(1368);
        private Vector2 currentWindDirection=new Vector2(0,1);
        public Vector4 MeanWindDirection;
        public  List<Cloud> clouds;
        public void GenerateClouds(int amount)
        {
            while (amount>0)
            {
                int x=seed.NextInt(0,width);
                int y=seed.NextInt(0,length);
                if (heightMap[x,y]<0)// next thing to see how this works is if clouds can be create where the waterlevel is high enough
                {
                    amount--;
                    clouds.Add(new Cloud(x,y,10,10,300));
                }
            }
        }
        public void AnimateClouds()
        {
            for (int i = 0; i < clouds.Count; i++)
            {
                clouds[i].MoveCloud(currentWindDirection, TimeStep);
                int x = (int)clouds[i].Position.X;
                int y = (int)clouds[i].Position.Y;
                if ((x < 0 | y < 0 || x >= width || y >= length))
                {
                    clouds.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (heightMap[x, y] > 0)
                    {


                        clouds[i].OutflowSpeed = heightMap[x, y]*heightMap[x, y] * 0.0003f;
                        float waterOut = clouds[i].OutflowSpeed * TimeStep;

                        if (clouds[i].Water < clouds[i].Width * clouds[i].Length * clouds[i].OutflowSpeed * TimeStep)
                        {
                            waterOut = clouds[i].Water / (clouds[i].Width * clouds[i].Length);
                        }
                        for (int k = -(int)(clouds[i].Width * 0.5f); k < (int)(clouds[i].Width * 0.5f); k++)
                        {
                            for (int l = -(int)(clouds[i].Length * 0.5f); l < (int)(clouds[i].Length * 0.5f); l++)
                            {
                                int xx = x + k;
                                int yy = y + l;
                                if (xx > 0 && xx < width - 1 && yy > 0 && yy < length - 1)
                                {
                                    GetNewWaterMap()[xx, yy] += waterOut;
                                }
                            }
                        }
                        clouds[i].Water -= clouds[i].Width * clouds[i].Length * waterOut;
                        if (clouds[i].Water < 0.00001f)
                        {
                            clouds.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }
        public class Cloud
        {
            public Vector2 Position;
            public int Width;
            public int Length;
            public float Water;
            public float OutflowSpeed;
            public Cloud(float x, float y,int width,int length,float waterAmount)
               {
                Position = new Vector2(x, y);
                Width=width;
                Length = length;
                Water = waterAmount;
                OutflowSpeed = 0;
               }
            public void MoveCloud(Vector2 windDirection,float timeStep)
            {
                Position += windDirection * timeStep;
            }
        }

        // ThermalErrosion
        float[,] newHeightMap;
        
        private float getThermalC(int x, int y)
        {
            return 0.01f;//seed.NextFloat(0, 0.1f);
        }
        public void SingleThermalErosionCycle()
        {
            newHeightMap = new float[width, length];
            for (int i = 1; i < width-1; i++)
			{
			 for (int j = 1; j < length-1; j++)
			    {
			        ComputerSingleThermalCycle(i,j);
			    }
			}
            
        }
		private void  ComputerSingleThermalCycle(int x, int y)// approciation on the edges not working !! like it should can't find the problem
        {
            Vector4 HeightDiff = new Vector4(heightMap[x, y]) - getSurroundingHeight(heightMap,new IndexStruct(x,y));
            float totalDiff = 0;
            float MinDiff = 0.1f;
            float MaxDiff=MinDiff;
            
            if (HeightDiff.X > MinDiff)
            {
                totalDiff += HeightDiff.X;
                if (HeightDiff.X>MaxDiff)
                {
                    MaxDiff = HeightDiff.X;
                }
            }
            else { HeightDiff.X = 0; }
            if (HeightDiff.Y > MinDiff)
                {
                    totalDiff += HeightDiff.Y;
                    if (HeightDiff.Y > MaxDiff)
                    {
                        MaxDiff = HeightDiff.Y;
                    }
                }
                else { HeightDiff.Y = 0; }
            if (HeightDiff.Z > MinDiff)
                {
                    totalDiff += HeightDiff.Z;
                    if (HeightDiff.Z > MaxDiff)
                    {
                        MaxDiff = HeightDiff.Z;
                    }
                }
                else { HeightDiff.Z = MinDiff; }
            if (HeightDiff.W > MinDiff)
                {
                    totalDiff += HeightDiff.W;
                    if (HeightDiff.W > MaxDiff)
                    {
                        MaxDiff = HeightDiff.W;
                    }
                }
                else { HeightDiff.W = 0; }
           

                if (totalDiff>0.00000001f)
                {
                    heightMap[x + 1, y] += (MaxDiff-MinDiff)*(HeightDiff.X / totalDiff) * getThermalC(x , y);
                    heightMap[x - 1, y] += (MaxDiff - MinDiff) * (HeightDiff.Y / totalDiff) * getThermalC(x, y);
                    heightMap[x, y + 1] += (MaxDiff - MinDiff) * (HeightDiff.Z / totalDiff) * getThermalC(x, y);
                    heightMap[x, y - 1] += (MaxDiff - MinDiff) * (HeightDiff.W / totalDiff) * getThermalC(x, y);
                    heightMap[x, y] -= (MaxDiff - MinDiff) * getThermalC(x, y);
                }
        } 

    }
}
