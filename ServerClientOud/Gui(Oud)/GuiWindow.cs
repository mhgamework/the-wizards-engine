using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient.Gui
{
    public class GuiWindow : GuiControl
    {
        [Flags]
        public enum Border
        {
            None = 0,
            Top = 1,
            Right = 2,
            Bottom = 4,
            Left = 8
        }
        public enum WindowBorderStyle
        {
            Normal = 1,
            None = 2,
        }

        public Texture2D tex;
        public TileSet tileSet;

        GuiWindowBorder dragWindowBorder;
        int[] dragBorderSizes;

        private WindowBorderStyle borderStyle;

        public WindowBorderStyle BorderStyle
        {
            get { return borderStyle; }
            set { borderStyle = value; }
        }


        private int GetDragBorderSize( Border border )
        {
            switch ( border )
            {
                case Border.Top:
                    return dragBorderSizes[ 0 ];
                case Border.Right:
                    return dragBorderSizes[ 1 ];
                case Border.Bottom:
                    return dragBorderSizes[ 2 ];
                case Border.Left:
                    return dragBorderSizes[ 3 ];
            }
            throw new InvalidOperationException( "Invalid border!" );
        }

        public GuiWindow()
        {
            dragBorderSizes = new int[] { 20, 20, 20, 20 };
            borderStyle = WindowBorderStyle.Normal;
        }


        public override void OnMouseDown( MouseEventArgs e )
        {
            base.OnMouseDown( e );

            if ( dragWindowBorder == null && borderStyle == WindowBorderStyle.Normal )
            {
                Rectangle bounding = GetBounding();

                int leftDist = e.CursorPosition.X - bounding.Left;
                int topDist = e.CursorPosition.Y - bounding.Top;
                int rightDist = bounding.Right - e.CursorPosition.X;
                int bottomDist = bounding.Bottom - e.CursorPosition.Y;


                //Check for borders to drag;

                Border border = Border.None;
                if ( leftDist > 0 && leftDist < GetDragBorderSize( Border.Left ) ) border |= Border.Left;
                if ( topDist > 0 && topDist < GetDragBorderSize( Border.Top ) ) border |= Border.Top;
                if ( rightDist > 0 && rightDist < GetDragBorderSize( Border.Right ) ) border |= Border.Right;
                if ( bottomDist > 0 && bottomDist < GetDragBorderSize( Border.Bottom ) ) border |= Border.Bottom;


                if ( border != Border.None )
                {
                    Vector2 pos = new Vector2( ( bounding.Left + bounding.Right ) / 2, ( bounding.Top + bounding.Bottom ) / 2 );
                    if ( ( border & Border.Left ) > 0 ) pos.X = bounding.Left;
                    if ( ( border & Border.Right ) > 0 ) pos.X = bounding.Right;
                    if ( ( border & Border.Top ) > 0 ) pos.Y = bounding.Top;
                    if ( ( border & Border.Bottom ) > 0 ) pos.Y = bounding.Bottom;

                    GuiWindowBorder windowBorder = new GuiWindowBorder( this, border );
                    dragWindowBorder = windowBorder;
                    dragWindowBorder.Position = pos;
                    dragWindowBorder.Size = new Vector2( 1, 1 );
                    Controls.Add( windowBorder );

                    windowBorder.Moved += new EventHandler<EventArgs>( border_Moved );
                    windowBorder.DragEnded += new EventHandler<EventArgs>( border_DragEnded );

                    dragWindowBorder.StartDrag();
                }
                else
                {
                    StartDrag();
                }
            }
            else
            {
                //Dragging the border, impossible?

            }
        }

        void border_DragEnded( object sender, EventArgs e )
        {
            border_Moved( sender, e );


            Controls.Remove( dragWindowBorder );
            dragWindowBorder.Dispose();

            dragWindowBorder = null;


        }

        void border_Moved( object sender, EventArgs e )
        {
            Rectangle bounding = GetBounding();
            int left = bounding.Left;
            int right = bounding.Right;
            int top = bounding.Top;
            int bottom = bounding.Bottom;

            if ( ( dragWindowBorder.Border & Border.Left ) > 0 ) left = (int)dragWindowBorder.Position.X;
            if ( ( dragWindowBorder.Border & Border.Right ) > 0 ) right = (int)dragWindowBorder.Position.X;
            if ( ( dragWindowBorder.Border & Border.Top ) > 0 ) top = (int)dragWindowBorder.Position.Y;
            if ( ( dragWindowBorder.Border & Border.Bottom ) > 0 ) bottom = (int)dragWindowBorder.Position.Y;

            float width = right - left;
            if ( width < 100 )
            {
                if ( ( dragWindowBorder.Border & Border.Left ) > 0 ) left = right - 100;
                else right = left + 100;
            }

            float height = bottom - top;
            if ( height < 100 )
            {
                if ( ( dragWindowBorder.Border & Border.Top ) > 0 ) top = bottom - 100;
                else bottom = top + 100;
            }

            Size = new Vector2( right - left, bottom - top );
            Position = new Vector2( left, top );
        }

        protected override void OnDrawBackground( GuiGraphics graphics )
        {
            base.OnDrawBackground( graphics );

            //e.TempCreateGraphics();

            //Graphics.SpriteBatch.Draw( tex, new Rectangle( 0, 0, (int)Size.X, (int)Size.Y ), new Rectangle( 237, 156, 770 - 237, 560 - 156 ), Color.White );

            //e.Graphics.Draw( tex, new Rectangle( 0, 0, tex.Width, tex.Height ), new Rectangle( (int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y ), GuiAlign.None );


            //e.Graphics.DrawTiled( tex, new Rectangle( 100, 100, 200, 200 ),
            //    new Rectangle( (int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y ),
            //    new Point( 50, 50 ),
            //    GuiAlign.None );


            int width = (int)Size.X;
            int height = (int)Size.Y;
            if ( borderStyle == WindowBorderStyle.Normal )
            {

                Rectangle sourceTopLeft = tileSet.GetTileRectangle( 0, 0 );
                Rectangle sourceTopCenter = tileSet.GetTileRectangle( 0, 1 );
                Rectangle sourceTopRight = tileSet.GetTileRectangle( 0, 2 );

                Rectangle sourceMiddleLeft = tileSet.GetTileRectangle( 1, 0 );
                Rectangle sourceMiddleCenter = tileSet.GetTileRectangle( 1, 1 );
                Rectangle sourceMiddleRight = tileSet.GetTileRectangle( 1, 2 );

                Rectangle sourceBottomLeft = tileSet.GetTileRectangle( 2, 0 );
                Rectangle sourceBottomCenter = tileSet.GetTileRectangle( 2, 1 );
                Rectangle sourceBottomRight = tileSet.GetTileRectangle( 2, 2 );

                TileSet target = new TileSet( 0, 0,
                    new int[] { tileSet.ColumnWidths[ 0 ], width - tileSet.ColumnWidths[ 0 ] - tileSet.ColumnWidths[ 2 ], tileSet.ColumnWidths[ 2 ] },
                    new int[] { tileSet.RowHeights[ 0 ], height - tileSet.RowHeights[ 0 ] - tileSet.RowHeights[ 2 ], tileSet.RowHeights[ 2 ] }
                    );


                for ( int x = 0; x < 3; x++ )
                {

                    for ( int y = 0; y < 3; y++ )
                        graphics.Draw( tex, tileSet.GetTileRectangle( x, y ), target.GetTileRectangle( x, y ), GuiAlign.None );

                }
            }
            else if ( borderStyle == WindowBorderStyle.None )
            {
                graphics.Draw( tex, tileSet.GetTileRectangle( 1, 1 ), new Rectangle( 0, 0, (int)Size.X, (int)Size.Y ), GuiAlign.None );
            }
        }

        public static void TestRenderWindow001()
        {
            TestXNAGame game = null;

            GuiWindow control = null;

            DrawEventArgs e = new DrawEventArgs();

            TestXNAGame.Start( "GuiWindow.TestRenderWindow001",
                delegate
                {
                    game = TestXNAGame.Instance;
                    game.Mouse.CursorEnabled = true;
                    //main.XNAGame.Graphics.ToggleFullScreen();

                    control = new GuiWindow();

                    control.tex = Texture2D.FromFile( game.GraphicsDevice,
                        game.EngineFiles.GuiDefaultSkin.GetFullFilename() );

                    control.tileSet = new TileSet( 228, 150,
                        new int[] { 52, 415, 80 },
                        new int[] { 76, 291, 49 } );

                    control.Position = new Vector2( 50, 50 );
                    control.Size = new Vector2( 700, 500 );
                    control.BackgroundColor = Color.Red;
                    control.Load( game.GraphicsDevice );

                    control.Service = game.GuiService;
                    game.GuiService.TopLevelControls.Add( control );


                },
                delegate
                {

                    //e.Device = main.XNAGame.GraphicsDevice;
                    //e.SpriteBatch = main.SpriteBatch;
                    //e.TempCreateGraphics();
                    //control.Graphics = e.Graphics;


                    //e.SpriteBatch.Begin();
                    //control.Invalidate();
                    //control.Draw( e );
                    //e.SpriteBatch.End();


                } );
        }
    }
}
