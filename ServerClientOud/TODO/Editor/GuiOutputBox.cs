using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class GuiOutputBox : GuiPanel
    {
        //private Texture2D texture;
        //private Rectangle sourceRectangle;

        private SpriteFont font;
        private List<string> lines = new List<string>();

        private Color textColor;

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        private float lineSpacing = 20.0f;

        public float LineSpacing
        {
            get { return lineSpacing; }
            set { lineSpacing = value; }
        }
        //private string text;

        //public delegate void ClickedEventHandler( object sender, EventArgs e );
        //public ClickedEventHandler Clicked;

        public GuiOutputBox( Rectangle nBounding )
            : base( nBounding )
        {
            //text = "(geen)";
            textColor = Color.Black;
        }

        public GuiOutputBox( int x, int y, int width, int height )
            : base( x, y, width, height )
        {
            //text = "(geen)";
            textColor = Color.Black;
        }

        public void AddLine( string text )
        {
            lines.Add( text );
        }


        public override void Draw( SpriteBatch batch )
        {
            base.Draw( batch );

            float totalHeight = lines.Count * lineSpacing;

            float yStart = Top + Height / 2 - totalHeight / 2;


            for ( int i = 0; i < lines.Count; i++ )
            {
                WriteCenteredText( batch, lines[ i ], yStart + i * lineSpacing, totalHeight );
            }


        }


        private void WriteCenteredText( SpriteBatch batch, string text, float y, float totalHeight )
        {
            Vector2 size = font.MeasureString( text );
            float x = Left + Width / 2 - size.X / 2;

            batch.DrawString( font, text, new Vector2( x, y ), textColor );
        }

        /*
        private Vector2 CalculateCenteredTextPos( string text, float totalHeight )
        {
            

            Vector2 pos = new Vector2( Width, totalHeight ) * 0.5f - size * 0.5f;

            pos += new Vector2( Left, Top );

            return pos;
        }
        */

        public SpriteFont Font
        { get { return font; } set { font = value; } }

    }
}
