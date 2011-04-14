using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient.Gui
{
    public class GuiButton : GuiControl
    {
        private SpriteFont font;

        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        private TileSet tileSet;

        public TileSet TileSet
        {
            get { return tileSet; }
            set { tileSet = value; }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private Color textColor;

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        private int textBorderSize;

        public int TextBorderSize
        {
            get { return textBorderSize; }
            set { textBorderSize = value; }
        }


        private GuiAlign textAlign;

        public GuiAlign TextAlign
        {
            get { return textAlign; }
            set { textAlign = value; }
        }


        public GuiButton()
        {
            BackgroundColor = Color.TransparentBlack;
            text = "Emtpy";
            textColor = Color.Black;
            textAlign = GuiAlign.MiddleCenter;
            Size = new Vector2( 100, 45 );
            textBorderSize = 10;
        }



        protected override void OnDrawBackground( GuiGraphics graphics )
        {
            base.OnDrawBackground( graphics );

            Rectangle bounding = GetBounding();

            int width = (int)Size.X;
            int height = (int)Size.Y;


            TileSet target = new TileSet( 0, 0,
                new int[] { tileSet.ColumnWidths[ 0 ] >> 1, width - ( tileSet.ColumnWidths[ 0 ] >> 1 ) - ( tileSet.ColumnWidths[ 2 ] >> 1 ), tileSet.ColumnWidths[ 2 ] >> 1 },
                new int[] { tileSet.RowHeights[ 0 ] >> 1, height - ( tileSet.RowHeights[ 0 ] >> 1 ) - ( tileSet.RowHeights[ 2 ] >> 1 ), tileSet.RowHeights[ 2 ] >> 1 }
                );




            for ( int x = 0; x < 3; x++ )
            {
                for ( int y = 0; y < 3; y++ )
                {
                    Rectangle source = tileSet.GetTileRectangle( x, y );
                    Rectangle dest = target.GetTileRectangle( x, y );

                    if ( x == 1 || y == 1 )
                    {

                        Vector2 minSize = new Vector2( source.Width, source.Height ) * 0.5f;

                        graphics.Draw( texture, source, dest, new Vector2( 0.5f ) );
                    }
                    else
                    {
                        graphics.Draw( texture, source, dest, GuiAlign.None );
                    }
                }
            }

            graphics.DrawText( font, text, textColor,
                new Rectangle( textBorderSize, textBorderSize,
                bounding.Width - textBorderSize * 2, bounding.Height - textBorderSize * 2 ), textAlign );

        }

        public static void TestRenderButton()
        {
            List<GuiButton> buttons = new List<GuiButton>();

            TestXNAGame game = null;
            game = new TestXNAGame( "Gui.GuiButton.TestRenderButton",
                delegate
                {
                    game.Mouse.CursorEnabled = true;
                    SpriteFont font = game.Content.Load<SpriteFont>( game.EngineFiles.DefaultFontAsset );

                    GuiButton button;
                    button = new GuiButton();
                    button.text = "TopLeft";
                    button.Position = new Vector2( 30, 30 );
                    button.Size = new Vector2( 120, 100 );
                    button.textAlign = GuiAlign.TopLeft;
                    buttons.Add( button );

                    button = new GuiButton();
                    button.text = "MiddleCenter Scaled";
                    button.Position = new Vector2( 150, 30 );
                    button.Size = new Vector2( 120, 90 );
                    buttons.Add( button );

                    button = new GuiButton();
                    button.text = "Exit";
                    button.Position = new Vector2( 500, 500 );
                    button.Size = new Vector2( 90, 90 );
                    buttons.Add( button );
                    button.MouseUp += delegate { game.Exit(); };

               

                    for ( int i = 0; i < buttons.Count; i++ )
                    {
                        buttons[ i ].Texture = Texture2D.FromFile( game.GraphicsDevice, game.EngineFiles.GuiButton001.GetFullFilename() );
                        buttons[ i ].TileSet = new TileSet( 18, 12, new int[] { 15, 194, 15 }, new int[] { 15, 43, 15 } );
                        buttons[ i ].Font = font;
                        buttons[ i ].Service = game.GuiService;
                        game.GuiService.TopLevelControls.Add( buttons[ i ] );
                        buttons[ i ].Load( game.GraphicsDevice );
                    }
                }, delegate
                {


                } );

            game.Run();
        }
    }
}
