using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using DirectX11.Graphics;
using DirectX11.Input;
using MHGameWork.TheWizards.TestRunner;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using Device = SlimDX.Direct3D11.Device;

namespace DirectX11
{
    /// <summary>
    /// This is actually a helper class used in testing. It can be used across all classes to simplify development.
    /// However, this class might be removed later, or this class could be a front-end class (facade).
    /// Currently, this is implemented as a (partial) facade for DX11 (partial because it exposes the Device)
    /// </summary>
    public class DX11Game : IGraphicsManager
    {

        public DX11Game()
        {
            form = new DX11Form();
            form.GameLoopEvent += messageLoop;
            RenderAxis = true;
            basicShaders = new List<BasicShader>();

            AllowF3InputToggle = true;
            InputDisabled = false;

        }

        void gameLoop()
        {
            while (true)
            {
                messageloopIdle.WaitOne(); // Wait for messageloop to become idle
                messageloopIdle.Reset(); // Event handled?

                Console.WriteLine("Gameloop");

                gameLoopStep();

                gameloopIdle.Set(); // Done!    
            }

        }

        void gameLoopStep()
        {

            if (TotalRunTime > 3 && TestRunner.IsRunningAutomated)
            {
                Exit();
                return;
            }

            ResetWindowTitle();

            updateElapsed();

            SpectaterCamera.Update(Elapsed);

            form.Device.ImmediateContext.ClearRenderTargetView(form.RenderTargetView, Color.Yellow);

            updateBasicShaders();

            doGameLoopEvent();

            renderAxisLines();

            LineManager3D.Render(Camera);
        }


        private void messageLoop()
        {
            messageloopIdle.Set(); // Idle
            gameloopIdle.WaitOne(); // wait for gameloop to finish
            gameloopIdle.Reset(); // Event handled?

            Console.WriteLine("Messageloop");


            updateInput();

            if (!Running)
            {

                if (diDevice == null)
                    return;

                // shutdown!
                form.Exit();

                diKeyboard.Dispose();
                diMouse.Dispose();
                diDevice.Dispose();

                diKeyboard = null;
                diMouse = null;
                diDevice = null;

                return;
            }


            lock (pauseLock)
            {
                if (paused)
                {
                    // Check if we need to unpause
                    if (Keyboard.IsKeyReleased(Key.Space) || keyboard.IsKeyReleased(Key.Return))
                    {
                        paused = false;
                        
                    }

                    Monitor.Pulse(pauseLock); // This causes the gameloop to say its idle :P careful!

                }
                else
                {
                    // Not paused, run normally

                    // Nothing to do! ==> idle

                }
            }
        }

        private ManualResetEvent messageloopIdle = new ManualResetEvent(false);
        private ManualResetEvent gameloopIdle = new ManualResetEvent(false);

        private object pauseLock = new object();

        private bool paused;

        /// <summary>
        /// This pauses the gameloop, but keeps the messageloop responsive
        /// To be called from within the gameloop!!!
        /// </summary>
        public void Pause()
        {
            lock (pauseLock)
            {
                paused = true;
                while (paused)
                {
                    gameloopIdle.Set();
                    Monitor.Wait(pauseLock);
                }
            }
        }

        private void doGameLoopEvent()
        {
            Performance.BeginEvent(new Color4(1, 1, 0), "GameLoop");
            if (GameLoopEvent != null) GameLoopEvent(this);
            Performance.EndEvent();
        }

        private void updateBasicShaders()
        {
            for (int i = 0; i < basicShaders.Count; i++)
            {
                basicShaders[i].Update();
            }
        }

        private void renderAxisLines()
        {
            if (!RenderAxis) return;
            var old = LineManager3D.DrawGroundShadows;
            LineManager3D.DrawGroundShadows = false;
            LineManager3D.WorldMatrix = Matrix.Identity;
            LineManager3D.AddLine(new Vector3(0, 0, 0), new Vector3(10, 0, 0), Color.Red);
            LineManager3D.AddLine(new Vector3(0, 0, 0), new Vector3(0, 10, 0), Color.Green);
            LineManager3D.AddLine(new Vector3(0, 0, 0), new Vector3(0, 0, 10), Color.Blue);

            LineManager3D.DrawGroundShadows = old;
        }

        protected bool RenderAxis { get; set; }

