//using System;
//using System.Collections.Generic;
//using System.Text;
//using TreeGenerator.help;
//using Microsoft.Xna.Framework;
//using MHGameWork.TheWizards.ServerClient;
//using Microsoft.Xna.Framework.Graphics;
//using MHGameWork.TheWizards.Graphics;


//namespace TreeGenerator.Editor
//{
//    public class EditorTreeRenderDataGenerater
//    {
//        float vLevel=0;
//        float segLenght = 0;
//        int numVertices;
//        public TreeStructure TreeStructure = new TreeStructure();
//        public EditorTreeRenderData TreeRenderData= new EditorTreeRenderData();

        
//        private EditorTreeRenderDataPart renderDataTemp;

//        public EditorTreeRenderDataGenerater(int _numVertices)
//        {

//            TreeRenderData = new EditorTreeRenderData();
//            numVertices = _numVertices;
           
//        }

//        public void CreateAllTtriangles(TreeStructure treeStructure, Vector3 pos, float previousSegmentLenght)
//        {
//            TreeRenderData.TreeRenderDataParts.Add(new EditorTreeRenderDataPart(new TreeRenderManager(),pos));
//            TreeRenderData.TreeRenderDataParts[0].NumVertices = numVertices;
//            TreeStructure = treeStructure;
//            for (int i = 0; i < treeStructure.Textures.Count; i++)
//            {
//               TreeRenderData.Texture.Add(treeStructure.Textures[i]);
//               TreeRenderData.BumpTexture.Add(treeStructure.Bumps[i]);
//               if (i!=0)
//               {
//                   TreeRenderData.TreeRenderDataParts.Add(new EditorTreeRenderDataPart(new TreeRenderManager(),pos));
                   
//               }
//            }

//            if (treeStructure.Base.Children.Count != 0)
//            {
//                CreateTrianglesForSegment(treeStructure.Base, treeStructure.Base.Children[0], pos, previousSegmentLenght);
//                segLenght = treeStructure.Base.Length;
//            }
//            pos += treeStructure.Base.Direction.Heading * treeStructure.Base.Length;
//            for (int i = 0; i < treeStructure.Base.Children.Count; i++)
//            {
//                segLenght = 0;
//                CreateAllTtriangles(treeStructure.Base.Children[i], pos, segLenght);

//            }
//            for (int i = 0; i < TreeRenderData.TreeRenderDataParts.Count; i++)
//            {
//                if (TreeRenderData.TreeRenderDataParts[i].Vertices.Count<1)
//                {
//                    TreeRenderData.TreeRenderDataParts.RemoveAt(i);
//                    i = 0;
//                }
//            }
//        }

//        private  void CreateAllTtriangles(TreeStructureSegment seg, Vector3 pos,float previousSegmentLenght)
//        {
//            if (seg.Children.Count != 0)
//            {
//                CreateTrianglesForSegment(seg, seg.Children[0], pos, previousSegmentLenght);
//                segLenght = seg.Length;
//            }
//            pos += seg.Direction.Heading * seg.Length;
//            for (int i = 0; i < seg.Children.Count; i++)
//            {
//                segLenght = 0;
//                CreateAllTtriangles(seg.Children[i], pos, segLenght);
                
//            }

//        }

//        public void CreateTrianglesForSegment(TreeStructureSegment currentSegment, TreeStructureSegment nextSegment, Vector3 position,float previousSegmentLenght)
//        {
//            TangentVertex[] currentVertices = createVerticesForSegment(currentSegment,TreeStructure.UVMaps[currentSegment.LevelIndex], position, TreeRenderData.TreeRenderDataParts[0].NumVertices,vLevel,previousSegmentLenght);
//            TangentVertex[] nextVertices = createVerticesForSegment(nextSegment, TreeStructure.UVMaps[nextSegment.LevelIndex], position + currentSegment.Direction.Heading * currentSegment.Length, TreeRenderData.TreeRenderDataParts[0].NumVertices,vLevel+1,currentSegment.Length);

//            vLevel += 1;


//            for (int i = 0; i < TreeRenderData.TreeRenderDataParts[0].NumVertices ; i++)
//            {

