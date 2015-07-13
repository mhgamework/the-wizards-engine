using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.help;

namespace TreeGenerator
{
    public class TreeLeafType
    {
        public Range width = new Range(1f, 1f);
        public Range Length = new Range(1f, 1f);
        public Range DistanceFromTrunk = new Range(4f, 4f);

        public RangeSpreading RelativePosition = new RangeSpreading(0, 1, 0);
        public RangeSpreading DropAngle = new RangeSpreading(0, 1, 1);
        public RangeSpreading AxialSplitPosition = new RangeSpreading(0, 1, 1);///This axialSplit discribes where on the segment I should be rotated
        public RangeSpreading AxialSplitOrientation = new RangeSpreading(MathHelper.PiOver4, MathHelper.PiOver2, 1);///This axialSplit discribes how the leaf is turned around it own Direction
        

        public Range LeafCount = new Range(1, 2);

        public ITexture Texture;
        public ITexture BumpTexture;

        public bool BillBoardLeaf = true;

        public bool Flower = false;
        public bool VolumetricLeaves = false;
        public Range BendingWidth = new Range(-0.2f, -0.2f);
        public Range BendingLength = new Range(0.2f, 0.2f);
        public int FaceCountWidth = 4;
        public int FaceCountLength = 1;
        bool connectingBranch = false;/// for later new implementation yet()
                                      /// 
        

