using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;
using TreeGenerator.Clouds;
using TreeGenerator.help;
using MHGameWork.TheWizards.Graphics;

namespace TreeGenerator.TreeEngine
{
    public class EngineTreeRenderDataGenerater
    {
        float vLevel = 0;
        float segLenght = 0;
        int numVertices;
        private float verticesCountDegradationEnd;
        public EngineTreeRenderData TreeRenderData;
        private Dictionary<TreeStructure, StructureRenderDatas> RenderDataDic = new Dictionary<TreeStructure, StructureRenderDatas>();
        TreeStructure treeStructure = new TreeStructure();
        TreeTypeData treeTypeData;

        public EngineTreeRenderDataGenerater(int _numVertices)//, float _verticesCountDegradationEnd)
        {

            numVertices = _numVertices;
            verticesCountDegradationEnd = 0.5f;// _verticesCountDegradationEnd;
        }

        int requestedLODLevel;
        private int totalAmountofLevels;
        public EngineTreeRenderData GetRenderData(TreeStructure treeStructure, IXNAGame game, int lodLevel)
        {
            StructureRenderDatas renderDatas;
            if (!RenderDataDic.TryGetValue(treeStructure, out renderDatas))
            {
                RenderDataDic[treeStructure] = renderDatas = new StructureRenderDatas();
                renderDatas.RenderDataLods = new EngineTreeRenderData[treeStructure.AmountOfLevels];
            }
            if (lodLevel > treeStructure.AmountOfLevels-1)
                lodLevel = treeStructure.AmountOfLevels-1;

            TreeRenderData = renderDatas.RenderDataLods[lodLevel];
            if (TreeRenderData == null)
            {
                GenerateRenderData(game, lodLevel, treeStructure);
                renderDatas.RenderDataLods[lodLevel] = TreeRenderData;
            }



            return TreeRenderData;
        }

        private void GenerateRenderData(IXNAGame game, int lodLevel, TreeStructure treeStructure)
        {
            TreeRenderData = new EngineTreeRenderData(game);


            requestedLODLevel = lodLevel;
            totalAmountofLevels = treeStructure.AmountOfLevels - 1;
            if (totalAmountofLevels != 0)
            {
                VerticeCountDegradationStep = verticesCountDegradationEnd / totalAmountofLevels;
            }
            else
            {
                VerticeCountDegradationStep = 1;
            }
            //some visual properties
            texCoordLength = treeStructure.TextureHeight;
            texCoordCircumference = treeStructure.TextureWidth;

            this.treeStructure = treeStructure;
            TreeRenderData.TreeBody = new EngineTreeRenderDataPart(Vector3.Zero);
            TreeRenderData.TreeBody.Texture = treeStructure.Textures[0];

            for (int i = 0; i < treeStructure.Textures.Count - 1; i++)
            {
                TreeRenderData.Leaves.Add(new EngineTreeRenderDataPart(Vector3.Zero));
                TreeRenderData.Leaves[i].Texture = treeStructure.Textures[i + 1];


            }


            Vector3 pos = Vector3.Zero;

            if (treeStructure.Base.Children.Count != 0)
            {
                CreateTrianglesForSegment(treeStructure.Base, treeStructure.Base.Children[0], pos, 0, 0);
                segLenght = treeStructure.Base.Length;
            }
            pos += treeStructure.Base.Direction.Heading * treeStructure.Base.Length;
            for (int j = 0; j < treeStructure.Base.Children.Count; j++)
            {
                segLenght = 0;
                CreateAllTtriangles(treeStructure.Base.Children[j], pos, segLenght, treeStructure.Base.Length);

            }
        }

