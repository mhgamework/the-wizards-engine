using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace MHGameWork.TheWizards.ServerClient
{
    class EditorXNA : Game
    {
              GraphicsDeviceManager graphics;
        ICamera camera;
        TWKeyboard keyboard;
        TWMouse mouse;
        private bool escapeExits = true;
        private XNAGameFiles engineFiles;
        private string rootDirectory;
        //private List<IGameObject> gameObjects = new List<IGameObject>();
        private float elapsed;
        private LineManager3D lineManager3D;
        private bool renderAxis = true;

        private Gui.GuiServiceXNA guiService;

        public Gui.GuiServiceXNA GuiService
        {
            get { return guiService; }
            set { guiService = value; }
        }

        private SpriteBatch spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        private ICursor cursor;

        public ICursor Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }




        /// <summary>
        /// Gets or sets if an axis should be rendered at the origin. 
        /// </summary>
        public bool RenderAxis
        {
            get { return renderAxis; }
            set { renderAxis = value; }
        }




        public EditorXNA()
        {
            Microsoft.Xna.Framework.Game f = new Game();
            
            graphics = new GraphicsDeviceManager(this  );
            Content.RootDirectory = "";
            //Content.RootDirectory = "Content";
            
            rootDirectory = System.Windows.Forms.Application.StartupPath + "\\";

            keyboard = new TWKeyboard();
            //mouse = new TWMouse( this );
            engineFiles = new XNAGameFiles();
            engineFiles.LoadDefaults( rootDirectory );

            //SpectaterCamera cam = new SpectaterCamera( this );
            //SetCamera( cam );

            //guiService = new MHGameWork.TheWizards.ServerClient.Gui.GuiServiceXNA( this );

            //cursor = new Cursor( this, EngineFiles.DefaultCursor, new Vector2( 10, 10 ) );

        }

        /*public void AddGameObject( IGameObject obj )
        {
            gameObjects.Add( obj );
        }*/

        public void SetCamera( ICamera cam )
        {
            camera = cam;
        }



        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// 
        /// TODO: MHGW: Initialize should be made obsolete. The shaders, models, textures, etc should be able
        /// to be created at any moment, and load themselves when the device is ready.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: This should be in the constructor (see method summary)
            //lineManager3D = new LineManager3D( this );
            spriteBatch = new SpriteBatch( GraphicsDevice );

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch( GraphicsDevice );

            // TODO: use this.Content to load your game content here
            cursor.Load();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override sealed void Update( GameTime gameTime )
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateInput();

            // Allows the game to exit
            if ( escapeExits && keyboard.IsKeyDown( Keys.Escape ) )
                this.Exit();

           /* for ( int i = 0; i < gameObjects.Count; i++ )
            {
                gameObjects[ i ].Process();
            }*/
            guiService.Process();

            if ( mouse.CursorEnabled )
            {
                cursor.Position = new Vector2( mouse.CursorPosition.X, mouse.CursorPosition.Y );
                cursor.Update();
            }
            Update();



            base.Update( gameTime );
        }

        protected virtual void Update()
        {

        }

        protected virtual void UpdateInput()
        {
            KeyboardState keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            //MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            keyboard.UpdateKeyboardState( keyboardState );
            mouse.UpdateMouseState();



        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        ///  
        protected override sealed void Draw( GameTime gameTime )
        {
            graphics.GraphicsDevice.Clear( Color.CornflowerBlue );

            Draw();

           /* for ( int i = 0; i < gameObjects.Count; i++ )
            {
                gameObjects[ i ].Render();
            }*/

            lineManager3D.Render();

            guiService.Render();
            if ( mouse.CursorEnabled )
                cursor.Render();


            base.Draw( gameTime );
        }

        protected virtual void Draw()
        {
            if ( renderAxis )
            {
                lineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 10, 0, 0 ), Color.Red );
                lineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 10, 0 ), Color.Green );
                lineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 10 ), Color.Blue );
            }
        }




        public bool EscapeExits
        {
            get { return escapeExits; }
            set { escapeExits = value; }
        }

        public TWMouse Mouse
        {
            get { return mouse; }
        }
        public TWKeyboard Keyboard
        {
            get { return keyboard; }
        }

        public ICamera Camera
        {
            get { return camera; }
        }

        public LineManager3D LineManager3D
        {
            get { return lineManager3D; }
        }

        /// <summary>
        /// The time elapsed during the last frame. 
        /// This can be smaller than the real time elapsed to prevent inaccurate calculates (for ex. in movement)
        /// </summary>
        public float Elapsed
        {
            get { return elapsed; }
        }

        public string RootDirectory
        {
            get { return rootDirectory; }
        }

        public XNAGameFiles EngineFiles
        {
            get { return engineFiles; }
        }


        public static void TestRunXNAGame()
        {
            XNAGame game = new XNAGame();

            try
            {
                game.Run();
            }
            finally
            {

                game.Dispose();
            }

        }

    }
}