//                TreeRenderData.TreeRenderDataParts[0].Vertices.Add(currentVertices[i]);
//                TreeRenderData.TreeRenderDataParts[0].Vertices.Add(nextVertices[i]);
//                TreeRenderData.TreeRenderDataParts[0].Vertices.Add(nextVertices[i + 1]);

//                TreeRenderData.TreeRenderDataParts[0].Vertices.Add(currentVertices[i]);
//                TreeRenderData.TreeRenderDataParts[0].Vertices.Add(nextVertices[i + 1]);
//                TreeRenderData.TreeRenderDataParts[0].Vertices.Add(currentVertices[i + 1]);


//            }

//            //TreeRenderData.Vertices.Add(currentVertices[TreeRenderData.NumVertices - 1]);
//            //TreeRenderData.Vertices.Add(nextVertices[TreeRenderData.NumVertices - 1]);
//            //TreeRenderData.Vertices.Add(nextVertices[0]);

//            //TreeRenderData.Vertices.Add(currentVertices[TreeRenderData.NumVertices - 1]);
//            //TreeRenderData.Vertices.Add(nextVertices[0]);
//            //TreeRenderData.Vertices.Add(currentVertices[0]);

//            if (currentSegment.Leaves != null)
//            {
//                createLeafVerticesForSegment(currentSegment, position);
//            }
            

//        }

//        private TangentVertex[] createVerticesForSegment(TreeStructureSegment segment, Vector2 uvMap, Vector3 position, int numVertices,float vlevel,float previousSegmentLenght)
//        {
            
//            TangentVertex[] vertices = new TangentVertex[numVertices+1];
//            float r = segment.Radius / 2;
//            double anglePerVer = (2 * MathHelper.Pi) / (numVertices - 1);
//            double angleVert = 0;

//            float uIncrease = 1.0f /numVertices;
//            //float uSurfaceIncrease = 1.0f / (2 * MathHelper.Pi * (segment.Radius / 2)); I tried to make the top and bottom bark look the same. this doesn't work because it curves the texture
//            //float vIncrease =segment
//            for (int i = 0; i < vertices.Length-1; i++)
//            {
//                float U;
//                U = i * uIncrease * uvMap.X;//*uSurfaceIncrease ;
//                float V;
//                V = vlevel* uvMap.Y*previousSegmentLenght;// *vIncrease;
//                vertices[i] = new TangentVertex(new Vector3((float)(r * Math.Sin(angleVert)), 0.0f, (float)(r * Math.Cos(angleVert))), new Vector2(U, V), new Vector3((float)(Math.Sin(angleVert)), 0.0f, (float)(Math.Cos(angleVert))), new Vector3((float)(Math.Cos(angleVert)), 0.0f, (float)(-Math.Sin(angleVert))));
//                //vertices[i].pos *= segment.Radius;
//                angleVert += anglePerVer;

//            }
//            vertices[vertices.Length - 1] = vertices[0];
//            vertices[vertices.Length - 1].uv.X = 1;

//            Vector3 cross;
//            float angle;

//            cross = Vector3.Cross(segment.Direction.Heading, Vector3.Up);
//            angle = (float)Math.Acos((double)Vector3.Dot(segment.Direction.Heading, Vector3.Up));


//            //game.LineManager3D.AddLine(Vector3.Zero, Vector3.Up, Color.Orange);
//            //game.LineManager3D.AddLine(Vector3.Zero, cross, Color.Yellow);

//            Matrix mRot;

//            mRot = Matrix.CreateFromAxisAngle(cross, -angle);


//            //mRot = mRot * Matrix.CreateFromAxisAngle(directions.Right, DropAngle);


//            Matrix m = mRot * Matrix.CreateTranslation(position);
//            for (int i = 0; i < vertices.Length; i++)
//            {
//                TangentVertex v = vertices[i];

//                v.pos = Vector3.Transform(v.pos, m);
//                v.tangent = Vector3.Transform(v.tangent, mRot);
//                v.normal = Vector3.Transform(v.normal, mRot);
//                vertices[i] = v;

//            }
//            return vertices;