        public static void TestLeaf()
        {

            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            TreeLeafType leafType = new TreeLeafType();
            TreeStructureGenerater structGen = new TreeStructureGenerater();
            TreeStructureLeaf structLeaf= new TreeStructureLeaf();
            List<Vector3[]> Triangles =new List<Vector3[]>();
            List<Vector3[]> UVlines = new List<Vector3[]>();
            List<Color> ColorsTriangles = new List<Color>();
            Seeder seed = new Seeder(468);
            Vector3 positie = Vector3.Zero;
            game.InitializeEvent +=
               delegate
               {
                  
                   

               };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad0))
                    {
                        Triangles = new List<Vector3[]>();
                        UVlines = new List<Vector3[]>();
                        structGen.leafSeeder = new Seeder(856);
                        structLeaf = structGen.CreateLeave(leafType, new TreeGenerator.help.Directions(new Vector3(0, 0, 1), new Vector3(1, 0, 0)), 0, 0,0, 0, 0,0);



                        if ((structLeaf.FaceCountWidth % 2) != 0)
                        {
                            structLeaf.FaceCountWidth--;
                        }

                        Vector2 UVStart = new Vector2(0, 0);
                        Vector2 UVEnd = new Vector2(1, 1);


                       Vector3 pos = positie;// rightcorner this has to be corrected in the final implemantation
                        Directions dir = structLeaf.Direction[0];
                        Vector3 cross;
                        cross = Vector3.Cross(dir.Right, dir.Heading);
                        cross.Normalize();
                        float WidthSingle = structLeaf.Width / structLeaf.FaceCountWidth;
                        float LengthSingle = structLeaf.Length / structLeaf.Direction.Count;

                        float uvUnitLenth = (UVEnd.X - UVStart.X) / (structLeaf.Direction.Count-1);
                        float uvUnitWidth = (UVEnd.Y - UVStart.Y) / structLeaf.FaceCountWidth;

                        float WidthFaceHalf = structLeaf.FaceCountWidth / 2;

                        Directions directionWidthCurrent, directionWidthNext;
                        for (int i = 0; i < structLeaf.Direction.Count-1; i++)// step one up in the length direction
                        {
                            if (i != 0)
                            {
                                pos += structLeaf.Direction[i-1].Heading  * LengthSingle;
                            }


                            directionWidthCurrent = structLeaf.Direction[i];
                            directionWidthNext = structLeaf.Direction[i + 1];
                            Vector3 tempPositionCurrent =pos;
                            Vector3 tempPositionNext = pos + structLeaf.Direction[i].Heading * LengthSingle;

                            float UHalf=(UVEnd.X-UVStart.X)*0.5f;
                            int uvBackOrForward=0;
                            int UVWidthCount = 0;
                            for (int j = 0; j < structLeaf.FaceCountWidth; j++)// create the surrounding vertices 
                            {
                                
                                    if (structLeaf.BendingWidth.Count > j)
                                    {
                                        
                                        directionWidthCurrent = Directions.DirectionsFromAngles(directionWidthCurrent, 0, structLeaf.BendingWidth[j]);
                                        directionWidthNext = Directions.DirectionsFromAngles(directionWidthNext, 0, structLeaf.BendingWidth[j]);

                                        uvBackOrForward=1;
                                        UVWidthCount = j;
                                    }
                                    else
                                    {
                                        if (structLeaf.BendingWidth.Count == j)
                                        {
                                            tempPositionCurrent = pos;
                                            tempPositionNext = pos + structLeaf.Direction[i].Heading * LengthSingle;
                                            directionWidthCurrent = structLeaf.Direction[i];
                                            directionWidthNext = structLeaf.Direction[i + 1];
                                            directionWidthCurrent = Directions.DirectionsFromAngles(directionWidthCurrent, 0, MathHelper.Pi);
                                            directionWidthNext = Directions.DirectionsFromAngles(directionWidthNext, 0, MathHelper.Pi);
                                            UVWidthCount = 0;
                                            uvBackOrForward = 1;
                                        }
                                        else { uvBackOrForward = -1; UVWidthCount++; }
                                        directionWidthCurrent = Directions.DirectionsFromAngles(directionWidthCurrent, 0, -structLeaf.BendingWidth[j - structLeaf.BendingWidth.Count]);
                                        directionWidthNext = Directions.DirectionsFromAngles(directionWidthNext, 0, -structLeaf.BendingWidth[j - structLeaf.BendingWidth.Count]);
                                       
                                        
                                       
                                    }
                                
                                
                                    Vector3[] triangleOne = new Vector3[3], triangleTwo = new Vector3[3];
                                    Vector3[] ULine=new Vector3[2], Vline=new Vector3[2];

                                    triangleOne[0] = tempPositionCurrent;
                                    //uvtesting
                                    ULine[0] = triangleOne[0]; Vline[0] = triangleOne[0];
                                    ULine[1] = ULine[0] + Vector3.Up * (UHalf + uvBackOrForward * UVWidthCount * uvUnitWidth);
                                    Vline[1] =Vline[0]+Vector3.Up*(UVEnd.Y - (i * uvUnitLenth));
                                    UVlines.Add(ULine); UVlines.Add(Vline);

                                    triangleOne[1] = tempPositionNext + directionWidthNext.Right * WidthSingle;
                                    ULine[0] = triangleOne[1]; Vline[0] = triangleOne[1];
                                    ULine[1] = ULine[0] + Vector3.Up * (UHalf + uvBackOrForward * (UVWidthCount + uvBackOrForward) * uvUnitWidth);
                                    Vline[1] = Vline[0] + Vector3.Up * (UVEnd.Y - ((i+1) * uvUnitLenth));
                                    UVlines.Add(ULine); UVlines.Add(Vline);

                                    triangleOne[2] = tempPositionNext;
                                    ULine[0] = triangleOne[2]; Vline[0] = triangleOne[2];
                                    ULine[1] = ULine[0] + Vector3.Up * (UHalf + uvBackOrForward * UVWidthCount * uvUnitWidth);
                                    Vline[1] = Vline[0] + Vector3.Up * (UVEnd.Y - ((i+1) * uvUnitLenth));
                                    UVlines.Add(ULine); UVlines.Add(Vline);

                                    triangleTwo[0] = tempPositionCurrent;
                                    triangleTwo[1] = tempPositionCurrent + directionWidthCurrent.Right * WidthSingle;
                                    triangleTwo[2] = tempPositionNext + directionWidthNext.Right * WidthSingle;

                                    Triangles.Add(triangleOne); Triangles.Add(triangleTwo);
                    
                                    //ColorsTriangles.Add(seed.NextColor());
                                    //ColorsTriangles.Add(seed.NextColor());
                                    if (structLeaf.BendingWidth.Count > j)
                                    {
                                        ColorsTriangles.Add(Color.Black);
                                        ColorsTriangles.Add(Color.Black);

                                    }
                                    else
                                    {
                                        ColorsTriangles.Add(Color.Red);
                                        ColorsTriangles.Add(Color.Red);
                                    }

                                    tempPositionCurrent += directionWidthCurrent.Right * WidthSingle;
                                    tempPositionNext += directionWidthNext.Right * WidthSingle;
                               
                                

                               

                            }


                        }



                    }


#region OldverticeMethode
                        //float lengthBendAngle =0;
                        //float WidthBendAngle = 0;

