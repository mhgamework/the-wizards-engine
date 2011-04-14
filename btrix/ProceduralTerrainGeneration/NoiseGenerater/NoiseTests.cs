using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TreeGenerator.NoiseGenerater
{
   public  class NoiseTests
    {
       public void FirstNoiseTest()
       {
           XNAGame game = new XNAGame();
           PerlinNoiseGenerater noise;  
           noise = new PerlinNoiseGenerater();
           float factor = 0.1f;
           float scale = 15f;
           List<Vector3> positions = new List<Vector3>();
           int width=500;
           int height = 500;
           float verticesPerMeter = 0.5f;
           int verticesX = (int)(width*verticesPerMeter);
           int verticesY = (int)(height*verticesPerMeter);
           
           game.InitializeEvent +=
               delegate
                   { 
                       noise.NumberOfOctaves = 2;
                       noise.persistance = 0.5f;
                      
                       for (int i = 0; i <verticesX ; i++)
                       {
                           for (int j = 0; j < verticesY; j++)
                           {
                               positions.Add(new Vector3(i / verticesPerMeter, noise.GetPerlineNoise(i * factor, j * factor) * scale, j / verticesPerMeter));
                           }
                       }
                      

                   };
         
           game.DrawEvent +=
               delegate
               {
                   for (int j = 0; j < verticesY-1; j++)
                       {

                       for (int i = 0; i < verticesX-1; i++)
                       {


                           game.LineManager3D.AddLine(positions[i+j*verticesY], positions[i+j*verticesY + 1], Color.Blue);
                           game.LineManager3D.AddLine(positions[i+j*verticesY], positions[i + (j+1)*verticesY], Color.Blue);
                           game.LineManager3D.AddLine(positions[i+j*verticesY + 1], positions[i + (j+1)*verticesY], Color.Blue);
                       }

                   }
               };
           game.Run();

       }
       public void NoiseTestSimpleTerrain()
       {
           XNAGame game = new XNAGame();
           PerlinNoiseGenerater noise;  
           noise = new PerlinNoiseGenerater();
           float factor = 0.1f;
           float scale = 10f;
           List<Vector3> positions = new List<Vector3>();
           List<Color> colors = new List<Color>();
           int width=100;
           int height = 100;
           float verticesPerMeter = 1;
           int verticesX = (int)(width*verticesPerMeter);
           int verticesY = (int)(height*verticesPerMeter);
           SimpleTerrain terrain;
           noise.NumberOfOctaves = 8;
                       noise.persistance = 0.2f;
                      
                       for (int i = 0; i <verticesX ; i++)
                       {
                           for (int j = 0; j < verticesY; j++)
                           {
                               positions.Add(new Vector3(i/verticesPerMeter, noise.interpolatedNoise((i/verticesPerMeter)*0.1f, (j/verticesPerMeter)*0.2f)*scale, j/verticesPerMeter));
                               colors.Add(new Color((byte)(150 * noise.GetPerlineNoise(i, j)), (byte)(100 + 70 * noise.GetPerlineNoise(i + 1, j + 1)), (byte)(140 * noise.GetPerlineNoise(i, j + 2))));
                           }
                       }
                       terrain = new SimpleTerrain(game,positions,colors,verticesX,verticesY);
           game.InitializeEvent +=
               delegate
                   {

                       terrain.CreateRenderData();

                   };
           bool changed = false;
           game.UpdateEvent +=
               delegate
                   {
                       if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.O))
                       {
                           noise.NumberOfOctaves++;
                           changed = true;
                       }
                       if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
                       {
                           noise.persistance *= 0.1f;
                           changed = true;
                       }
                       if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                       {
                           factor *= 0.1f;
                           changed = true;
                       }
                       if (changed)
                       {
                           for (int i = 0; i < verticesX; i++)
                           {
                               for (int j = 0; j < verticesY; j++)
                               {
                                   positions.Add(new Vector3(i / verticesPerMeter, noise.GetPerlineNoise((i / verticesPerMeter) * factor, (j / verticesPerMeter) * factor) * scale, j / verticesPerMeter));
                                   colors.Add(new Color((byte)(150 * noise.GetPerlineNoise(i, j)), (byte)(100 + 70 * noise.GetPerlineNoise(i + 1, j + 1)), (byte)(140 * noise.GetPerlineNoise(i, j + 2))));
                               }
                           }
                           terrain = new SimpleTerrain(game, positions, colors, verticesX, verticesY);
                           terrain.CreateRenderData();
                           changed = false;

                       }
                   };
           game.DrawEvent +=
               delegate
                   {
                       terrain.Render();
                  
                     };
           game.Run();

       }
       public void NoiseTestFractalBrowning()
       {
           XNAGame game = new XNAGame();
           PerlinNoiseGenerater noise;
           noise = new PerlinNoiseGenerater();
           float factor = 0.02f;
           float scale = 40f;
           List<Vector3> positions = new List<Vector3>();
           List<Color> colors = new List<Color>();
           int width = 300;
           int height = 300;
           float verticesPerMeter = 0.5f;
           int verticesX = (int)(width * verticesPerMeter);
           int verticesY = (int)(height * verticesPerMeter);
           SimpleTerrain terrain;
           noise.NumberOfOctaves = 4;
           noise.persistance = 0.5f;

           for (int i = 0; i < verticesX; i++)
           {
               for (int j = 0; j < verticesY; j++)
               {
                   positions.Add(new Vector3(i / verticesPerMeter, noise.GetFractalBrowningNoise(i / verticesPerMeter, j / verticesPerMeter, 4, 2.0f, 0.4f, 0.2f) * scale, j / verticesPerMeter));
                   colors.Add(new Color((byte)(150 * noise.GetPerlineNoise(i, j)), (byte)(100 + 70 * noise.GetPerlineNoise(i + 1, j + 1)), (byte)(140 * noise.GetPerlineNoise(i, j + 2))));
               }
           }
           terrain = new SimpleTerrain(game, positions, colors, verticesX, verticesY);
           game.InitializeEvent +=
               delegate
               {

                   terrain.CreateRenderData();

               };

           game.DrawEvent +=
               delegate
               {
                   terrain.Render();

               };
           game.Run();

       }
       public void NoiseTestRidgeMF()
       {
           XNAGame game = new XNAGame();
           PerlinNoiseGenerater noise;
           noise = new PerlinNoiseGenerater();
           float factor = 0.02f;
           float scale = 40f;
           List<Vector3> positions = new List<Vector3>();
           List<Color> colors = new List<Color>();
           int width = 300;
           int height = 300;
           float verticesPerMeter = 0.5f;
           int verticesX = (int)(width * verticesPerMeter);
           int verticesY = (int)(height * verticesPerMeter);
           SimpleTerrain terrain;
           noise.NumberOfOctaves = 4;
           noise.persistance = 0.5f;

           for (int i = 0; i < verticesX; i++)
           {
               for (int j = 0; j < verticesY; j++)
               {
                   positions.Add(new Vector3(i / verticesPerMeter, noise.RidgedMF((i*0.2f) / verticesPerMeter, (j*0.2f) / verticesPerMeter,0.08f, 8, 4.0f, 0.3f, 1.2f) * scale, j / verticesPerMeter));
                   colors.Add(new Color((byte)(150 * noise.GetPerlineNoise(i, j)), (byte)(100 + 70 * noise.GetPerlineNoise(i + 1, j + 1)), (byte)(140 * noise.GetPerlineNoise(i, j + 2))));
               }
           }
           terrain = new SimpleTerrain(game, positions, colors, verticesX, verticesY);
           game.InitializeEvent +=
               delegate
               {

                   terrain.CreateRenderData();

               };

           game.DrawEvent +=
               delegate
               {
                   terrain.Render();

               };
           game.Run();

       }
       public void Combined()
       {
           XNAGame game = new XNAGame();
           game.SpectaterCamera.FarClip = 10000;
           PerlinNoiseGenerater noise;
           PerlinNoiseGenerater colorNoise;
           noise = new PerlinNoiseGenerater();
           colorNoise = new PerlinNoiseGenerater();
           float frequency = 0.5f;
           ///float factor = 0.02f;
           float scale =100f;
           List<Vector3> positions = new List<Vector3>();
           List<Color> colors = new List<Color>();
           int width = 5000;
           int height = 5000;
           float verticesPerMeter = 0.1f;
           int verticesX = (int)(width * verticesPerMeter);
           int verticesY = (int)(height * verticesPerMeter);
           SimpleTerrain terrain;
           noise.NumberOfOctaves = 4;
           noise.persistance = 0.5f;
           
           for (int i = 0; i < verticesX; i++)
           {
               for (int j = 0; j < verticesY; j++) 
               {
                   //                                           combinedFractalBrowning (float x,float y,int octaves,float lacunarityFractal,float lacunarityRidge,float gainFractal,float gainRidge,float offset,float distribution )
                   float zValue = noise.CombinedFractalBrowningAndRidgedMF(i/verticesPerMeter, j/verticesPerMeter, 8, 4, 4, 0.3f, 0.2f, 1.2f, 0.8f)*scale;
                   positions.Add(new Vector3(i / verticesPerMeter,zValue , j / verticesPerMeter));
                   colors.Add(heightColoring(zValue,j+i*verticesX));
                   //colors.Add(new Color((byte)(150 * colorNoise.interpolatedNoise(i * frequency, j * frequency)), (byte)(100 + 70 * colorNoise.interpolatedNoise((i + 1) * frequency, (j + 1) * frequency)), (byte)(140 * colorNoise.interpolatedNoise(i * frequency, (j + 2) * frequency))));
               }
           }
           terrain = new SimpleTerrain(game, positions, colors, verticesX, verticesY);
           game.InitializeEvent +=
               delegate
               {

                   terrain.CreateRenderData();

               };

           game.DrawEvent +=
               delegate
               {
                   terrain.Render();

               };
           game.Run();

       }

       private Color heightColoring(float heightValue,int count)
       {
           float startLevel = 100f;
           float Zvalue = heightValue - startLevel;
           float BeachLevel =10f;
           float grassLand =30f;
           float spruceLevel = 70f;
           float rockLevel = 100f;
           float snowLevel = 130f;
           if (Zvalue<BeachLevel)
           {
               return smoothColor(Color.Blue, Color.Yellow, Zvalue/BeachLevel, count);
           }
           if (Zvalue<grassLand)
           {
               return smoothColor(Color.Yellow, Color.LawnGreen, Zvalue/grassLand, count);

           }
           if (Zvalue < spruceLevel)
           {
               return smoothColor(Color.LawnGreen, Color.DarkGreen, Zvalue / spruceLevel, count);

           }
           if (Zvalue < rockLevel)
           {
               return smoothColor(Color.DarkGreen, Color.Gray, Zvalue / rockLevel, count);

           }
           if (Zvalue < snowLevel)
           {
               return smoothColor(Color.Gray, Color.White, Zvalue / snowLevel, count);

           }
           if (Zvalue > snowLevel)
           {
               return smoothColor(Color.White, Color.White, Zvalue / snowLevel, count);

           }
           return Color.Pink;
       }
       PerlinNoiseGenerater noise = new PerlinNoiseGenerater();
       private Color smoothColor(Color colorA,Color colorB,float distribution,float count)
       {
           Color color = new Color(Vector4.Lerp(colorA.ToVector4(), colorB.ToVector4(), distribution*noise.interpolatedNoise(count, 1)));

          return color;
           return Color.White;
       }
    }
}