//        }

//        public void createLeafVerticesForSegment(TreeStructureSegment segment, Vector3 position)
//        {
//            for (int i = 0; i < segment.Leaves.Count; i++)
//            {
//                TreeStructureLeaf leaf=segment.Leaves[i];
//                Directions dirAxial = Directions.DirectionsFromAngles(segment.Direction, 0, leaf.AxialSplit);
//                if(!leaf.volumetricLeave)
//                {
                      
                   
//                Vector3 pos = position + segment.Direction.Heading * leaf.Position + dirAxial.Right * (segment.Radius + leaf.DistanceFromTrunk);
//                Directions dir = leaf.Direction[0]; 
//                Vector3 cross;
//                cross = Vector3.Cross(dir.Right, dir.Heading);
//                cross.Normalize();
//                //rechthoek1
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos - dir.Right * (leaf.Width / 2), new Vector2(1, 1), cross, dir.Right));
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos + dir.Right * (leaf.Width / 2) + dir.Heading * leaf.Length, new Vector2(0, 0), cross, dir.Right));
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos - dir.Right * (leaf.Width / 2) + dir.Heading * leaf.Length, new Vector2(1, 0), cross, dir.Right));

//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos - dir.Right * (leaf.Width / 2), new Vector2(1, 1), cross, dir.Right));
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos + dir.Right * (leaf.Width / 2) + dir.Heading * leaf.Length, new Vector2(0, 0), cross, dir.Right));
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos + dir.Right * (leaf.Width / 2), new Vector2(0, 1), cross, dir.Right));




//                //rechthoek2
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos - cross * (leaf.Width / 2), new Vector2(1, 1), dir.Right, cross));
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos + cross * (leaf.Width / 2) + dir.Heading * leaf.Length, new Vector2(0, 0), dir.Right, cross));
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos - cross * (leaf.Width / 2) + dir.Heading * leaf.Length, new Vector2(1, 0), dir.Right, cross));

//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos + cross * (leaf.Width / 2), new Vector2(0, 1), dir.Right, cross));
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos - cross * (leaf.Width / 2), new Vector2(1, 1), dir.Right, cross));
//                TreeRenderData.TreeRenderDataParts[1].Vertices.Add(new TangentVertex(pos + cross * (leaf.Width / 2) + dir.Heading * leaf.Length, new Vector2(0, 0), dir.Right, cross));

//                }
//                if (leaf.volumetricLeave && !leaf.Flower)
//                {
//                    Vector3 pos = position + segment.Direction.Heading * leaf.Position;// + dirAxial.Right * (segment.Radius + leaf.DistanceFromTrunk);
//                    List<TangentVertex> volumetricLeaveVertices = CreateVerticesForVolumetricLeaf(leaf, pos);

//                    for (int j = 0; j < volumetricLeaveVertices.Count; j++)
//                    {
//                        TreeRenderData.TreeRenderDataParts[1].Vertices.Add(volumetricLeaveVertices[j]);
//                    }

//                }
//                else
//                {

//                }


                 
			 
//            }
           

//        }
//        public List<TangentVertex> CreateVerticesForVolumetricLeaf(TreeStructureLeaf leaf, Vector3 position)
//        {
//            List<TangentVertex> vertices = new List<TangentVertex>();

//            Vector2 UVStart = new Vector2(0, 0);
//            Vector2 UVEnd = new Vector2(1f, 1f);


//            Vector3 pos = position;
//            Directions dir = leaf.Direction[0];
//            Vector3 cross;
//            cross = Vector3.Cross(dir.Right, dir.Heading);
//            cross.Normalize();
//            float WidthSingle = leaf.Width / leaf.FaceCountWidth;
//            float LengthSingle = leaf.Length / leaf.Direction.Count;
//            float uvUnitLenth = (UVEnd.Y - UVStart.Y) / (leaf.Direction.Count-1);
//            float uvUnitWidth = (UVEnd.X - UVStart.X) / leaf.FaceCountWidth;

//            float WidthFaceHalf = leaf.FaceCountWidth / 2;