        private void CreateAllTtriangles(TreeStructureSegment seg, Vector3 pos, float previousSegmentLenght, float currentLength)
        {
            if (seg.Children.Count != 0)
            {
                CreateTrianglesForSegment(seg, seg.Children[0], pos, previousSegmentLenght, currentLength);
                segLenght = seg.Length;
            }
            pos += seg.Direction.Heading * seg.Length;
            for (int i = 0; i < seg.Children.Count; i++)
            {
                segLenght = 0;
                CreateAllTtriangles(seg.Children[i], pos, segLenght, currentLength + seg.Children[i].Length);

            }

        }

        public void CreateTrianglesForSegment(TreeStructureSegment currentSegment, TreeStructureSegment nextSegment, Vector3 position, float previousSegmentLenght, float currentLength)
        {
            if (SegmentIsPartOfRightLOD(currentSegment.LevelTextureData.Level))
            {

                TangentVertex[] currentVertices = createVerticesForSegment(currentSegment, position, numVertices, vLevel, previousSegmentLenght, currentLength);
                TangentVertex[] nextVertices = createVerticesForSegment(nextSegment, position + currentSegment.Direction.Heading * currentSegment.Length, numVertices, vLevel + 1, currentSegment.Length, currentLength + currentSegment.Length);

                vLevel += 1;


                for (int i = 0; i < currentVertices.Length - 1; i++) //changed 
                {
                    TreeRenderData.TreeBody.Vertices1.Add(nextVertices[i]);
                    TreeRenderData.TreeBody.Vertices1.Add(currentVertices[i + 1]);
                     TreeRenderData.TreeBody.Vertices1.Add(currentVertices[i]);

                    TreeRenderData.TreeBody.Vertices1.Add(nextVertices[i]); 
                    TreeRenderData.TreeBody.Vertices1.Add(nextVertices[i + 1]);
                    TreeRenderData.TreeBody.Vertices1.Add(currentVertices[i+1]);
                    

                    
                   
                    


                }

            }

            if (currentSegment.Leaves != null)
            {
                createLeafVerticesForSegment(currentSegment, position);
            }


        }

        private float texCoordLength = 0.01f;
        private float texCoordCircumference = 1;
        private float VerticeCountDegradationStep = 0;
        private TangentVertex[] createVerticesForSegment(TreeStructureSegment segment, Vector3 position, int numVertices, float vlevel, float previousSegmentLenght, float currentLength)//make uv coords according to length and with of segment
        {
            if (segment.LevelTextureData.Level == 2)
            {
                int mklj = 0;
            }
            int numberOfVertices = (int)(numVertices - (numVertices * VerticeCountDegradationStep * segment.LevelTextureData.Level));
            TangentVertex[] vertices = new TangentVertex[numberOfVertices + 1];
            float r = segment.Radius / 2;
            double anglePerVer = (2 * MathHelper.Pi) / (numberOfVertices - 1);
            double angleVert = 0;

            float U = 0;
            float Uunit = (texCoordCircumference / (segment.Radius)) / numberOfVertices;

            float V = currentLength / (texCoordLength * segment.Radius);

            currentLength += segment.Length;
            for (int i = 0; i < vertices.Length - 1; i++)
            {

                Vector3 normal = (new Vector3((float) (Math.Sin(angleVert)), 0.0f, (float) (Math.Cos(angleVert))));
                normal.Normalize();
                vertices[i] = new TangentVertex(new Vector3((float)(r * Math.Sin(angleVert)), 0.0f, (float)(r * Math.Cos(angleVert))), new Vector2(U, V),normal,new Vector3((float)(Math.Cos(angleVert)), 0.0f, (float)(-Math.Sin(angleVert))));
                //vertices[i].pos*= segment.Radius;
                angleVert += anglePerVer;
                 U += Uunit;
            }
            vertices[vertices.Length - 1] = vertices[0];
            vertices[vertices.Length - 1].uv.X = 1;

            Vector3 cross;
            float angle;

            cross = Vector3.Cross(segment.Direction.Heading, Vector3.Up);
            angle = (float)Math.Acos((double)Vector3.Dot(segment.Direction.Heading, Vector3.Up));


            //game.LineManager3D.AddLine(Vector3.Zero, Vector3.Up, Color.Orange);
            //game.LineManager3D.AddLine(Vector3.Zero, cross, Color.Yellow);

            Matrix mRot;

            mRot = Matrix.CreateFromAxisAngle(cross, -angle);


            //mRot = mRot * Matrix.CreateFromAxisAngle(directions.Right, DropAngle);


            Matrix m = mRot * Matrix.CreateTranslation(position);
            for (int i = 0; i < vertices.Length; i++)
            {
                TangentVertex v = vertices[i];

                v.pos = Vector3.Transform(v.pos, m);
                v.tangent = Vector3.Transform(v.tangent, mRot);
                v.normal = Vector3.Transform(v.normal, mRot);
                vertices[i] = v;

            }
            return vertices;

        }

