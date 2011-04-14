using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class GuiKnop : GuiPanel
    {
        private Texture2D texture;
        private Rectangle sourceRectangle;

        private SpriteFont font;
        private string text;

        //public delegate void ClickedEventHandler( object sender, EventArgs e );
        //public ClickedEventHandler Clicked;

        public GuiKnop( Rectangle nBounding )
            : base( nBounding )
        {
            text = "(geen)";
        }

        public GuiKnop( int x, int y, int width, int height )
            : base( x, y, width, height )
        {
            text = "(geen)";
        }

        public override void Draw( SpriteBatch batch )
        {
            base.Draw( batch );
            Vector2 textSize = font.MeasureString( text );

            Vector2 textPosition = Vector2.Zero;
            textPosition.X = Bounding.Left + Bounding.Width / 2 - textSize.X / 2;
            textPosition.Y = Bounding.Top + Bounding.Height  / 2 - textSize.Y / 2;

            batch.Draw( texture, Bounding, sourceRectangle, Color.White );
            batch.DrawString( font, text, textPosition, Color.Black );
            
        }


        public Texture2D Texture
        { get { return texture; } set { texture = value; } }
        public Rectangle SourceRectangle
        { get { return sourceRectangle; } set { sourceRectangle = value; } }

        public SpriteFont Font
        { get { return font; } set { font = value; } }
        public string Text
        { get { return text; } set { text = value; } }
    }
}