//            Directions directionWidthCurrent, directionWidthNext;
//            for (int i = 0; i < leaf.Direction.Count - 1; i++)// step one up in the length direction
//            {
//                if (i != 0)
//                {
//                    pos += leaf.Direction[i - 1].Heading * LengthSingle;
//                }


//                directionWidthCurrent = leaf.Direction[i];
//                directionWidthNext = leaf.Direction[i + 1];
//                Vector3 tempPositionCurrent = pos;
//                Vector3 tempPositionNext = pos + leaf.Direction[i].Heading * LengthSingle;


//                float UHalf = (UVEnd.X - UVStart.X) * 0.5f;
//                int uvBackOrForward = 0;
//                int UVWidthCount = 0;
//                for (int j = 0; j < leaf.FaceCountWidth; j++)// create the surrounding vertices 
//                {
//                    if ((leaf.FaceCountWidth % 2) != 0)
//                    {
//                        leaf.FaceCountWidth--;
//                    }

//                    if (leaf.BendingWidth.Count > j)
//                    {
//                        directionWidthCurrent = Directions.DirectionsFromAngles(directionWidthCurrent, 0, leaf.BendingWidth[j]);
//                        directionWidthNext = Directions.DirectionsFromAngles(directionWidthNext, 0, leaf.BendingWidth[j]);

//                        uvBackOrForward = 1;
//                        UVWidthCount = j;
//                    }
//                    else
//                    {
//                        if (leaf.BendingWidth.Count == j)
//                        {
//                            tempPositionCurrent = pos;
//                            tempPositionNext = pos + leaf.Direction[i].Heading * LengthSingle;
//                            directionWidthCurrent = leaf.Direction[i];
//                            directionWidthNext = leaf.Direction[i + 1];
//                            directionWidthCurrent = Directions.DirectionsFromAngles(directionWidthCurrent, 0, MathHelper.Pi);
//                            directionWidthNext = Directions.DirectionsFromAngles(directionWidthNext, 0, MathHelper.Pi);

//                            UVWidthCount = 0;
//                            uvBackOrForward = -1;
//                        }
//                        else { uvBackOrForward = -1; UVWidthCount++; }
//                        directionWidthCurrent = Directions.DirectionsFromAngles(directionWidthCurrent, 0, -leaf.BendingWidth[j - leaf.BendingWidth.Count]);
//                        directionWidthNext = Directions.DirectionsFromAngles(directionWidthNext, 0, -leaf.BendingWidth[j - leaf.BendingWidth.Count]);
//                    }

//                    cross = Vector3.Cross(directionWidthCurrent.Heading, directionWidthCurrent.Right);
//                    if ((j) == 2)
//                    { 
//                        int gh = 0; 
//                    }
//                    vertices.Add(new TangentVertex(tempPositionCurrent, new Vector2(UHalf + uvBackOrForward * UVWidthCount * uvUnitWidth, UVEnd.Y - (i * uvUnitLenth)), cross, directionWidthCurrent.Right));
//                    vertices.Add(new TangentVertex(tempPositionNext + directionWidthNext.Right * WidthSingle, new Vector2(UHalf + uvBackOrForward * (UVWidthCount + 1) * uvUnitWidth, UVEnd.Y - ((i + 1) * uvUnitLenth)), cross, directionWidthCurrent.Right));
//                    vertices.Add(new TangentVertex(tempPositionNext, new Vector2(UHalf + uvBackOrForward * UVWidthCount * uvUnitWidth, UVEnd.Y - ((i + 1) * uvUnitLenth)), cross, directionWidthCurrent.Right));

//                    vertices.Add(new TangentVertex(tempPositionCurrent, new Vector2(UHalf + uvBackOrForward * UVWidthCount * uvUnitWidth, UVEnd.Y - (i * uvUnitLenth)), cross, directionWidthCurrent.Right));
//                    vertices.Add(new TangentVertex(tempPositionCurrent + directionWidthCurrent.Right * WidthSingle, new Vector2(UHalf + uvBackOrForward * (UVWidthCount + 1) * uvUnitWidth, UVEnd.Y - (i * uvUnitLenth)), cross, directionWidthCurrent.Right));
//                    vertices.Add(new TangentVertex(tempPositionNext + directionWidthNext.Right * WidthSingle, new Vector2(UHalf + uvBackOrForward * (UVWidthCount + 1) * uvUnitWidth, UVEnd.Y - ((i + 1) * uvUnitLenth)), cross, directionWidthCurrent.Right));


