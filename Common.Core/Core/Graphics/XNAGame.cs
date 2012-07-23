using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Wpf;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
using MHGameWork.TheWizards.Common.Core;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace MHGameWork.TheWizards.Graphics
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class XNAGame : Microsoft.Xna.Framework.Game, IXNAGame
    {
        /// <summary>
        /// This holds the last created instance of XNAGame. Used for testing
        /// </summary>
        private static XNAGame lastInstance;
        /// <summary>
        /// Returns the last created instance of an XNAGame. Used in testing
        /// </summary>
        /// <returns></returns>
        public static XNAGame Get()
        {
            return lastInstance;
        }

        /// <summary>
        /// Used in testing. The XNAGame will automatically shut down after a set period.
        /// Set this to -1 to disable autoshutdown
        /// </summary>
        public static float AutoShutdown { get; set; }
        public static Boolean DefaultInputDisabled { get; set; }

        static XNAGame()
        {
            AutoShutdown = -1;
        }


        GraphicsDeviceManager graphics;
        ICamera camera;

        public GraphicsDeviceManager Graphics1
        {
            get { return graphics; }
        }

        TWKeyboard keyboard;
        TWMouse mouse;
        private bool escapeExits = true;
        private XNAGameFiles engineFiles;
        private string rootDirectory;
        private List<IXNAObject> gameObjects = new List<IXNAObject>();
        private float elapsed;
        private LineManager3D lineManager3D;
        private bool renderAxis = true;
        private SpriteFont spriteFont;
        private bool drawFPS = false;
        private AverageFPSCalculater averageFPSCalculater;

        /// <summary>
        /// In the first Update call, the FrameNumber = 1
        /// </summary>
        public int FrameNumber { get; private set; }

        private float totalTime;

        public bool DrawFps
        {
            [DebuggerStepThrough]
            get { return drawFPS; }
            [DebuggerStepThrough]
            set { drawFPS = value; }
        }

        private GameTime gameTime;

        /// <summary>
        /// Returns the XNA GameTime object used in the last/current update call
        /// </summary>
        public GameTime GameTime
        {
            get { return gameTime; }
            set { gameTime = value; }
        }

        public SpriteFont SpriteFont
        {
            [DebuggerStepThrough]
            get { return spriteFont; }
        }

        /// <summary>
        /// This flag is used to check if this game terminates before drawing once.
        /// This most likely means there is something wrong with the XNAGame setup
        /// </summary>
        private bool flagFirstDraw = true;

        private ServerClient.Gui.GuiServiceXNA guiService;

        [Obsolete("Not supported anymore!")]
        public ServerClient.Gui.GuiServiceXNA GuiService
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

        [Obsolete("Not supported anymore!!")]
        public ICursor Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }


        private SpectaterCamera spectaterCamera;

        public SpectaterCamera SpectaterCamera
        {
            get { return spectaterCamera; }
        }

        /// <summary>
        /// This is a somewhat hack to get the form this XNA-game uses to render to. 
        /// It is the same form that the Graphics.Window is bound to
        /// </summary>
        /// <returns></returns>
        public Form GetWindowForm()
        {
            return (Form)Control.FromHandle(Window.Handle);
        }

        /// <summary>
        /// Gets or sets if an axis should be rendered at the origin. 
        /// </summary>
        public bool RenderAxis
        {
            get { return renderAxis; }
            set { renderAxis = value; }
        }






        public XNAGame()
        {
            lastInstance = this;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
            Content.RootDirectory = System.Windows.Forms.Application.StartupPath + "\\";

            //Content.RootDirectory = "Content";

            rootDirectory = System.Windows.Forms.Application.StartupPath + "\\";

            keyboard = new TWKeyboard();
            mouse = new TWMouse(this);
            engineFiles = new XNAGameFiles();
            engineFiles.LoadDefaults(rootDirectory);

            spectaterCamera = new SpectaterCamera(this);
            SetCamera(spectaterCamera);

            guiService = new MHGameWork.TheWizards.ServerClient.Gui.GuiServiceXNA(this);

            averageFPSCalculater = new AverageFPSCalculater();
            //cursor = new Cursor( this, EngineFiles.DefaultCursor, new Vector2( 10, 10 ) );

            AllowF3InputToggle = true;

            InputDisabled = DefaultInputDisabled;

            Wpf = new XNAGameWpf();
            AddXNAObject(Wpf);
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            //NOTE: This disables vertical sync
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.Immediate;

            // This prevents bazooka style clearing of SetRenderTarget command (causing the depthbuffer to malfunction)
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

            foreach (GraphicsAdapter curAdapter in GraphicsAdapter.Adapters)
            {
                if (curAdapter.Description.Contains("NVIDIA PerfHUD"))
                {
                    e.GraphicsDeviceInformation.Adapter = curAdapter;
                    e.GraphicsDeviceInformation.DeviceType = DeviceType.Reference;
                    System.Windows.Forms.MessageBox.Show("Using NVIDIA PerfHUD adapter!");
                    break;
                }
            }
        }



        public void AddXNAObject(IXNAObject obj)
        {
            if (gameObjects.Contains(obj))
                throw new InvalidOperationException("This object was already added!");
            gameObjects.Add(obj);


            if (FrameNumber != 0)
            {
                // Initialize has already been called! call it now!
                obj.Initialize(this);

            }
        }

        public void SetCamera(ICamera cam)
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
        /// TODO: to be created at any moment, and load themselves when the device is ready.
        /// TODO: EDIT: previous is true, however not the function of XNAGame anymore. It is supposed to be a generic XNA handling class
        /// </summary>
        protected override void Initialize()
        {
            // TODO: This should be in the constructor (see method summary)
            lineManager3D = new LineManager3D(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            loadSpriteFont();

            if (InitializeEvent != null) InitializeEvent(this, null);
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Initialize(this);
            }

            base.Initialize();
        }

        private void loadSpriteFont()
        {
            // The spritefont is stored as an embedded assembly file. Since the dumb content manager does not support loading from stream
            //   we first have to store the file on disk before when can load it.

            //TODO: find a way to load directly from stream

            string filename = System.Windows.Forms.Application.StartupPath + "\\Content\\Calibri.xnb";

            if (!System.IO.Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Content"))
            {
                System.IO.Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Content");
            }

            if (!System.IO.File.Exists(filename))
            {

                Stream strm = EmbeddedFile.GetStream("MHGameWork.TheWizards.Graphics.Files.Calibri.xnb", "Calibri.xnb");
                byte[] data = new byte[strm.Length];
                strm.Read(data, 0, (int)strm.Length);

                System.IO.File.WriteAllBytes(filename, data);

                data = null;
                strm.Close();

            }



            spriteFont = Content.Load<SpriteFont>("Content\\Calibri");
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
            //cursor.Load();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public event EventHandler InitializeEvent;
        public event XNAGameLoopEventHandler DrawEvent;
        public event XNAGameLoopEventHandler UpdateEvent;
        public delegate void XNAGameLoopEventHandler();


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override sealed void Update(GameTime _gameTime)
        {
            gameTime = _gameTime;
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FrameNumber++;
            totalTime += elapsed;

            if (totalTime > AutoShutdown && AutoShutdown > 0)
                Exit();

            UpdateInput();
            updateCursor();

            doInvokeUpdates();



            updateGameObjects();
            updateShaders();

            doUpdateEvent();


            base.Update(gameTime);
        }

        private void doUpdateEvent()
        {
            if (UpdateEvent != null) UpdateEvent();
        }

        private void updateCursor()
        {
            if (mouse.CursorEnabled && cursor != null && !InputDisabled)
            {
                cursor.Position = new Vector2(mouse.CursorPosition.X, mouse.CursorPosition.Y);
                cursor.Update();
            }
        }

        private void updateGameObjects()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update(this);
            }
        }


        private void updateShaders()
        {
            for (int i = 0; i < shaders.Count; i++)
            {
                var shader = shaders[i];
                shader.Update();
            }
        }

        bool inputJustToggled = false;
        protected virtual void UpdateInput()
        {
            KeyboardState keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            // Allows the game to exit
            if (escapeExits && keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();


            if (AllowF3InputToggle && keyboardState.IsKeyDown(Keys.F3) && !inputJustToggled)
            {
                InputDisabled = !InputDisabled;
                inputJustToggled = true;
            }
            if (!keyboardState.IsKeyDown(Keys.F3))
                inputJustToggled = false;

            if (InputDisabled) return;


            //MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            keyboard.UpdateKeyboardState(keyboardState);
            mouse.UpdateMouseState();



        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        ///  
        protected override sealed void Draw(GameTime gameTime)
        {
            // See the flagFirstDraw for details
            flagFirstDraw = false;

            averageFPSCalculater.AddFrame((float)gameTime.ElapsedRealTime.TotalSeconds);



            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            Draw();

            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Render(this);
            }

            lineManager3D.Render();

            //guiService.Render();
            //if ( mouse.CursorEnabled )
            //    cursor.Render();


            base.Draw(gameTime);
        }


        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            lastInstance = null;

            if (flagFirstDraw)
            {
                //Game terminated before calling Draw once
                //  This is most likely bad behaviour

                //Note: Maybe not with ApplicationContext, but with Application

                // This functionality was implemented for NUnit tests.
                // When multiple test of NUnit are run, creating XNAGame instances, only the first XNAGame runs, the latter shutdown
                // before the first run. 
                // NUnit tests using XNA need the [RequiresThread( System.Threading.ApartmentState.STA )] attribute
                // in order to work properly. This exception will warn the user that the test in fact failed/was not run.
                throw new Exception(
                    "This game terminated before drawing once! Most likely there is something wrong with this threads ApplicationContext. It is also not allowed to terminated the game in the first update call!");
            }

            gameObjects = null;

            // This is again for unit testing, assuring that the XNADevice is cleaned up before executing next test
            System.Threading.Thread.Sleep(500);
        }



        protected virtual void Draw()
        {
            if (DrawEvent != null) DrawEvent();
            if (renderAxis)
            {
                var old = lineManager3D.DrawGroundShadows;
                lineManager3D.DrawGroundShadows = false;
                lineManager3D.WorldMatrix = Matrix.Identity;
                lineManager3D.AddLine(new Vector3(0, 0, 0), new Vector3(10, 0, 0), Color.Red);
                lineManager3D.AddLine(new Vector3(0, 0, 0), new Vector3(0, 10, 0), Color.Green);
                lineManager3D.AddLine(new Vector3(0, 0, 0), new Vector3(0, 0, 10), Color.Blue);

                lineManager3D.DrawGroundShadows = old;
            }
            if (drawFPS)
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
                spriteBatch.DrawString(spriteFont, Math.Floor(averageFPSCalculater.AverageFps).ToString(), new Vector2(10, 10), Color.Red);
                spriteBatch.End();
            }
        }





        /// <summary>
        /// Wierd Location
        /// </summary>

        public Ray GetWereldViewRay(Vector2 pos)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            Vector3 nearSource = viewport.Unproject(new Vector3(pos.X, pos.Y, viewport.MinDepth), Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 farSource = viewport.Unproject(new Vector3(pos.X, pos.Y, viewport.MaxDepth), Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 direction = farSource - nearSource;
            direction.Normalize();

            Ray ray = new Ray(nearSource, direction);

            return ray;

        }




        public bool EscapeExits
        {
            get { return escapeExits; }
            set { escapeExits = value; }
        }

        public bool AllowF3InputToggle { get; set; }

        private bool lastMouseEnabledState;
        private bool lastIsMouseVisible;
        private bool inputDisabled;
        public bool InputDisabled
        {
            get { return inputDisabled; }
            set
            {
                inputDisabled = value;
                if (inputDisabled)
                {
                    lastMouseEnabledState = mouse.CursorEnabled;
                    lastIsMouseVisible = IsMouseVisible;
                    mouse.CursorEnabled = true;
                    IsMouseVisible = true;
                }
                else
                {
                    mouse.CursorEnabled = lastMouseEnabledState;
                    IsMouseVisible = lastIsMouseVisible;
                }
            }
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

        public Point ClientSize
        {
            get { return new Point(Window.ClientBounds.Width, Window.ClientBounds.Height); }
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

        /// <summary>
        /// This calls the update function on a list of objects
        /// TODO: these are not working at the moment
        /// </summary>
        /// <param name="objs"></param>
        public static void Update(IXNAGame game, List<IXNAObject> objs)
        {
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].Update(game);
            }
        }
        /// <summary>
        /// This calls the update function on a list of objects
        /// TODO: these are not working at the moment
        /// </summary>
        /// <param name="objs"></param>
        public static void Render(IXNAGame game, List<IXNAObject> objs)
        {
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].Render(game);
            }
        }
        /// <summary>
        /// This calls the update function on a list of objects
        /// TODO: these are not working at the moment
        /// </summary>
        /// <param name="objs"></param>
        public static void Initialize(IXNAGame game, List<IXNAObject> objs)
        {
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].Initialize(game);
            }
        }

        public bool IsCursorInWindow()
        {
            if (mouse.CursorEnabled == false) throw new InvalidOperationException("Cursor is not enabled!");

            if (mouse.CursorPosition.X < 0) return false;
            if (mouse.CursorPosition.Y < 0) return false;
            if (mouse.CursorPosition.X > Window.ClientBounds.Width) return false;
            if (mouse.CursorPosition.Y > Window.ClientBounds.Height) return false;

            return true;
        }

        private List<BasicShader> shaders = new List<BasicShader>();
        public void AddBasicShader(BasicShader basicShader)
        {
            shaders.Add(basicShader);
        }


        private ConcurrentQueue<Action> invokeUpdateQueue = new ConcurrentQueue<Action>();
        /// <summary>
        /// Invokes a delegate in the next Update of this game
        /// </summary>
        /// <param name="action"></param>
        public void InvokeUpdate(Action action)
        {
            invokeUpdateQueue.Enqueue(action);
        }

        private void doInvokeUpdates()
        {
            Action a;
            while (invokeUpdateQueue.TryDequeue(out a))
            {
                a();
            }
        }


        public XNAGameWpf Wpf { get; private set; }

    }
}