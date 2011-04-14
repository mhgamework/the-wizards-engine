using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay.Core;
using MHGameWork.TheWizards.ServerClient.Engine;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards.ServerClient
{
    public class ServerClientMainOud : AdminClient.AdminClientMain, Game3DPlay.Core.Elements.IRenderable
    {
        private ModelManager modelManager;
        private ShaderManager shaderManager;
        private TextureManager textureManager;

        private GameFileManager gameFileManager;

        public GameFileManager GameFileManager
        {
            get { return gameFileManager; }
        }

        private CoreFileManager coreFileManager;

        public CoreFileManager CoreFileManager
        {
            get { return coreFileManager; }
            set { coreFileManager = value; }
        }

        private LoadingManager loadingManager;

        public LoadingManager LoadingManager
        {
            get { return loadingManager; }
            set { loadingManager = value; }
        }

        private Gui.GuiServiceXNA guiService;

        public Gui.GuiServiceXNA GuiService
        {
            get { return guiService; }
            set { guiService = value; }
        }

        private Cursor002 cursor;

        public Cursor002 Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }

        private Engine.LineManager3D lineManager3D;

        private Server.ServerMainNew serverMain;
        public Client.PhysicsDebugRenderer serverDebugRenderer;

        private Wereld.WereldOud wereld;
        private GameClient gameClient;

        public static ServerClientMainOud instance;

        protected Network.ProxyServer server;

        private GameSettings settings;

        public GameSettings Settings
        {
            get { return settings; }
        }

        private bool cursorEnabled;

        public bool CursorEnabled
        {
            get { return cursorEnabled; }
            set { cursorEnabled = value; }
        }

        private int gameIniFileID = -1;

        public int GameIniFileID
        {
            get { return gameIniFileID; }
            set { gameIniFileID = value; }
        }

        protected void UpdateCursorEnabled()
        {
            if ( CursorEnabled )
            {
                XNAGame.IsMouseVisible = false;
            }
            else
            {
                XNAGame.IsMouseVisible = false;
            }
        }

        public ServerClientMainOud()
            : base()
        {
            instance = this;
            int temp = 0;
            temp += temp;
            serverMain = new MHGameWork.TheWizards.Server.ServerMainNew();

            //Input bug: the creation of the server sets the mouse windowhandle to the server window
            Microsoft.Xna.Framework.Input.Mouse.WindowHandle = XNAGame.Window.Handle;


            XNAGame.Activated += new EventHandler( XNAGame_Activated );
            XNAGame.Deactivated += new EventHandler( XNAGame_Deactivated );
            XNAGame.Graphics.DeviceCreated += new EventHandler( Graphics_DeviceCreated );
            XNAGame.Graphics.DeviceReset += new EventHandler( Graphics_DeviceReset );



            modelManager = new ModelManager( this );
            shaderManager = new ShaderManager( this );
            textureManager = new TextureManager( this );

            lineManager3D = new Engine.LineManager3D( this );

            gameFileManager = new GameFileManager( this, System.Windows.Forms.Application.StartupPath + @"\ServerSavedData", System.Windows.Forms.Application.StartupPath + @"\SavedData" );
            coreFileManager = new CoreFileManager( this );

            loadingManager = new LoadingManager( this );

            //guiService = new MHGameWork.TheWizards.ServerClient.Gui.GuiServiceXNA( this );
            cursor = new Cursor002( this );

            debugRenderer.Enabled = false;

            serverDebugRenderer = new Client.PhysicsDebugRenderer( this,
                                            new NovodexWrapper.NovodexDebugRenderer( serverMain.XNAGame.Graphics.GraphicsDevice ),
                                            serverMain.PhysicsScene );
            serverDebugRenderer.Enabled = true;

            wereld = new MHGameWork.TheWizards.ServerClient.Wereld.WereldOud( this );

            server = new MHGameWork.TheWizards.ServerClient.Network.ProxyServer( this );
            Server.GetGameFileDataCompleted += new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetGameFileDataEventArgs>( Server_GetGameFileDataCompleted );

            gameClient = new GameClient( this );


            string settingsFile = System.Windows.Forms.Application.StartupPath + @"\Settings.xml";

            settings = new GameSettings();
            if ( System.IO.File.Exists( settingsFile ) )
                settings = GameSettings.LoadSettings( settingsFile );


        }

        void GraphicsDevice_Disposing( object sender, EventArgs e )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        void GraphicsDevice_DeviceLost( object sender, EventArgs e )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        void GraphicsDevice_DeviceReset( object sender, EventArgs e )
        {
            //XNAGame.Graphics.GraphicsDevice.DisplayMode.Height;
            //throw new Exception( "The method or operation is not implemented." );
        }

        void Graphics_DeviceReset( object sender, EventArgs e )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        void Graphics_DeviceCreated( object sender, EventArgs e )
        {
            XNAGame.GraphicsDevice.DeviceReset += new EventHandler( GraphicsDevice_DeviceReset );
            XNAGame.GraphicsDevice.DeviceLost += new EventHandler( GraphicsDevice_DeviceLost );
            XNAGame.GraphicsDevice.Disposing += new EventHandler( GraphicsDevice_Disposing );
        }

        void XNAGame_Deactivated( object sender, EventArgs e )
        {
            XNAGame.IsMouseVisible = true;
            //throw new Exception( "The method or operation is not implemented." );
        }

        void XNAGame_Activated( object sender, EventArgs e )
        {
            UpdateCursorEnabled();
            //throw new Exception( "The method or operation is not implemented." );
        }


        public override void Dispose()
        {
            base.Dispose();
            ServerMain.Dispose();
        }

        public void CreateProxyServer()
        {
            server = new Network.ProxyServer( this );
        }


        public void SetGameIniFileID( int id )
        {
            gameIniFileID = id;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="br"></param>
        public void SetGameFilesList( Common.ByteReader br )
        {
            throw new Exception( "Deprecated" );
            //GameFileManager.Load( br.BaseStream );


        }

        /// <summary>
        /// Synchronizes the gamefile and its data.
        /// Redundant calls we be ignored.
        /// </summary>
        public void SynchronizeGameFileAsync( GameFileOud file )
        {
            if ( file.State == GameFileOud.GameFileState.Synchronizing ) return;
            file.OnStartedSynchronizing();
            Server.GetGameFileDataAsync( file );

        }

        void Server_GetGameFileDataCompleted( object sender, MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetGameFileDataEventArgs e )
        {
            GameFileOud file = GameFileManager.GetGameFile( e.ID );

            file.AssetName = e.AssetName;
            file.OnSynchronizingComplete( e.Version );

            System.IO.FileStream fs = null;
            try
            {
                fs = System.IO.File.Open( file.GetFullFilename(), System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None );
                fs.Write( e.data, 0, e.data.Length );
            }
            finally
            {
                if ( fs != null )
                    fs.Close();
            }



            if ( SynchronizeGameFileCompleted != null )
                SynchronizeGameFileCompleted( this, new SynchronizeGameFileEventArgs( file ) );







        }

        public class SynchronizeGameFileEventArgs : EventArgs
        {
            public GameFileOud File;
            public SynchronizeGameFileEventArgs( GameFileOud nFile )
            {
                File = nFile;
            }
        }

        public event EventHandler<SynchronizeGameFileEventArgs> SynchronizeGameFileCompleted;


        public override void Initialize()
        {
            base.Initialize();
            serverMain.Initialize();
        }

        public override void DoProcess( object sender, float nElapsed )
        {
            base.DoProcess( sender, nElapsed );
            serverMain.DoProcess( sender, nElapsed );
        }

        //MouseState centerMouseState;
        Microsoft.Xna.Framework.Point prevMousePosMoved = new Microsoft.Xna.Framework.Point( -1, -1 );

        protected override void ProcessInput()
        {


            if ( XNAGame.IsActive )
            {

                KeyboardState keyboard = Keyboard.GetState();

                _processEventArgs.Keyboard.UpdateKeyboardState( keyboard );


                if ( Keyboard.GetState().IsKeyDown( Keys.Escape ) )
                {
                    Exit();
                }


            }



            if ( CursorEnabled == false )
            {
                if ( XNAGame.IsActive )
                {

                    MouseState mouse = Mouse.GetState();
                    _processEventArgs.Mouse.UpdateMouseState( mouse );

                    if ( prevMousePosMoved.X != -1 )
                    {
                        _processEventArgs.Mouse.SetRelativePos( mouse.X - prevMousePosMoved.X, mouse.Y - prevMousePosMoved.Y );
                        prevMousePosMoved = new Microsoft.Xna.Framework.Point( -1, -1 );

                    }
                    if ( true )
                    {
                        //Method 1: higher framerate, possible flickering of cursor when moving mouse real fast.

                        //Only works when screensize is bigger than 200 x 200

                        if ( mouse.X < 100 || mouse.Y < 100 || mouse.X > XNAGame.Window.ClientBounds.Width - 100 || mouse.Y > XNAGame.Window.ClientBounds.Height - 100 )
                        {
                            Mouse.SetPosition( XNAGame.Window.ClientBounds.Width >> 1, XNAGame.Window.ClientBounds.Height >> 1 );
                            prevMousePosMoved = new Microsoft.Xna.Framework.Point( XNAGame.Window.ClientBounds.Width >> 1, XNAGame.Window.ClientBounds.Height >> 1 );
                            //centerMouseState = Mouse.GetState();
                            //_processEventArgs.Mouse.UpdateMouseState( mouse, centerMouseState );
                            //_backbufferCenterX = XNAGame.Window.ClientBounds.Left + XNAGame.Window.ClientBounds.Width >> 1;
                            //_backbufferCenterY = XNAGame.Window.ClientBounds.Top + XNAGame.Window.ClientBounds.Height >> 1;
                            //Mouse.SetPosition( _backbufferCenterX, _backbufferCenterY );
                        }
                        else
                        {
                            //_processEventArgs.Mouse.UpdateMouseState( mouse );
                        }
                    }
                    else
                    {
                        //Method 2: lower framerate, low chance of flickering.
                        //TODO

                    }

                }



            }
            else
            {
                //Cursor Enabled
                MouseState mouse = Mouse.GetState();
                _processEventArgs.Mouse.UpdateMouseState( mouse );

                if ( prevMousePosMoved.X != -1 )
                {
                    _processEventArgs.Mouse.SetRelativePos( mouse.X - prevMousePosMoved.X, mouse.Y - prevMousePosMoved.Y );
                    prevMousePosMoved = new Microsoft.Xna.Framework.Point( -1, -1 );

                }
            }




        }

        /*public override void DoTick(object sender, float nElapsed)
        {
            base.DoTick(sender, nElapsed);
            serverMain.DoTick(sender, nElapsed);
        }*/

        public virtual void OnBeforeRender( object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e )
        {

        }

        public virtual void OnRender( object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e )
        {
            //guiService.Render();

        }

        public virtual void OnAfterRender( object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e )
        {

            //TODO: probably not a good place for this.
            if ( CursorEnabled )
            {
                SpriteBatch.Begin();
                cursor.Render( SpriteBatch );
                SpriteBatch.End();
            }
        }

        public override void OnLoad( object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e )
        {
            base.OnLoad( sender, e );
            ModelManager.Load( sender, e );
            shaderManager.Load( sender, e );
            textureManager.Load();
            lineManager3D.Load();
            cursor.Load( XNAGame.Content.RootDirectory + @"\Content\Cursor001.dds" );
            cursor.PointerPosition = new Microsoft.Xna.Framework.Vector2( 11, 9 );

            serverDebugRenderer.DebugRenderer.setRenderDevice( XNAGame.Graphics.GraphicsDevice );
        }

        public override void OnUnload( object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e )
        {

            base.OnUnload( sender, e );
            ModelManager.Unload( sender, e );
            shaderManager.Unload( sender, e );
            textureManager.Unload();
            //lineManager3D.Unload(sender,e);
        }

        public override void OnProcess( object sender, MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            base.OnProcess( sender, e );
            //guiService.Process( e );
            cursor.Position = new Microsoft.Xna.Framework.Vector2( _processEventArgs.MouseStateOld.X, _processEventArgs.MouseStateOld.Y );
        }

        public override void OnTick( object sender, MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            base.OnTick( sender, e );
            guiService.Tick( e );
        }


        public ModelManager ModelManager { get { return modelManager; } }
        public ShaderManager ShaderManager { get { return shaderManager; } }
        public TextureManager TextureManager { get { return textureManager; } }

        public Engine.LineManager3D LineManager3D { get { return lineManager3D; } }

        public Game3DPlay.Core.Elements.ProcessEventArgs ProcessEventArgs { get { return _processEventArgs; } }

        public Server.ServerMainNew ServerMain
        { get { return serverMain; } }

        public Wereld.WereldOud Wereld
        { get { return wereld; } }

        public Network.ProxyServer Server { get { return server; } }

        public int Time { get { return _processEventArgs.Time; } }

        public GameClient GameClient
        { get { return gameClient; } }

        public string RootDirectory
        { get { return XNAGame.Content.RootDirectory; } }
    }
}