//                    tempPositionCurrent += directionWidthCurrent.Right * WidthSingle;
//                    tempPositionNext += directionWidthNext.Right * WidthSingle;


//                }
//            }




//            return vertices;
//        }
//        //public void CreateMesh()
//        //{

//        //    EditorTreeRenderData.decl = TangentVertex.CreateVertexDeclaration(game);
//        //    vertexStride = TangentVertex.SizeInBytes;
//        //    EditorTreeRenderData.vertexCount = vertices.Count;
//        //    EditorTreeRenderData.triangleCount = vertexCount / 3;

//        //    EditorTreeRenderData.vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount, BufferUsage.None);
//        //    vertexBuffer.SetData(vertices.ToArray());
//        //}



//        /*public static void TestCreateVertices()
//        {
//            XNAGame game;
//            game = new XNAGame();
//            EditorTreeRenderDataGenerater gen = new EditorTreeRenderDataGenerater(20,1,1);
//            TreeStructureSegment seg = new TreeStructureSegment();
//            seg.Direction = new Directions(Vector3.Normalize(new Vector3(0.2f, 0.6f, 0.8f)), Vector3.UnitZ);
//            seg.Radius = 5;


//            TangentVertex[] vertices = null;



//            game.DrawEvent +=
//                delegate
//                {
//                    vertices = gen.createVerticesForSegment(seg,1,1, Vector3.Zero, 20);
//                    game.LineManager3D.AddLine(seg.Direction.Heading * 10, Vector3.Zero, Color.Purple);
//                    for (int i = 0; i < vertices.Length; i++)
//                    {
//                        game.LineManager3D.AddCenteredBox(vertices[i].pos, 0.2f, Color.Red);
//                        if (i + 1 < vertices.Length)
//                        {

//                            game.LineManager3D.AddLine(vertices[i].pos, vertices[i + 1].pos, Color.Black);
//                        }
//                        else
//                        {
//                            game.LineManager3D.AddLine(vertices[0].pos, vertices[i].pos, Color.Black);
//                        }

//                    }
//                };

//            game.Run();
//        }*/

//        public static void TestCreateAllVertices()
//        {
//            XNAGame game;
//            game = new XNAGame();
//            TreeType treeType = TreeType.LoadFromXML("tree");
//            TreeStructure treeStructure = new TreeStructure();
//            TreeStructureGenerater structGen = new TreeStructureGenerater();
//            treeStructure = structGen.GenerateTree(treeType, 12385,0);
//            EditorTreeRenderDataGenerater renderGen = new EditorTreeRenderDataGenerater(20);
//            renderGen.CreateAllTtriangles(treeStructure, Vector3.Zero,0);



//            game.DrawEvent +=
//                delegate
//                {


//                    for (int j = 0; j < renderGen.TreeRenderData.TreeRenderDataParts.Count; j++)
//                    {

//                        for (int i = 0; i < renderGen.TreeRenderData.TreeRenderDataParts[j].Vertices.Count; i += 3)
//                        {
//                            game.LineManager3D.AddTriangle(
//                                renderGen.TreeRenderData.TreeRenderDataParts[j].Vertices[i + 0].pos,
//                                renderGen.TreeRenderData.TreeRenderDataParts[j].Vertices[i + 1].pos,
//                                renderGen.TreeRenderData.TreeRenderDataParts[j].Vertices[i + 2].pos,
//                                Color.Black);
//                            //game.LineManager3D.AddCenteredBox(renderGen.TreeRenderData.Vertices[i].pos, 0.2f, Color.Red);
//                            /*if (i + 1 < renderGen.TreeRenderData.Vertices.Count)
//                            {

