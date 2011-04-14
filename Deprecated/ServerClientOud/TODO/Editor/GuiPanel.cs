using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class GuiPanel
    {
        private Rectangle bounding;
        private List<GuiPanel> children;
        private bool enabled;

        private Color backgroundColor;
        private bool opaque;


        public GuiPanel( Rectangle nBounding )
        {
            children = new List<GuiPanel>();
            bounding = nBounding;
            enabled = true;
        }

        public GuiPanel( int x, int y, int width, int height )
            : this( new Rectangle( x, y, width, height ) )
        {
        }

        public GuiPanel()
            : this( new Rectangle( 0, 0, 200, 200 ) )
        {
        }

        public void AddChild( GuiPanel nChild )
        {
            children.Add( nChild );
        }


        public void SetPosition( int nX, int nY )
        {
            Move( nX - Left, nY - Top );
        }

        public void Move( int moveX, int moveY )
        {
            bounding.Offset( moveX, moveY );

            for ( int i = 0; i < children.Count; i++ )
            {
                children[ i ].Move( moveX, moveY );
            }

        }
        public void ChangeSize( int width, int height )
        {
            bounding.Width = width;
            bounding.Height = height;
        }


        public GuiPanel FindPointedObject( Point clickPoint )
        {
            if ( enabled == false ) return null;
            GuiPanel ret;

            if ( !PointerInPanel( clickPoint ) ) return null;

            for ( int i = 0; i < children.Count; i++ )
            {
                ret = children[ i ].FindPointedObject( clickPoint );
                if ( ret != null ) return ret;
            }

            return this;

        }

        public bool PointerInPanel( Point pointer )
        {
            return bounding.Contains( pointer );
        }

        public virtual void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch batch )
        {
            if ( opaque )
            {
                batch.GraphicsDevice.Clear( ClearOptions.Target, backgroundColor, 0, 0, new Rectangle[] { bounding } );
            }

            //Draw from back to front so the first child is on top
            for ( int i = children.Count - 1; i >= 0; i-- )
            {
                if ( children[ i ].Enabled ) children[ i ].Draw( batch );
            }
        }

        public void SetSides( int left, int right, int top, int bottom )
        {
            Move( left - Left, top - Top );
            ChangeSize( right - left, bottom - top );
        }


        public Rectangle Bounding
        { get { return bounding; } }
        public List<GuiPanel> Children
        { get { return children; } }

        public bool Enabled
        { get { return enabled; } set { enabled = value; } }

        public int Bottom
        {
            get { return bounding.Bottom; }
        }
        public int Top
        {
            get { return bounding.Top; }
        }
        public int Left
        {
            get { return bounding.Left; }
        }
        public int Right
        {
            get { return bounding.Right; }
        }

        public int Width
        {
            get { return bounding.Width; }
        }
        public int Height
        {
            get { return bounding.Height; }
        }

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        public bool Opaque
        {
            get { return opaque; }
            set { opaque = value; }
        }
    }
}

