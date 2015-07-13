using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TreeGenerator.AtlasTool
{
    class TextureUnit
    {
        public Texture2D Texture;
        public Vector2 Position;
        private float scale;

        public float Scale
        {
            get { return scale; }
            set { scale = value; Size = Resolution * scale; }
        }
        public Vector4 UVCoords;
        public Vector2 Resolution;
        public Vector2 Size;
        Texture2D lineTex;

        public TextureUnit(Texture2D tex, Vector2 pos, float scale,GraphicsDevice dev)
        {
            Texture=tex;
            Position=pos;
            Scale=scale;
            Resolution = new Vector2(tex.Width, tex.Height);
            Size = new Vector2(tex.Width, tex.Height)*scale;

            lineTex = new Texture2D(dev, 2, 2, 1, TextureUsage.None, SurfaceFormat.Color);
            Color[] pixels = new Color[4];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.White;
            }
            pixels[0] = Color.White;
            lineTex.SetData<Color>(pixels);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            DrawLine(Position,new Vector2(Position.X,Position.Y+Size.Y),Color.Black,batch);
            DrawLine(Position, new Vector2(Position.X+Size.X, Position.Y), Color.Black, batch);
            DrawLine(Position+Size, new Vector2(Position.X + Size.X, Position.Y), Color.Black, batch);
            DrawLine(Position+Size, new Vector2(Position.X, Position.Y + Size.Y), Color.Black, batch);


            

        }
        private void DrawLine(Vector2 pos1, Vector2 pos2, Color color, SpriteBatch spriteBatch)
        {
           
           
            float length=Vector2.Distance(pos1,pos2);
            Vector2 pos;
            if (pos1.X<=pos2.X)
	            {
		        pos=pos1;
	            }else{pos=pos2;}
               
            float AngleBetweenTwoVectors =(float)Math.Atan((pos2.Y-pos1.Y)/(pos2.X-pos1.X));
         
            
            spriteBatch.Draw(lineTex, new Rectangle((int)pos.X,(int) pos.Y, (int)length, 1),new Rectangle(0, 0, 1, 1),color,AngleBetweenTwoVectors,new Vector2(0, 0),SpriteEffects.None, 0);
            
            
            
        }
        
        public void DrawFinal(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
        public void Draw(SpriteBatch batch,Color color)
        {
            batch.Draw(Texture, Position, null, color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}