//                                game.LineManager3D.AddLine(renderGen.TreeRenderData.Vertices[i].pos, renderGen.TreeRenderData.Vertices[i + 1].pos, Color.Black);
//                            }
//                            else
//                            {
//                                game.LineManager3D.AddLine(renderGen.TreeRenderData.Vertices[0].pos, renderGen.TreeRenderData.Vertices[i].pos, Color.Black);
//                            }*/

//                        }
//                    }
//                };

//            game.Run();
//        }
//        public static void TestCreateVolumetricLeaveVertices()
//        {
//            XNAGame game;
//            game = new XNAGame();
//             TreeType TreeType;
//             TreeStructure TreeStructure;
//             TreeStructureGenerater treeStructureGenerater;
//             EditorTreeRenderDataGenerater RenderGen = new EditorTreeRenderDataGenerater(20);
//             EditorTreeRenderData RenderData;

//             TreeType = new TreeType();
//             treeStructureGenerater = new TreeStructureGenerater();

//            TreeStructure = treeStructureGenerater.GenerateTree(TreeType, 456, 0);

//            RenderGen = new EditorTreeRenderDataGenerater(20);
//            RenderGen.CreateAllTtriangles(TreeStructure, Vector3.Zero, 0);

//            RenderData = new EditorTreeRenderData();
//            RenderData = RenderGen.TreeRenderData;
//            //for (int i = 0; i < RenderData.TreeRenderDataParts.Count; i++)
//            //{
//            //    if (RenderData.BumpTexture[i] == "null")
//            //    {
//            //        RenderData.TreeRenderDataParts[i].Initialize(XNAGameControl, RenderData.Texture[i]);
//            //    }
//            //    else
//            //    {
//            //        RenderData.TreeRenderDataParts[i].InitializeBump(XNAGameControl, RenderData.Texture[i], RenderData.BumpTexture[i]);
//            //    }
//            //    RenderData.TreeRenderDataParts[i].SetWorldMatrix(Matrix.Identity);
//            //}



//            game.DrawEvent +=
//                delegate
//                {



                        
                    
//                };

//            game.Run();
//        }

//        public static void TestCreateForest()
//        { XNAGame game;
//            game = new XNAGame();
//            TreeType treeType = TreeType.LoadFromXML("tree");
//            //List<TreeStructure> treeStructures = new List<TreeStructure>();
            
//            TreeStructureGenerater structGen = new TreeStructureGenerater();
//            EditorTreeRenderDataGenerater renderGen = new EditorTreeRenderDataGenerater(20);

//            List<EditorTreeRenderData> RenderData = new List<EditorTreeRenderData>();
//            Seeder seeder = new Seeder(789);
//            int treeNum = 100;

//            game.InitializeEvent +=
//                delegate
//                {
//                    TreeStructure treestruct;
//                    EditorTreeRenderData renderData;
//                    renderGen = new EditorTreeRenderDataGenerater(20);

//                    for (int i = 0; i < treeNum; i++)
//                    {
//                        renderGen = new EditorTreeRenderDataGenerater(20);
//                        treestruct = new TreeStructure();
//                        renderData = new EditorTreeRenderData();
//                        treestruct=structGen.GenerateTree(treeType,i,0);
//                        //treeStructures.Add(treestruct)
//                        renderGen.CreateAllTtriangles(treestruct,seeder.NextVector3(new Vector3(75,0,75),new Vector3(0,0,0)),0);
//                        renderData = renderGen.TreeRenderData;
//                        renderData.Initialize(game);
//                        RenderData.Add(renderData);
//                    }
//                    renderGen = null;
//                };

           

//            game.DrawEvent +=
//                delegate
//                {
//                    for (int j = 0; j < RenderData.Count; j++)
//                    {


//                        RenderData[j].render();
//                    }
//                    for (int i = 0; i < RenderData.Count; i++)
//                    {
//                        for (int j = 0; j < RenderData[i].TreeRenderDataParts.Count; j++)
//                        {
//                            RenderData[i].TreeRenderDataParts[j].Render();
//                        }
//                    }


                        
                   
//                };

//            game.Run();
//        }

//        static void game_InitializeEvent(object sender, EventArgs e)
//        {
//            throw new Exception("The method or operation is not implemented.");
//        }

//    }
//}
