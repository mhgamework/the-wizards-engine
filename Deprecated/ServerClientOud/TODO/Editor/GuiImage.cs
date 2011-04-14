using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class GuiImage : GuiPanel
    {
        private Texture2D texture;

        public GuiImage()
            : base()
        {

        }

        public GuiImage( Rectangle nBounding )
            : base( nBounding )
        {

        }

        public GuiImage( int x, int y, int width, int height )
            : base( x, y, width, height )
        {

        }


        public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch batch )
        {
            batch.Draw( texture, Bounding, Color.White );
            base.Draw( batch );
        }


        public Texture2D Texture
        { get { return texture; } set { texture = value; } }

    }
}
