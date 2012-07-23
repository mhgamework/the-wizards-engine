using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TreeGenerator.TerrrainGeneration
{
    public class HeigthMapGenerater
    {
        private Texture2D texture;
        
        
        public Texture2D CreateTextureFloat(int width, int length, float[,] HeightValues,GraphicsDevice device)
        {
            float[] height = new float[width*length];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    height[i*width + j] = HeightValues[i, j];

                }
            }

            texture = new Texture2D(device, width, length, 1, TextureUsage.None, SurfaceFormat.Single);
            texture.SetData(height);
            return texture;
        }
        public Texture2D CreateTextureVector2(int width, int length, Vector2[,] values, GraphicsDevice device)
        {
            //Vector4[] height = new Vector4[width * length ];
            Vector2[] height = new Vector2[width*length];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    //var el = new Vector3(values[i, j].X, values[i, j].Y, values[i, j].Z);
                    
                    if(values[i,j].Length()>0.0001)
                    {
                        int k = 5;
                    }
                    //height[i * width + j] = new Vector4(Vector3.Normalize(el),el.Length());
                    height[i*width + j] = values[i, j];
                }
            }

            texture = new Texture2D(device, width, length, 1, TextureUsage.None, SurfaceFormat.Vector2);
            texture.SetData(height);
            return texture;
        }

    }
}