        private bool lastMouseEnabledState;
        private bool lastIsMouseVisible;
        private bool inputDisabled;
        public bool InputDisabled
        {
            get { return inputDisabled; }
            set
            {
                inputDisabled = value;
                if (mouse == null) return; //TODO: this is fishy
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

        protected bool IsMouseVisible
        {
            get { return true; }
            set
            { //TODO
                var f = value;
            }
        }

        public bool AllowF3InputToggle { get; set; }
        bool inputJustToggled = false;

        private Boolean isMouseExclusive = true;

        private void updateInput()
        {
            var acquire = diKeyboard.Acquire();
            if (acquire.IsFailure)
            {
                keyboard.UpdateKeyboardState(new KeyboardState()); // reset!!
                return;
            }

            if (!mouse.CursorEnabled != isMouseExclusive)
            {
                // Exclusive mode is incorrect
                // set to correct exclusive mode
                diMouse.Unacquire();
                if (mouse.CursorEnabled)
                {
                    diMouse.SetCooperativeLevel(form.Form, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
                }
                else
                {

                    diMouse.SetCooperativeLevel(form.Form, CooperativeLevel.Exclusive | CooperativeLevel.Foreground);
                }

                isMouseExclusive = !mouse.CursorEnabled;
            }


            var keyboardState = diKeyboard.GetCurrentState();


            if (AllowF3InputToggle && keyboardState.PressedKeys.Contains(Key.F3) && !inputJustToggled)
            {
                InputDisabled = !InputDisabled;
                inputJustToggled = true;
            }
            if (!keyboardState.PressedKeys.Contains(Key.F3))
                inputJustToggled = false;

            if (InputDisabled) return;


            //MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            keyboard.UpdateKeyboardState(keyboardState);
            if (diMouse.Acquire().IsSuccess)
                mouse.UpdateMouseState(diMouse.GetCurrentState());

            // Allows the game to exit
            if (keyboard.IsKeyDown(Key.Escape))
                Exit();
        }

        private void updateElapsed()
        {
            var nextFrameTime = SlimDX.Configuration.Timer.Elapsed;
            Elapsed = (float)(nextFrameTime - lastFrameTime).TotalSeconds;
            lastFrameTime = nextFrameTime;
            TotalRunTime += Elapsed;

            fpsCalculater.AddFrame(Elapsed);

            AddToWindowTitle( FPS.ToString());

            if (Elapsed > 1 / 30f) Elapsed = 1 / 30f;

        }

        public Device Device { get { return form.Device; } }
        public event Action<DX11Game> GameLoopEvent;
        private TimeSpan lastFrameTime;
        private TWKeyboard keyboard;
        private TWMouse mouse;
        private DX11Form form;
        private Keyboard diKeyboard;
        private Mouse diMouse;
        private DirectInput diDevice;
        private List<BasicShader> basicShaders;
        public TextureRenderer TextureRenderer { get; private set; }
        public float Elapsed { get; private set; }
        public float TotalRunTime { get; private set; }
        public bool IsDirectXInitialized { get { return form.IsDirectXInitialized; } }

        public RenderTargetView BackBufferRTV
        {
            get { return form.RenderTargetView; }
        }

        public void InitDirectX()
        {
            form.InitDirectX();


            keyboard = new TWKeyboard();


            diDevice = new SlimDX.DirectInput.DirectInput();
            diKeyboard = new SlimDX.DirectInput.Keyboard(diDevice);
            diKeyboard.SetCooperativeLevel(form.Form, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
            diKeyboard.Acquire();


            mouse = new TWMouse();
            diMouse = new SlimDX.DirectInput.Mouse(new DirectInput());
            diMouse.SetCooperativeLevel(form.Form, CooperativeLevel.Exclusive | CooperativeLevel.Foreground);

            SpectaterCamera = new SpectaterCamera(keyboard, mouse);
            Camera = SpectaterCamera;



            LineManager3D = new LineManager3D(form.Device);
            TextureRenderer = new TextureRenderer(form.Device);
            HelperStates = new HelperStatesContainer(form.Device);

        }
        public void Run()
        {
            if (!IsDirectXInitialized)
                InitDirectX();

            Running = true;

            var t = new Thread(gameLoop);
            t.Name = "DX11Game:GameLoop";
            t.Start();


            form.Run();

        }
        public void Exit()
        {
            

            Running = false;


        }
        public ICamera Camera { get; set; }



        // Helper
        public SpectaterCamera SpectaterCamera { get; private set; }
        public LineManager3D LineManager3D { get; private set; }
        /// <summary>
        /// Contains some pipeline states for easy use.
        /// </summary>
        public HelperStatesContainer HelperStates { get; private set; }
        public float FPS { get { return fpsCalculater.AverageFps; } }
        private MHGameWork.TheWizards.Graphics.AverageFPSCalculater fpsCalculater = new MHGameWork.TheWizards.Graphics.AverageFPSCalculater();

        private bool running;
        public bool Running
        {
            get { lock(this) return running; }
            private set { lock(this) running = value; }
        }

        public TWKeyboard Keyboard
        {
            get { return keyboard; }
        }

        public TWMouse Mouse
        {
            get { return mouse; }
        }

        void IGraphicsManager.AddBasicShader(BasicShader shader)
        {
            basicShaders.Add(shader);
        }

        public void AddToWindowTitle(string text)
        {
            form.Form.BeginInvoke(new Action(delegate { form.Form.Text += text; }));
        }
        public void ResetWindowTitle()
        {
            form.Form.BeginInvoke(new Action(delegate { form.Form.Text = ""; }));
        }

        /// <summary>
        /// Sets the OutputMerger's targets and depthstencilview to the backbuffer's, and sets the viewport
        /// </summary>
        public void SetBackbuffer()
        {
            form.SetBackbuffer();
        }

        public class HelperStatesContainer
        {
            private readonly Device device;

            public HelperStatesContainer(Device device)
            {
                this.device = device;

                RasterizerShowAll = RasterizerState.FromDescription(device, new RasterizerStateDescription
                                                                                {
                                                                                    CullMode = CullMode.None,
                                                                                    FillMode = FillMode.Solid
                                                                                });
            }

            /// <summary>
            /// Culling is disabled and fillmode is solid
            /// </summary>
            public RasterizerState RasterizerShowAll { get; set; }
        }
    }
}
