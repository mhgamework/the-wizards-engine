using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MHGameWork.TheWizards.ServerClient.Gui
{
    public class GuiListBox<T> : GuiListBoxBase
    {
        private struct ListItem
        {
            public string Text;
            public T Item;
            public ListItem( string nText, T nItem )
            {
                Text = nText;
                Item = nItem;
            }
        }

        private Microsoft.Xna.Framework.Graphics.SpriteFont font;

        public Microsoft.Xna.Framework.Graphics.SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        private Microsoft.Xna.Framework.Graphics.Color textColor;

        public Microsoft.Xna.Framework.Graphics.Color TextColor
        {

            get { return textColor; }
            set { textColor = value; }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set {
                if ( selectedIndex == value ) return;

                Invalidate();
                selectedIndex = value;
            }
        }

        public T SelectedItem
        {
            get
            {
                if ( selectedIndex == -1 ) return default( T );
                return items[ selectedIndex ].Item;
            }
        }

        public string SelectedItemText
        {
            get
            {
                if ( selectedIndex == -1 ) return null;
                return items[ selectedIndex ].Text;
            }
        }


        private GuiAlign textAlign;

        public GuiAlign TextAlign
        {
            get { return textAlign; }
            set { textAlign = value; }
        }



        private List<ListItem> items = new List<ListItem>();

        private GuiListBox()
        {
            this.MouseUp += new EventHandler<MouseEventArgs>( GuiListBox_MouseUp );
        }

        void GuiListBox_MouseUp( object sender, MouseEventArgs e )
        {
            int borderWidth = 10;

            int itemHeight = 40;

            Vector2 clickPos = new Vector2( e.CursorPosition.X, e.CursorPosition.Y );
            clickPos -= Position + new Vector2( borderWidth, borderWidth );

            if ( clickPos.X < 0 || clickPos.X > Size.X - borderWidth - borderWidth )
                return;

            int index = (int)Math.Floor( clickPos.Y / itemHeight );

            SelectedIndex = index;



        }

        public void AddItem( string text, T item )
        {
            items.Add( new ListItem( text, item ) );
        }

        protected override void OnDrawBackground( GuiGraphics graphics )
        {
            base.OnDrawBackground( graphics );

            int borderWidth = 10;

            int startIndex = 0;
            int endIndex = items.Count - 1;

            int localI = 0;

            int itemHeight = 40;

            //Draw selection rectangle
            if ( selectedIndex != -1 )
            {
                Viewport oldViewport = graphics.Device.Viewport;

                Viewport newViewport = oldViewport;

                newViewport.X += borderWidth;
                newViewport.Y += borderWidth + selectedIndex * itemHeight;
                newViewport.Height = itemHeight;
                newViewport.Width = (int)Size.X - borderWidth - borderWidth;

                graphics.Device.Viewport = newViewport;

                graphics.Device.Clear( Color.LightBlue );


                graphics.Device.Viewport = oldViewport;

            }


            for ( int i = startIndex; i <= endIndex; i++ )
            {
                graphics.DrawText( font, items[ i ].Text, textColor, new Microsoft.Xna.Framework.Rectangle(
                    borderWidth, borderWidth + itemHeight * localI, (int)Size.X - borderWidth - borderWidth, itemHeight ), textAlign );

                localI++;

            }
        }



        public static void TestRenderListBox()
        {
            TestXNAGame game = new TestXNAGame( "Gui.GuiListBox.TestRenderListBox" );
            game.initCode =
                delegate
                {
                    GuiListBox<int> listBox = new GuiListBox<int>();

                    game.Mouse.CursorEnabled = true;

                    listBox.AddItem( "Getal 1", 11 );
                    listBox.AddItem( "Getal 2", 22 );
                    listBox.AddItem( "Getal 3", 33 );
                    listBox.AddItem( "Getal 4", 44 );

                    listBox.BackgroundColor = Microsoft.Xna.Framework.Graphics.Color.PowderBlue;

                    listBox.Position = new Microsoft.Xna.Framework.Vector2( 200, 100 );
                    listBox.Size = new Microsoft.Xna.Framework.Vector2( 100, 300 );
                    listBox.font = game.Content.Load<Microsoft.Xna.Framework.Graphics.SpriteFont>( game.EngineFiles.DefaultFontAsset );
                    listBox.TextColor = Microsoft.Xna.Framework.Graphics.Color.Red;
                    listBox.TextAlign = GuiAlign.MiddleLeft;

                    listBox.Service = game.GuiService;
                    game.GuiService.TopLevelControls.Add( listBox );
                    listBox.Load( game.GraphicsDevice );

                };

            game.Run();
        }

    }
}
