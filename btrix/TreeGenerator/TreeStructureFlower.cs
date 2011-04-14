using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Graphics;
using TreeGenerator.help;

namespace TreeGenerator
{
    public class TreeStructureFlower
    {
        public List<TreeStructureLeaf> FlowerLeafsOpen = new List<TreeStructureLeaf>();
        public List<TreeStructureLeaf> FlowerLeafsClosed = new List<TreeStructureLeaf>();
        //flowerHeart
        public float FlowerHeartDiameter = 1f;/// for now only circularish
        public float FlowerHeartLength = 2f;///distance of the cone that goes from the trunk to the flower
       //pistill and stamen these are represented by an image rendered on a face like the grass
        public int PistillFaceCount = 1;
        public int StamenFaceCount = 1;
        public Vector2 PistillFaceDimentions = new Vector2(1, 1);
        public Vector2 StamenFaceDimentions = new Vector2(1, 1);
        public float PistillDistanceFromCenter = 0;
        public float StamenDistanceFromCenter = 0.1f;





        public static void TestFlower()
        {

            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            TreeStructureGenerater gen = new TreeStructureGenerater();
            
            TreeFlowerType flowerType=new TreeFlowerType() ;
            TreeStructureFlower flowerStruct = new TreeStructureFlower();
            game.InitializeEvent +=
               delegate
               {
                  


               };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad0))//Recreate the flower
                    {
                        //Creating the Leaves in the structure
                        float angle=MathHelper.TwoPi/flowerType.FlowerLeafCount;
                        for (int i = 0; i < flowerType.FlowerLeafCount; i++)
                        {
                            flowerStruct.FlowerLeafsClosed.Add(gen.CreateLeave(flowerType.FlowerleafTypeClosed, new Directions(Vector3.Up, Vector3.Right), gen.leafSeeder.NextFloat(flowerType.FlowerleafTypeClosed.DropAngle, i, flowerType.FlowerLeafCount), i * angle, 0, 0, i,0));
                            flowerStruct.FlowerLeafsOpen.Add(gen.CreateLeave(flowerType.FlowerLeafTypeOpen, new Directions(Vector3.Up, Vector3.Right), gen.leafSeeder.NextFloat(flowerType.FlowerLeafTypeOpen.DropAngle, i, flowerType.FlowerLeafCount), i * angle, 0, 0, i,0));
                        }
                        
                        //fill in the rest of the variables
                    }
                  

                };

            game.DrawEvent +=
                delegate
                {
                    
                };
            game.Run();
        }


    }
}
