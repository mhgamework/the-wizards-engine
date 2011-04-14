using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Gui
{
    public class GuiGraphics
    {
        private GraphicsDevice device;

        public GraphicsDevice Device
        {
            get { return device; }
            set { device = value; }
        }
        private SpriteBatch batch;

        public SpriteBatch SpriteBatch
        {
            get { return batch; }
            set { batch = value; }
        }

        public GuiGraphics( GraphicsDevice nDevice, SpriteBatch nBatch )
        {
            device = nDevice;
            batch = nBatch;
        }

        /// <summary>
        /// Align not implemented
        /// </summary>
        public void Draw( Texture2D tex, Rectangle source, Rectangle dest, GuiAlign align )
        {

            batch.Draw( tex, dest, source, Color.White );
        }

        /// <summary>
        /// Scales the source to match the dest, unless the dest is smaller than the source. 
        /// In this case the source is cropped to match dest.
        /// 
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="minSize"></param>
        public void Draw( Texture2D tex, Rectangle source, Rectangle dest, Vector2 scale )
        {
            Vector2 minSize = new Vector2( source.Width * scale.X, source.Height * scale.Y );
            if ( dest.Width < minSize.X ) source.Width = (int)(dest.Width / scale.X);
            if ( dest.Height < minSize.Y ) source.Height = (int)(dest.Height / scale.Y);
            Draw( tex, source, dest, GuiAlign.None );
        }

        /// <summary>
        /// Align not implemented
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="tileSize"></param>
        /// <param name="align"></param>
        public void DrawTiled( Texture2D tex, Rectangle source, Rectangle dest, Microsoft.Xna.Framework.Point tileSize, GuiAlign align )
        {
            //DrawTiledRow( tex, source, dest, tileSize, align );

            int width;
            int height;

            Rectangle iSource = source;
            Rectangle iDest = dest;

            for ( int iy = dest.Top; iy < dest.Bottom; iy += tileSize.Y )
            {
                height = dest.Bottom - iy;

                if ( height > tileSize.Y )
                {
                    height = tileSize.Y;
                    iSource.Height = source.Height;

                }
                else
                {
                    iSource.Height = source.Height / tileSize.Y * height;
                }

                iDest.Y = iy;
                iDest.Height = height;


                for ( int ix = dest.Left; ix < dest.Right; ix += tileSize.X )
                {
                    width = dest.Right - ix;
                    if ( width > tileSize.X )
                    {
                        width = tileSize.X;
                        iSource.Width = source.Width;
                    }
                    else
                    {
                        iSource.Width = source.Width / tileSize.X * width;
                    }

                    iDest.X = ix;
                    iDest.Width = width;

                    Draw( tex, iSource, iDest, align );

                }

                //if ( ix < dest.Right )
                //{
                //    Draw( tex, source, new Rectangle( ix, iy, dest.Right - ix, tileSize.Y ) );
                //}
            }

            //if ( iy < dest.Bottom )
            //{
            //    //Draw( tex, source, new Rectangle( ix, iy, dest.Right - ix, dest.Bottom - ) );
            //}
        }

        private void DrawTiledRow( Texture2D tex, Rectangle source, Rectangle dest, Microsoft.Xna.Framework.Point tileSize, GuiAlign align )
        {
            int ix = dest.Left;
            for ( ; ix + tileSize.X < dest.Right; ix += tileSize.X )
            {
                Draw( tex, source, new Rectangle( ix, dest.Y, tileSize.X, tileSize.Y ), align );
            }

            if ( ix < dest.Right )
            {
                int width = dest.Right - ix;

                Draw( tex, new Rectangle( source.X, source.Y, source.Width / tileSize.X * width, source.Height ), new Rectangle( ix, dest.Y, width, source.Height ), align );
            }
        }

        public void DrawScaled( Texture2D tex, Rectangle source, Rectangle dest )
        {
            DrawTiled( tex, source, dest, new Point( source.Width, source.Height ), GuiAlign.None );
        }

        public void DrawText( SpriteFont font, string text, Color textColor, Rectangle dest, GuiAlign textAlign )
        {
            
            Vector2 scale = Vector2.One;
            Vector2 pos = Vector2.Zero;


            Vector2 textSize = font.MeasureString( text );

            if ( textSize.X > dest.Width ) scale.X = dest.Width / textSize.X;
            if ( textSize.Y > dest.Height ) scale.Y = dest.Height / textSize.Y;


            float uniScale = MathHelper.Min( scale.X, scale.Y );

            textSize.X *= uniScale;
            textSize.Y *= uniScale;

            
            Vector2 halftextSize = textSize * 0.5f;



            Vector2 destCenter = new Vector2( dest.X + dest.Width * 0.5f, dest.Y + dest.Height * .5f );




            if ( ( textAlign & GuiAlign.Left ) > 0 ) pos.X = dest.X;
            if ( ( textAlign & GuiAlign.Center ) > 0 ) pos.X = destCenter.X - halftextSize.X;
            if ( ( textAlign & GuiAlign.Right ) > 0 ) pos.X = dest.Right - textSize.X;

            if ( ( textAlign & GuiAlign.Top ) > 0 ) pos.Y = dest.Y;
            if ( ( textAlign & GuiAlign.Middle ) > 0 ) pos.Y = destCenter.Y - halftextSize.Y;
            if ( ( textAlign & GuiAlign.Bottom ) > 0 ) pos.Y = dest.Bottom - textSize.Y;



            SpriteBatch.DrawString( font, text, pos, textColor, 0, Vector2.Zero, uniScale, SpriteEffects.None, 0 );
        }


    }
}

