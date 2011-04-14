using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace MHGameWork.Game3DPlay.Core
{
    public class BaseHoofdObject : SpelObject, Elements.ILoadable
    {
        private int tickCount;

        public BaseHoofdObject()
            : base()
        {
            HoofdObj = this;

            _processContainer = new Elements.ProcessContainer( this );
            _tickContainer = new Elements.TickContainer( this );
            _renderContainer = new Elements.RenderContainer( this );



            CreateStandardElements( this );

            _loadElement = GetElement<Elements.LoadElement>();

            XNAGame = new XNAGame( this );


            _processEventArgs = new MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs( this );

            _tickEventArgs = new MHGameWork.Game3DPlay.Core.Elements.TickEventArgs( this );

            TickRate = 1f / 30f;

            FPSUpdateRate = 1000;

            _renderEventArgs = new MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs();
            _renderEventArgs.CameraInfo = new CameraInfo();
            _renderEventArgs.Graphics = XNAGame.Graphics;

            cameraInfo.ViewMatrix = Matrix.CreateLookAt( new Vector3( 0, 0, -4000 ), Vector3.Zero, Vector3.Up );
            cameraInfo.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView( MathHelper.ToRadians( 45.0f ),
                    4 / 3, 1.0f, 10000.0f );
            //aspectRatio, 1.0f, 10000.0f);


            Invoker = new Invoker();



        }

        protected int _backbufferCenterX;
        protected int _backbufferCenterY;

        public override void Initialize()
        {
            base.Initialize();

        }



        Elements.ProcessContainer _processContainer;
        Elements.TickContainer _tickContainer;
        Elements.RenderContainer _renderContainer;
        Elements.LoadElement _loadElement;

        protected XNAGame _xnaGame;
        public XNAGame XNAGame
        {
            get { return _xnaGame; }
            private set { _xnaGame = value; }
        }

        private float _preciseTime;

        public float PreciseTime
        {
            get { return _preciseTime; }
            set { _preciseTime = value; }
        }

        private int _FPS;

        public int FPS
        {
            get { return _FPS; }
            set { _FPS = value; }
        }

        private float fpsFrameCount = 0;
        private float fpsLastUpdate = 0;

        private int _FPSUpdateRate;

        public int FPSUpdateRate
        {
            get { return _FPSUpdateRate; }
            set { _FPSUpdateRate = value; }
        }

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; }
        }




        protected Elements.ProcessEventArgs _processEventArgs;
        public Elements.TickEventArgs _tickEventArgs;

        public virtual void DoProcess( object sender, float nElapsed )
        {

            //if (nElapsed < 0.001f) System.Threading.Thread.Sleep(1); //Debug: Prevent elapsed to go under 1ms because the engine gets inaccurate


            PreciseTime += nElapsed;

            //_processEventArgs.GameTime = gameTime;
            _processEventArgs.Elapsed = nElapsed;
            _processEventArgs.Time = (int)( PreciseTime * 1000 );


            fpsFrameCount++;
            if ( _processEventArgs.Time > fpsLastUpdate + FPSUpdateRate )
            {
                if ( _processEventArgs.Time - fpsLastUpdate <= 0 )
                { FPS = -1; }
                else
                {
                    FPS = (int)( ( fpsFrameCount / ( _processEventArgs.Time - fpsLastUpdate ) * 1000 ) );
                }

                fpsLastUpdate = _processEventArgs.Time;
                fpsFrameCount = 0;

            }


            ProcessInput();

            _processContainer.OnProcess( sender, _processEventArgs );
            //if (_processEventArgs.Keyboard.IsKeyStateDown(Keys.I)) System.Windows.Forms.MessageBox.Show  (TimeSpan.FromSeconds(nElapsed).ToString());

            //CheckTick( TimeSpan.FromSeconds( nElapsed ) );
            CheckTick( nElapsed );

            Invoker.ProcessInvokes( _processEventArgs.Time );



        }

        //private TimeSpan tickAccumulatedElapsed;
        //private TimeSpan tickTotalTime;

        //private TimeSpan _tickRate;

        //public TimeSpan TickRate
        //{
        //    get { return _tickRate; }
        //    set { _tickRate = value; }
        //}

        public int TimeToLastTick( int nTime )
        {
            return (int)Math.Floor( nTime / ( TickRate * 1000 ) );
        }

        public void SetTime( int nTime )
        {
            _preciseTime = nTime / 1000f;
            _processEventArgs.Time = nTime;
            tickCount = TimeToLastTick( nTime );
            tickTotalTime = _preciseTime;
            tickAccumulated = ToMachineTicks( nTime / 1000f - tickCount * TickRate );


        }

        private long tickAccumulated;
        private float tickTotalTime;

        private float _tickRateSeconds;
        private long _tickRateTicks;

        public float TickRate
        {
            get { return _tickRateSeconds; }
            set
            {
                _tickRateSeconds = value;
                _tickRateTicks = ToMachineTicks( _tickRateSeconds );
            }
        }

        public long ToMachineTicks( float seconds )
        {
            return (long)( (double)seconds * (double)TimeSpan.TicksPerSecond );
        }

        //private void CheckTick(TimeSpan nElapsed)
        private void CheckTick( float nElapsed )
        {
            long ticks = (long)( (double)nElapsed * (double)TimeSpan.TicksPerSecond );

            if ( _tickRateTicks == 0 ) throw new Exception( "Tickrate can niet 0 zijn!" );


            //tickAccumulated += nElapsed;
            tickAccumulated += ticks;
            long num = tickAccumulated / _tickRateTicks;
            //tickAccumulated = TimeSpan.FromTicks(tickAccumulated % TickRate);
            tickAccumulated = ( tickAccumulated % _tickRateTicks );
            //this.lastFrameElapsedGameTime = TimeSpan.Zero;
            //TimeSpan targetElapsedTime = this.targetElapsedTime;
            if ( num > 0L )
            {
                while ( num > 1L )
                {
                    //this.gameTime.IsRunningSlowly = this.drawRunningSlowly = true;
                    num -= 1L;
                    try
                    {
                        DoTick( this, _tickRateSeconds );
                        continue;
                    }
                    finally
                    {
                        //this.lastFrameElapsedGameTime += targetElapsedTime;
                        //this.totalGameTime += targetElapsedTime;
                    }
                }
                //this.gameTime.IsRunningSlowly = false;
                try
                {
                    //this.gameTime.ElapsedGameTime = targetElapsedTime;
                    //this.gameTime.TotalGameTime = this.totalGameTime;
                    DoTick( this, _tickRateSeconds );
                }
                finally
                {
                    //this.lastFrameElapsedGameTime += targetElapsedTime;
                    //this.totalGameTime += targetElapsedTime;
                }
            }

        }


        protected virtual void ProcessInput()
        {
            if ( XNAGame.IsActive )
            {
                if ( _backbufferCenterX == -1 )
                {
                    _backbufferCenterX = XNAGame.Window.ClientBounds.Left + XNAGame.Window.ClientBounds.Width >> 1;
                    _backbufferCenterY = XNAGame.Window.ClientBounds.Top + XNAGame.Window.ClientBounds.Height >> 1;
                    Mouse.SetPosition( _backbufferCenterX, _backbufferCenterY );
                }

                KeyboardState keyboard = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();
                _processEventArgs.UpdateInput( keyboard, mouse );
                _processEventArgs.Mouse.SetRelativePos( _backbufferCenterX - mouse.X, _backbufferCenterY - mouse.Y );


                Mouse.SetPosition( _backbufferCenterX, _backbufferCenterY );



            }
            else
            {
                _backbufferCenterX = -1;
                _backbufferCenterY = -1;

            }
            if ( Keyboard.GetState().IsKeyDown( Keys.Escape ) )
            {
                Exit();
            }
        }

        public virtual void DoProcess( object sender, GameTime nGameTime )
        {
            DoProcess( sender, (float)nGameTime.ElapsedGameTime.TotalSeconds );

        }


        public virtual void DoTick( object sender, float nElapsed )
        {
            tickCount++;

            tickTotalTime += nElapsed;
            _tickEventArgs.Elapsed = nElapsed;
            _tickEventArgs.Time = (int)( tickTotalTime * 1000 );

            _tickEventArgs.UpdateInput( _processEventArgs.KeyboardState, _processEventArgs.MouseStateOld );


            _tickContainer.OnTick( sender, _tickEventArgs );


        }

        public Elements.RenderEventArgs _renderEventArgs;

        private CameraInfo cameraInfo
        {
            get { return _renderEventArgs.CameraInfo; }
            set { _renderEventArgs.CameraInfo = value; }
        }

        private Camera _activeCamera;
        public Camera ActiveCamera
        {
            get { return _activeCamera; }

        }


        public void SetCamera( Camera cam )
        {
            _activeCamera = cam;
            cameraInfo = cam.CameraInfo;
        }

        private SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
            set
            {
                _spriteBatch = value;
                _renderEventArgs.SpriteBatch = value;
            }
        }


        public virtual void DoBeforeRender( object sender )//, Elements.BaseRenderElement.RenderEventArgs e)
        {
            _renderContainer.OnBeforeRender( sender, _renderEventArgs );
        }
        public virtual void DoRender( object sender )//, Elements.BaseRenderElement.RenderEventArgs e)
        {


            _renderContainer.OnRender( sender, _renderEventArgs );

        }
        public virtual void DoAfterRender( object sender )//, Elements.BaseRenderElement.RenderEventArgs e)
        {
            _renderContainer.OnAfterRender( sender, _renderEventArgs );
        }

        public virtual void Run()
        {
            XNAGame.Run();
        }

        public virtual void Exit()
        {
            //Moet dispose ervoor of erna?

            //Dispose();

            XNAGame.Exit();

            Dispose();
        }

        public bool _inRun;
        public bool InRun
        {
            get { return _inRun; }
        }


        public virtual void CreateStandardElements( SpelObject nSpO )
        {
            if ( nSpO is Elements.IProcessable )
            {
                if ( nSpO.GetElement<Elements.ProcessElement>() == null )
                {
                    Elements.ProcessElement pE = new MHGameWork.Game3DPlay.Core.Elements.ProcessElement( (Elements.IProcessable)nSpO );
                }
            }
            if ( nSpO is Elements.ITickable )
            {
                if ( nSpO.GetElement<Elements.TickElement>() == null )
                {
                    Elements.TickElement pE = new MHGameWork.Game3DPlay.Core.Elements.TickElement( (Elements.ITickable)nSpO );
                }
            }

            if ( nSpO is Elements.IRenderable2D )
            {
                if ( nSpO.GetElement<Elements.Render2DElement>() == null )
                {
                    Elements.Render2DElement pE = new MHGameWork.Game3DPlay.Core.Elements.Render2DElement( (Elements.IRenderable2D)nSpO );
                }
            }
            else if ( nSpO is Elements.IRenderable )
            {
                if ( nSpO.GetElement<Elements.RenderElement>() == null )
                {
                    Elements.RenderElement pE = new MHGameWork.Game3DPlay.Core.Elements.RenderElement( (Elements.IRenderable)nSpO );
                }
            }
            if ( nSpO is Elements.ILoadable )
            {
                if ( nSpO.GetElement<Elements.LoadElement>() == null )
                {
                    Elements.LoadElement pE = new MHGameWork.Game3DPlay.Core.Elements.LoadElement( (Elements.ILoadable)nSpO );
                }
            }
        }






        public virtual void DoInitialize()
        {
            Initialize();


        }


        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        public virtual void DoLoadGraphicsContent( bool loadAllContent )
        {
            _backbufferCenterX = (int)( XNAGame.Graphics.GraphicsDevice.DisplayMode.Width * 0.5 );
            _backbufferCenterY = (int)( XNAGame.Graphics.GraphicsDevice.DisplayMode.Height * 0.5 );
            _loadElement.OnLoad( this, new MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs( XNAGame._content, loadAllContent ) );

        }


        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        public virtual void DoUnloadGraphicsContent( bool unloadAllContent )
        {
            _loadElement.OnUnload( this, new MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs( XNAGame._content, unloadAllContent ) );

        }



        public void RegisterNewSpO( SpelObject nSpO )
        {
            if ( _inRun )
            {
                nSpO.Initialize();
            }
        }


        #region ILoadable Members

        public virtual void OnLoad( object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e )
        {
            if ( e.AllContent )
            {
                SpriteBatch = new SpriteBatch( XNAGame.Graphics.GraphicsDevice );
            }
        }

        public virtual void OnUnload( object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e )
        {
            if ( e.AllContent )
            {
                //SpriteBatch
            }
        }

        #endregion


        private Invoker _invoker;
        public Invoker Invoker
        {
            get { return _invoker; }
            private set { _invoker = value; }
        }

        public int Tick { get { return tickCount; } }




    }

}