                        //float tempLength = 0;
                        //for (int i = 0; i < structLeaf.BendingWidth.Count; i++)
                        //{
                        //    WidthBendAngle += structLeaf.BendingWidth[i];
                        //    tempLength += (float)Math.Sin(structLeaf.BendingWidth[i]) * (WidthSingle/2);
                        //}
                        //Vector3 tempPosition = pos;
                        //Vector3 lengthTransformationOne = Vector3.Zero;
                        //for (int k = 0; k < structLeaf.FaceCountLength; k++)// Counting to make each length structure
                        //{
                        //    Directions dirLength=new Directions();
                        //    Directions dirWidthPrevious = new Directions();
                        //    Directions dirLengthNext = new Directions();
                        //    Directions dirWidthNext = new Directions();
                        //    lengthBendAngle += structLeaf.BendingLenght[k];

                        //    dirLength = Directions.DirectionsFromAngles(dir, lengthBendAngle, 0);
                        //    if (k + 1 < structLeaf.BendingLenght.Count - 1)
                        //    {
                        //        dirLengthNext = Directions.DirectionsFromAngles(dir, lengthBendAngle+structLeaf.BendingLenght[k+1], 0);
                        //    }
                        //    else { dirLengthNext = Directions.DirectionsFromAngles(dir,lengthBendAngle+structLeaf.BendingLenght[k], 0); }
                        //    lengthTransformationOne = dirLength.Heading * LengthSingle;
                           



                        //    float tempWidthBendAngle = WidthBendAngle + structLeaf.BendingWidth[0];
                        //    float tempWidthBendAngleNext = WidthBendAngle;

                        //    tempPosition = pos + cross * tempLength;
                        //    Vector3 PositionOne, PositionTwo;
                        //    PositionOne = tempPosition;
                        //    PositionTwo = tempPosition;
                        //    if (k != 0)
                        //    {
                        //        PositionTwo = PositionOne + lengthTransformationOne;
                        //    }
                        //    for (int j = 0; j < structLeaf.FaceCountWidth; j++)// Counting to make each with structure in each length structure
                        //    {
                        //        Vector2 UVStartLocal = new Vector2(UVStart.X + (structLeaf.FaceCountLength - (k + 1)) * uvUnitLenth, UVStart.Y + j * uvUnitHeigth);
                        //        Vector2 UVEndLocal = new Vector2(UVStart.X + (structLeaf.FaceCountLength - (k)) * uvUnitLenth, UVStart.Y + (j + 1) * uvUnitHeigth);

                        //        Vector3[] triangleOne=new Vector3[3],triangleTwo = new Vector3[3];
                        //        if ((structLeaf.FaceCountWidth %2)== 0)
                        //        {
                        //            if (structLeaf.BendingWidth.Count > j&&j!=0)
                        //            {
                        //                tempWidthBendAngle -= structLeaf.BendingWidth[j-1];
                        //                tempWidthBendAngleNext-=structLeaf.BendingWidth[j];
                                       
                        //            }
                        //            else
                        //            {
                        //                if (structLeaf.BendingWidth.Count==j)
                        //                {
                        //                    tempWidthBendAngle -= structLeaf.BendingWidth[j - structLeaf.BendingWidth.Count];
                        //                    tempWidthBendAngleNext -= structLeaf.BendingWidth[j - structLeaf.BendingWidth.Count];
                        //                }
                        //                if (j != 0)
                        //                {
                        //                    tempWidthBendAngle -= structLeaf.BendingWidth[j - structLeaf.BendingWidth.Count];
                        //                    tempWidthBendAngleNext -= structLeaf.BendingWidth[j - structLeaf.BendingWidth.Count];
                        //                    tempPosition = pos;
                        //                }
                        //            }