        public void createLeafVerticesForSegment(TreeStructureSegment segment, Vector3 position)
        {
            int index = 0;
            for (int i = 0; i < segment.Leaves.Count; i++)
            {
                index = segment.Leaves[i].LevelTextureData.TextureIndex - 1;
                TreeStructureLeaf leaf = segment.Leaves[i];
                Vector3 pos = position + segment.Direction.Heading * segment.Leaves[i].Position * segment.Length + (segment.Radius + leaf.DistanceFromTrunk) * leaf.Direction[0].Heading;//radius thing is not correct must be improved
                Directions dir = leaf.Direction[0];
                Vector3 cross;
                cross = Vector3.Cross(dir.Right, dir.Heading);
                cross.Normalize();
                if (leaf.volumetricLeave == false)
                {
                    if (leaf.BillboardLeaf)
                    {

                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos, new Vector2(0, 0), new Vector3(-1, 1, 0), new Vector3(leaf.Width, leaf.Length, 0)));
                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos, new Vector2(0, 1), new Vector3(-1,- 1, 0), new Vector3(leaf.Width, leaf.Length, 0)));
                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos, new Vector2(1, 1), new Vector3(1, -1, 0), new Vector3(leaf.Width, leaf.Length, 0)));

                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos, new Vector2(0, 0), new Vector3(-1, 1, 0), new Vector3(leaf.Width, leaf.Length, 0)));
                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos, new Vector2(1, 1), new Vector3(1, -1, 0), new Vector3(leaf.Width, leaf.Length, 0)));
                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos, new Vector2(1, 0), new Vector3(1, 1, 0), new Vector3(leaf.Width, leaf.Length, 0)));

                         TreeRenderData.Leaves[index].RenderAsBillBoards = true;
                    }
                    else
                    {
                        //rechthoek1
                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos - dir.Right * (leaf.Width / 2),
                                                                                     new Vector2(1, 1), cross, dir.Right));
                        TreeRenderData.Leaves[index].Vertices1.Add(
                            new TangentVertex(pos + dir.Right * (leaf.Width / 2) + dir.Heading * leaf.Length,
                                              new Vector2(0, 0), cross, dir.Right));
                        TreeRenderData.Leaves[index].Vertices1.Add(
                            new TangentVertex(pos - dir.Right * (leaf.Width / 2) + dir.Heading * leaf.Length,
                                              new Vector2(1, 0), cross, dir.Right));

                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos - dir.Right * (leaf.Width / 2),
                                                                                     new Vector2(1, 1), cross, dir.Right));
                        TreeRenderData.Leaves[index].Vertices1.Add(
                            new TangentVertex(pos + dir.Right * (leaf.Width / 2) + dir.Heading * leaf.Length,
                                              new Vector2(0, 0), cross, dir.Right));
                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos + dir.Right * (leaf.Width / 2),
                                                                                     new Vector2(0, 1), cross, dir.Right));




                        //rechthoek2
                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos - cross * (leaf.Width / 2),
                                                                                     new Vector2(1, 1), dir.Right, cross));
                        TreeRenderData.Leaves[index].Vertices1.Add(
                            new TangentVertex(pos + cross * (leaf.Width / 2) + dir.Heading * leaf.Length, new Vector2(0, 0),
                                              dir.Right, cross));
                        TreeRenderData.Leaves[index].Vertices1.Add(
                            new TangentVertex(pos - cross * (leaf.Width / 2) + dir.Heading * leaf.Length, new Vector2(1, 0),
                                              dir.Right, cross));

                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos + cross * (leaf.Width / 2),
                                                                                     new Vector2(0, 1), dir.Right, cross));
                        TreeRenderData.Leaves[index].Vertices1.Add(new TangentVertex(pos - cross * (leaf.Width / 2),
                                                                                     new Vector2(1, 1), dir.Right, cross));
                        TreeRenderData.Leaves[index].Vertices1.Add(
                            new TangentVertex(pos + cross * (leaf.Width / 2) + dir.Heading * leaf.Length, new Vector2(0, 0),
                                              dir.Right, cross));
                    }
                }
                else
                {
                    List<TangentVertex> volumetricLeaveVertices = CreateVerticesForVolumetricLeaf(leaf, pos);

                    for (int j = 0; j < volumetricLeaveVertices.Count; j++)
                    {
                        TreeRenderData.Leaves[index].Vertices1.Add(volumetricLeaveVertices[j]);
                    }


                }


            }


        }

        public List<TangentVertex> CreateVerticesForVolumetricLeaf(TreeStructureLeaf leaf, Vector3 position)
        {
            List<TangentVertex> vertices = new List<TangentVertex>();

            Vector2 UVStart = new Vector2(0, 0);
            Vector2 UVEnd = new Vector2(1, 1);

            if ((leaf.FaceCountWidth % 2) != 0)
            {
                leaf.FaceCountWidth--;
            }

            Vector3 pos = position;
            Directions dir = leaf.Direction[0];
            Vector3 cross;
            cross = Vector3.Cross(dir.Right, dir.Heading);
            cross.Normalize();
            float WidthSingle = leaf.Width / leaf.FaceCountWidth;
            float LengthSingle = leaf.Length / leaf.Direction.Count;
            float uvUnitLenth = (UVEnd.X - UVStart.X) / (leaf.Direction.Count-1);
            float uvUnitWidth = (UVEnd.Y - UVStart.Y) / leaf.FaceCountWidth;

            float WidthFaceHalf = leaf.FaceCountWidth / 2;

            Directions directionWidthCurrent, directionWidthNext;
            for (int i = 0; i < leaf.Direction.Count - 1; i++)// step one up in the length direction
            {
                if (i != 0)
                {
                    pos += leaf.Direction[i - 1].Heading * LengthSingle;
                }


                directionWidthCurrent = leaf.Direction[i];
                directionWidthNext = leaf.Direction[i + 1];
                Vector3 tempPositionCurrent = pos;
                Vector3 tempPositionNext = pos + leaf.Direction[i].Heading * LengthSingle;


                float UHalf = (UVEnd.X - UVStart.X) * 0.5f;
                int uvBackOrForward = 0;
                int UVWidthCount = 0;
                for (int j = 0; j < leaf.FaceCountWidth; j++)// create the surrounding vertices 
                {
                   
                    if (leaf.BendingWidth.Count > j)
                    {
                        directionWidthCurrent = Directions.DirectionsFromAngles(directionWidthCurrent, 0, leaf.BendingWidth[j]);
                        directionWidthNext = Directions.DirectionsFromAngles(directionWidthNext, 0, leaf.BendingWidth[j]);

                        uvBackOrForward = 1;
                        UVWidthCount = j;
                    }
                    else
                    {
                        if (leaf.BendingWidth.Count == j)
                        {
                            tempPositionCurrent = pos;
                            tempPositionNext = pos + leaf.Direction[i].Heading * LengthSingle;
                            directionWidthCurrent = leaf.Direction[i];
                            directionWidthNext = leaf.Direction[i + 1];
                            directionWidthCurrent = Directions.DirectionsFromAngles(directionWidthCurrent, 0, MathHelper.Pi);
                            directionWidthNext = Directions.DirectionsFromAngles(directionWidthNext, 0, MathHelper.Pi);

                            UVWidthCount = 0;
                            uvBackOrForward = -1;
                        }
                        else { uvBackOrForward = -1; UVWidthCount++; }
                        directionWidthCurrent = Directions.DirectionsFromAngles(directionWidthCurrent, 0, -leaf.BendingWidth[j - leaf.BendingWidth.Count]);
                        directionWidthNext = Directions.DirectionsFromAngles(directionWidthNext, 0, -leaf.BendingWidth[j - leaf.BendingWidth.Count]);
                    }

                    //    triangleOne[0] = tempPositionCurrent;
                    //    triangleOne[1] = tempPositionNext + directionWidthNext.Right * WidthSingle;
                    //    triangleOne[2] = tempPositionNext;

                    //    triangleTwo[0] = tempPositionCurrent;
                    //    triangleTwo[1] = tempPositionCurrent + directionWidthCurrent.Right * WidthSingle;
                    //    triangleTwo[2] = tempPositionNext + directionWidthNext.Right * WidthSingle;
                    cross = Vector3.Cross(directionWidthCurrent.Heading ,directionWidthCurrent.Right);
                    if (!(uvBackOrForward==-1))
                    {


                        vertices.Add(new TangentVertex(tempPositionCurrent, new Vector2(UHalf + uvBackOrForward*UVWidthCount*uvUnitWidth, UVEnd.Y - (i*uvUnitLenth)), cross, directionWidthCurrent.Right));
                        vertices.Add(new TangentVertex(tempPositionNext + directionWidthNext.Right*WidthSingle, new Vector2(UHalf + uvBackOrForward*(UVWidthCount + 1)*uvUnitWidth, UVEnd.Y - ((i + 1)*uvUnitLenth)), cross, directionWidthCurrent.Right));
                        vertices.Add(new TangentVertex(tempPositionNext, new Vector2(UHalf + uvBackOrForward*UVWidthCount*uvUnitWidth, UVEnd.Y - ((i + 1)*uvUnitLenth)), cross, directionWidthCurrent.Right));

                        vertices.Add(new TangentVertex(tempPositionCurrent, new Vector2(UHalf + uvBackOrForward*UVWidthCount*uvUnitWidth, UVEnd.Y - (i*uvUnitLenth)), cross, directionWidthCurrent.Right));
                        vertices.Add(new TangentVertex(tempPositionCurrent + directionWidthCurrent.Right*WidthSingle, new Vector2(UHalf + uvBackOrForward*(UVWidthCount + 1)*uvUnitWidth, UVEnd.Y - (i*uvUnitLenth)), cross, directionWidthCurrent.Right));
                        vertices.Add(new TangentVertex(tempPositionNext + directionWidthNext.Right*WidthSingle, new Vector2(UHalf + uvBackOrForward*(UVWidthCount + 1)*uvUnitWidth, UVEnd.Y - ((i + 1)*uvUnitLenth)), cross, directionWidthCurrent.Right));
                    }
                    else
                    {
                        vertices.Add(new TangentVertex(tempPositionCurrent, new Vector2(UHalf + uvBackOrForward * UVWidthCount * uvUnitWidth, UVEnd.Y - (i * uvUnitLenth)), cross * -1, directionWidthCurrent.Right));
                        vertices.Add(new TangentVertex(tempPositionNext, new Vector2(UHalf + uvBackOrForward * UVWidthCount * uvUnitWidth, UVEnd.Y - ((i + 1) * uvUnitLenth)), cross * -1, directionWidthCurrent.Right));
                        vertices.Add(new TangentVertex(tempPositionNext + directionWidthNext.Right * WidthSingle, new Vector2(UHalf + uvBackOrForward * (UVWidthCount + 1) * uvUnitWidth, UVEnd.Y - ((i + 1) * uvUnitLenth)), cross * -1, directionWidthCurrent.Right));

                        vertices.Add(new TangentVertex(tempPositionCurrent, new Vector2(UHalf + uvBackOrForward * UVWidthCount * uvUnitWidth, UVEnd.Y - (i * uvUnitLenth)), cross * -1, directionWidthCurrent.Right));
                        vertices.Add(new TangentVertex(tempPositionNext + directionWidthNext.Right * WidthSingle, new Vector2(UHalf + uvBackOrForward * (UVWidthCount + 1) * uvUnitWidth, UVEnd.Y - ((i + 1) * uvUnitLenth)), cross * -1, directionWidthCurrent.Right));
                        vertices.Add(new TangentVertex(tempPositionCurrent + directionWidthCurrent.Right * WidthSingle, new Vector2(UHalf + uvBackOrForward * (UVWidthCount + 1) * uvUnitWidth, UVEnd.Y - (i * uvUnitLenth)), cross * -1, directionWidthCurrent.Right));
                    }

                    tempPositionCurrent += directionWidthCurrent.Right * WidthSingle;
                    tempPositionNext += directionWidthNext.Right * WidthSingle;


                }
            }




            return vertices;
        }

        public bool SegmentIsPartOfRightLOD(int segmentLevel)
        {
            if (segmentLevel == 0)
            {
                return true;
            }
            if (requestedLODLevel == 0)
            {
                return true;
            }
            if (segmentLevel <= (totalAmountofLevels - requestedLODLevel))
            {
                return true;
            }

            return false;
        }

        public static void TestEngineRenderdataGenerater()
        {
            int lodIndex = 0;
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            EngineTreeType tree = new EngineTreeType();
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(20);
            EngineTreeRenderData treeRenderData = new EngineTreeRenderData(game);
            TreeStructure treeStruct = new TreeStructure();

            TreeRenderManager bodyManager = new TreeRenderManager();
            TreeRenderManager leafManager = new TreeRenderManager();
            treeRenderData = gen.GetRenderData(treeStruct, game, 0);

            game.InitializeEvent +=
               delegate
               {

                   treeRenderData.Initialize();

               };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad0))
                    {
                        lodIndex = 0;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad1))
                    {
                        lodIndex = 1;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                    {
                        lodIndex = 2;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad3))
                    {
                        lodIndex = 3;
                    }


                };

            game.DrawEvent +=
                delegate
                {


                    treeRenderData.draw();

                };
            game.Run();
        }

        public static void TestEngineRenderdataGeneraterVolumetricLeaves()
        {
            int lodIndex = 0;
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            EngineTreeType tree = new EngineTreeType();
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(20);
            EngineTreeRenderData treeRenderData = new EngineTreeRenderData(game);
            TreeStructure treeStruct = new TreeStructure();

            TreeRenderManager bodyManager = new TreeRenderManager();
            TreeRenderManager leafManager = new TreeRenderManager();
            treeRenderData = gen.GetRenderData(treeStruct, game, 0);

            game.InitializeEvent +=
               delegate
               {

                   treeRenderData.Initialize();

               };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad0))
                    {
                        lodIndex = 0;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad1))
                    {
                        lodIndex = 1;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                    {
                        lodIndex = 2;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad3))
                    {
                        lodIndex = 3;
                    }


                };

            game.DrawEvent +=
                delegate
                {


                    treeRenderData.draw();

                };
            game.Run();
        }


    }

    class StructureRenderDatas
    {
        public EngineTreeRenderData[] RenderDataLods;


    }
}
