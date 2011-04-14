using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient.Gui
{
    public class GuiImage : GuiControl
    {
        public Texture2D boe;

        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                Invalidate();
            }
        }

        private Rectangle? sourceRectangle;

        public Rectangle? SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }



        public GuiImage()
        {
            
        }

        protected override void OnDrawBackground( GuiGraphics graphics )
        {
            base.OnDrawBackground( graphics );
            if ( texture == null ) return;

            if ( !sourceRectangle.HasValue )
                sourceRectangle = new Rectangle( 0, 0, texture.Width, texture.Height );

            graphics.Draw( texture, sourceRectangle.Value, new Rectangle( 0, 0, (int)Size.X, (int)Size.Y ), GuiAlign.None );
        }


        public static void TestRenderImage()
        {
            TestXNAGame game = new TestXNAGame( "Gui.GuiImage.TestRenderImage" );
            game.initCode = delegate
            {
                Gui.GuiImage image = new GuiImage();
                image.Texture = Texture2D.FromFile( game.GraphicsDevice, 
                    game.EngineFiles.Wallpaper001.GetFullFilename() );
                image.Position = new Vector2( 100, 100 );
                image.Size = new Vector2( 640, 480 );
                image.Load( game.GraphicsDevice );
                game.GuiService.TopLevelControls.Add( image );
                image.Service = game.GuiService;

            };

            game.Run();
        }
    }
}