                        //            dirWidthPrevious = Directions.DirectionsFromAngles(dir, 0, tempWidthBendAngle);
                        //            dirWidthNext = Directions.DirectionsFromAngles(dir, 0, tempWidthBendAngleNext);

                        //            PositionOne = PositionTwo;
                        //            PositionTwo = PositionOne + dirWidthNext.Right * (WidthSingle / 2);
                        //            if (k != 0)
                        //            {
                        //                PositionOne += lengthTransformationOne;
                        //                PositionTwo+=lengthTransformationOne;
                        //            }
                        //        }
                        //        else
                        //        {

                        //        }



                        //        //triangleOne[0] = tempPosition + j * dirWidthPrevious.Right * (WidthSingle / 2) + dirLength.Heading * k * LengthSingle;
                        //        //triangleOne[1] = tempPosition + (j + 1) * dirWidthNext.Right * (WidthSingle / 2) + dirLengthNext.Heading * (k + 1) * LengthSingle;
                        //        //triangleOne[2] = tempPosition + j * dirWidthPrevious.Right * (WidthSingle / 2) + dirLengthNext.Heading * (k + 1) * LengthSingle;

                        //        //triangleTwo[0] = tempPosition + j * dirWidthPrevious.Right * (WidthSingle / 2) + dirLength.Heading * k * LengthSingle;
                        //        //triangleTwo[1] = tempPosition + (j + 1) * dirWidthNext.Right * (WidthSingle / 2) + dirLength.Heading * k * LengthSingle;
                        //        //triangleTwo[2] = tempPosition + (j + 1) * dirWidthNext.Right * (WidthSingle / 2) + dirLengthNext.Heading * (k + 1) * LengthSingle;

                        //        triangleOne[0] = PositionOne + dirLength.Heading * LengthSingle;
                        //        triangleOne[1] = PositionTwo;// +dirLengthNext.Heading * LengthSingle;
                        //        triangleOne[2] = PositionOne;// +dirLengthNext.Heading * LengthSingle;

                        //        triangleTwo[0] = PositionOne + dirLength.Heading  * LengthSingle;
                        //        triangleTwo[1] = PositionTwo + dirLength.Heading  * LengthSingle;
                        //        triangleTwo[2] = PositionTwo;// +dirLengthNext.Heading * LengthSingle;
                        //        Triangles.Add(triangleOne); Triangles.Add(triangleTwo);

                        //        if (k != 0)
                        //        {
                        //            PositionOne -= lengthTransformationOne;
                        //            PositionTwo -= lengthTransformationOne;
                        //        }
                        //    }

#endregion
                        
                    
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad8))
                    {
                        leafType.FaceCountLength++;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad6))
                    {
                        leafType.FaceCountWidth++;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad4))
                    {
                        leafType.FaceCountWidth--;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                    {
                        leafType.FaceCountLength--;
                    }


                };

            game.DrawEvent +=
                delegate
                {
                    for (int i = 0; i < Triangles.Count; i++)
			            {
                Color color;
                //if ((i % 2) == 0)
                //{
                //    color = Color.Black;
                //}
                //else { color = Color.Red; }
               

                game.LineManager3D.AddTriangle(Triangles[i][0], Triangles[i][1], Triangles[i][2], ColorsTriangles[i]);
             

			            }
                       Vector3 position =positie;
                float LengthSingle = structLeaf.Length / structLeaf.Direction.Count;
                for (int j = 0; j < structLeaf.Direction.Count; j++)
                {
                    game.LineManager3D.AddLine(position, position + structLeaf.Direction[j].Heading, Color.Yellow);
                    game.LineManager3D.AddLine(position, position + structLeaf.Direction[j].Right, Color.Green);

                    position+=structLeaf.Direction[j].Heading*LengthSingle;
                }

                    for (int i = 0; i < UVlines.Count; i++)
                    {
                        Color color;
                        if ((i%2)==0)
	                        {
                    		    color=Color.BlueViolet; 
	                        }
                        else
                        {
                            color=Color.Brown;
                        }
                        game.LineManager3D.AddLine(UVlines[i][0],UVlines[i][1],color);
                    }
                };
            game.Run();
        }

        
    
     }
    }


