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
        private List<Color> colors = new List<Color>();
        public Texture2D CreateTexture(int width, int length, float[,] HeightValues,float HeightDifference,GraphicsDevice device)
        {
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    colors.Add(new Color(new Vector4(HeightValues[i, j] / HeightDifference, HeightValues[i, j] / HeightDifference, HeightValues[i, j] / HeightDifference, 1.0f)));
                }
            }

            texture = new Texture2D(device, width, length, 1, TextureUsage.None, SurfaceFormat.Color);
            texture.SetData(colors.ToArray());
            return texture;
        }

    }
}
