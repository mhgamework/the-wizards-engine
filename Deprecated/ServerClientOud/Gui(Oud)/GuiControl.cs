using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Gui
{
    /// <summary>
    /// Partially based on the design of System.Windows.Forms.Control
    /// </summary>
    public class GuiControl : IDisposable
    {
        private Vector2 size;

        public Vector2 Size
        {
            get { return size; }
            set
            {
                Vector2 oldSize = size;
                size = value;

                Invalidate();


            }
        }

        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set
            {
                Vector2 oldPos = position;
                position = value;
                if ( oldPos.X != value.X || oldPos.Y != value.Y ) OnMoved( null );
            }
        }

        private GuiGraphics graphics;

        public GuiGraphics Graphics
        {
            get
            {
                if ( parent == null )
                    return graphics;
                else
                    return parent.Graphics;
            }
            set
            {
                if ( parent == null )
                    graphics = value;
                else
                    throw new Exception();
            }
        }

        private IGuiService service;

        public IGuiService Service
        {
            get
            {
                if ( parent == null )
                    return service;
                else
                    return parent.Service;
            }
            set
            {
                if ( parent == null )
                    service = value;
                else
                    throw new Exception();
            }
        }

        private bool visible;

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }


        private GuiControlCollection controls;

        public GuiControlCollection Controls
        {
            get { return controls; }
            set { controls = value; }
        }

        private GuiControl parent;

        public GuiControl Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private Vector2 clientSize;

        public Vector2 ClientSize
        {
            get { return clientSize; }
            //set { clientSize = value; }
        }

        public bool IsTopLevelControl
        {
            get { return parent == null; }
        }

        private bool containsFocus;

        /// <summary>
        /// Gets a value indicating whether the control, or one of its child controls, currently has the input focus.
        /// </summary>
        public bool ContainsFocus
        {
            get { return containsFocus; }
            //set { containsFocus = value; }
        }

        private bool focused;

        /// <summary>
        /// Gets a value indicating whether the control has input focus.
        /// </summary>
        public bool Focused
        {
            get { return focused; }
            set { focused = value; }
        }

        private bool dirty;

        /// <summary>
        /// Invalidates a specific region of the control and causes a paint message to be sent to the control.
        /// </summary>
        public void Invalidate()
        {
            dirty = true;
            if ( parent != null ) parent.Invalidate();
        }



        private bool tabStop;
        /// <summary>
        /// Gets or sets a value indicating whether the user can give the focus to this control using the TAB key.
        /// </summary>
        public bool TabStop
        {
            get { return tabStop; }
            set { tabStop = value; }
        }
        private int tabIndex;

        /// <summary>
        /// Gets or sets the tab order of the control within its container.
        /// </summary>
        public int TabIndex
        {
            get { return tabIndex; }
            set { tabIndex = value; }
        }

        private Color backgroundColor;

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        private RenderTarget2D renderTarget;
        private Texture2D renderTexture;
        private Viewport viewPort;

        public GuiControl()
        {
            controls = new GuiControlCollection( this );
            clientSize = Vector2.Zero;
            containsFocus = false;
            visible = true;
            size = new Vector2( 100, 100 );
        }




        public void Load( GraphicsDevice device )
        {
            Vector2 targetSize = size;

            //WARNING: this has to be fixed 
            if ( size.X > device.PresentationParameters.BackBufferWidth )
                size.X = device.PresentationParameters.BackBufferWidth;
            if ( size.Y > device.PresentationParameters.BackBufferHeight )
                size.Y = device.PresentationParameters.BackBufferHeight;


            targetSize = size;



            //renderTarget = new RenderTarget2D( device, (int)size.X, (int)size.Y, 1, SurfaceFormat.Color );

            if ( renderTarget != null ) renderTarget.Dispose();


            renderTarget = new RenderTarget2D( device, (int)targetSize.X, (int)targetSize.Y, 1, SurfaceFormat.Color
                , device.PresentationParameters.MultiSampleType
                , device.PresentationParameters.MultiSampleQuality );
            //renderTarget = new RenderTarget2D( device, (int)512, (int)512, 1, SurfaceFormat.Color
            //    , device.PresentationParameters.MultiSampleType
            //    , device.PresentationParameters.MultiSampleQuality );

            viewPort = device.Viewport;
            viewPort.Height = (int)targetSize.Y;
            viewPort.Width = (int)targetSize.X;
            viewPort.X = 0;
            viewPort.Y = 0;



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void OnDraw( DrawEventArgs e )
        {
            if ( !visible ) return;

            if ( renderTexture == null )
            {
                Invalidate();
                return;
            }
            else
            {
                if ( renderTexture.IsDisposed ) throw new Exception( "TODO: if this is true then we need to ReDraw this control" );
            }


            //e.SpriteBatch.Begin();
            e.SpriteBatch.Draw( renderTexture, position, Color.White );
            //e.SpriteBatch.End();
        }

        protected virtual void OnDrawBackground( GuiGraphics graphics )
        {
            graphics.Device.Clear( ClearOptions.Target, backgroundColor, 1, 0 );
        }

        protected virtual void OnDrawChildren( DrawEventArgs e )
        {
            for ( int i = controls.Count - 1; i >= 0; i-- )
            {
                controls[ i ].Draw( e );
            }

        }

        /// <summary>
        /// Causes the control to redraw the invalidated regions within its client area.
        /// </summary>
        //public void Update()
        //{
        //    if ( dirty )
        //    {

        //        ForceUpdate();
        //    }
        //}

        public void OnUpdate( DrawEventArgs e )
        {
            if ( dirty )
            {
                ForceUpdate( e );
            }
        }

        public void ForceUpdate( DrawEventArgs e )
        {
            if ( !visible ) return;
            //if ( renderTexture != null ) renderTexture.Dispose();
            if ( renderTarget.Width != Size.X || renderTarget.Height != Size.Y )
            {
                Load( service.Game.GraphicsDevice );
            }

            //if ( Graphics == null ) return;
            Viewport oldViewport = e.Graphics.Device.Viewport;

            e.Graphics.Device.Viewport = viewPort;

            e.Graphics.Device.RenderState.DepthBufferEnable = false;
            //RenderTarget oldTarget = e.Graphics.Device.GetRenderTarget( 0 );

            e.Graphics.Device.SetRenderTarget( 0, renderTarget );
            e.Graphics.SpriteBatch.Begin();

            OnDrawBackground( e.Graphics );
            OnDrawChildren( e );

            e.Graphics.SpriteBatch.End();

            e.Graphics.Device.SetRenderTarget( 0, null );


            //Texture2D tex = renderTarget.GetTexture();
            renderTexture = renderTarget.GetTexture();

            e.Graphics.Device.Viewport = oldViewport;

            dirty = false;

            if ( parent != null ) parent.Invalidate();

        }

        public void Draw( DrawEventArgs e )
        {
            OnDraw( e );
        }

        public void OnMoved( EventArgs e )
        {
            if ( Moved != null ) Moved( this, e );
        }

        public event EventHandler<EventArgs> Moved;


        public void OnDragStarted( EventArgs e )
        {
            if ( DragStarted != null ) DragStarted( this, e );
        }

        public void OnDragEnded( EventArgs e )
        {
            if ( DragEnded != null ) DragEnded( this, e );
        }

        public event EventHandler<EventArgs> DragStarted;
        public event EventHandler<EventArgs> DragEnded;

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseMove;

        public virtual void OnMouseDown( MouseEventArgs e )
        {
            if ( MouseDown != null ) MouseDown( this, e );
        }
        public virtual void OnMouseUp( MouseEventArgs e )
        {
            if ( MouseUp != null ) MouseUp( this, e );
        }
        public virtual void OnMouseMove( MouseEventArgs e )
        {
            if ( MouseMove != null ) MouseMove( this, e );
        }

        public void StartDrag()
        {
            Service.StartDrag( this );
        }


        public Rectangle GetBounding()
        {
            return new Rectangle( (int)position.X, (int)position.Y, (int)size.X, (int)size.Y );
        }

        public virtual bool CheckOnControl( Point p )
        {
            return GetBounding().Contains( p );
            //if ( p.X >= position.X && p.Y >= position.Y && p.X <= position.X + size.X && p.Y <= position.Y + size.Y )
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public static void TestRender001()
        {
            TestServerClientMain main = null;

            GuiControl control = null;
            TestServerClientMain.Start( "TestRenderGuiControl001",
                delegate
                {
                    main = TestServerClientMain.Instance;
                    control = new GuiControl();

                    control.position = new Vector2( 100, 100 );
                    control.size = new Vector2( 200, 100 );
                    control.backgroundColor = Color.RoyalBlue;
                    control.Load( main.XNAGame.GraphicsDevice );
                },
                delegate
                {
                    DrawEventArgs e = new DrawEventArgs();
                    e.Device = main.XNAGame.GraphicsDevice;
                    e.SpriteBatch = main.SpriteBatch;

                    control.Draw( e );

                } );
        }

        #region IDisposable Members

        private bool disposed = false;

        public void Dispose()
        {
            if ( !disposed )
            {
            }
            if ( parent != null )
                parent.controls.Remove( this );
            parent = null;
            disposed = true;

            for ( int i = 0; i < controls.Count; i++ )
            {
                controls[ i ].Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
