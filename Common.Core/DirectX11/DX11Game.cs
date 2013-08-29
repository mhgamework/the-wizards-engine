using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Input;
using MHGameWork.TheWizards.Profiling;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using Device = SlimDX.Direct3D11.Device;

namespace MHGameWork.TheWizards.DirectX11
{
    /// <summary>
    /// This is actually a helper class used in testing. It can be used across all classes to simplify development.
    /// However, this class might be removed later, or this class could be a front-end class (facade).
    /// Currently, this is implemented as a (partial) facade for DX11 (partial because it exposes the Device)
    /// </summary>
    public class DX11Game : IGraphicsManager
    {
        public ProfilingPoint GameLoopProfilingPoint
        {
            get { return Form.GameLoopProfilingPoint; }
        }

        public DX11Game()
        {
            form = new DX11Form();
            Form.GameLoopEvent += gameLoopStep;
            RenderAxis = true;
            basicShaders = new List<BasicShader>();

            AllowF3InputToggle = true;
            InputDisabled = false;

            EscapeExists = true;

            var viz = new FPSVizualizer(this);
            form.GameLoopEvent += viz.Update;
        }

        public class FPSVizualizer
        {
            private MHGameWork.TheWizards.Graphics.AverageFPSCalculater fpsCalculater;
            private DX11Game game;

            private StringBuilder builder = new StringBuilder();

            public FPSVizualizer(DX11Game game)
            {
                this.game = game;
                fpsCalculater = new MHGameWork.TheWizards.Graphics.AverageFPSCalculater();
                fpsCalculater.DataAvailable += fpsCalculater_DataAvailable;
            }

            void fpsCalculater_DataAvailable(float obj)
            {
                try
                {
                    game.Form.Form.BeginInvoke(new Action(() => Target(obj)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            private void Target(float obj)
            {
                try
                {
                    builder.Clear();
                    builder.Append(obj);
                    builder.Append(" - ");
                    builder.Append(1 / obj);
                    game.Form.Form.Text = builder.ToString();

                }
                catch (Exception party)
                {
                    Console.WriteLine(party);
                }
            }

            public void Update()
            {
                fpsCalculater.AddFrame(game.RealElapsed);
            }
        }

        public bool CustomGameLoopDisabled { get; set; }

        [TWProfile]
        void gameLoopStep()
        {
            if (!running)
            {
                if (exitAndCleanup()) return;
                return;
            }

            if (!Form.Active)
                Thread.Sleep(100);

            updateInput();

            updateElapsed();

            SpectaterCamera.Update(Elapsed);

            Form.Device.ImmediateContext.ClearRenderTargetView(Form.RenderTargetView, new Color4(Color.DeepSkyBlue));

            updateBasicShaders();

            doGameLoopEvent();

            renderAxisLines();

            LineManager3D.Render(Camera);
        }

        private bool exitAndCleanup()
        {
            if (diDevice == null)
                return true;

            // shutdown!
            Form.Exit();

            diKeyboard.Dispose();
            diMouse.Dispose();
            diDevice.Dispose();

            diKeyboard = null;
            diMouse = null;
            diDevice = null;
            return false;
        }


        private void doGameLoopEvent()
        {
            if (CustomGameLoopDisabled) return;
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

        public bool EscapeExists { get; set; }

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

        public bool MouseInputDisabled { get; set; }
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
                    diMouse.SetCooperativeLevel(Form.Form, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
                }
                else
                {

                    diMouse.SetCooperativeLevel(Form.Form, CooperativeLevel.Exclusive | CooperativeLevel.Foreground);
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
            if (!MouseInputDisabled)
            {
                if (diMouse.Acquire().IsSuccess)
                    mouse.UpdateMouseState(diMouse.GetCurrentState());
            }
            

            // Allows the game to exit
            if (EscapeExists && keyboard.IsKeyDown(Key.Escape))
                Exit();
        }

        private void updateElapsed()
        {
            var nextFrameTime = SlimDX.Configuration.Timer.Elapsed;
            Elapsed = (float)(nextFrameTime - lastFrameTime).TotalSeconds;
            lastFrameTime = nextFrameTime;
            TotalRunTime += Elapsed;

            RealElapsed = Elapsed;
            if (Elapsed > 1 / 30f) Elapsed = 1 / 30f;

        }


        public Device Device { get { return Form.Device; } }
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
        protected float RealElapsed { get; private set; }

        public float TotalRunTime { get; private set; }
        public bool IsDirectXInitialized { get { return Form.IsDirectXInitialized; } }

        public RenderTargetView BackBufferRTV
        {
            get { return Form.RenderTargetView; }
        }

        public void InitDirectX()
        {
            Form.InitDirectX();


            keyboard = new TWKeyboard();


            diDevice = new SlimDX.DirectInput.DirectInput();
            diKeyboard = new SlimDX.DirectInput.Keyboard(diDevice);
            diKeyboard.SetCooperativeLevel(Form.Form, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
            diKeyboard.Acquire();


            mouse = new TWMouse();
            diMouse = new SlimDX.DirectInput.Mouse(new DirectInput());
            diMouse.SetCooperativeLevel(Form.Form, CooperativeLevel.Exclusive | CooperativeLevel.Foreground);

            SpectaterCamera = new SpectaterCamera(keyboard, mouse);
            Camera = SpectaterCamera;



            LineManager3D = new LineManager3D(Form.Device);
            TextureRenderer = new TextureRenderer(Form.Device);
            HelperStates = new HelperStatesContainer(Form.Device);

        }
        public void Run()
        {
            if (!IsDirectXInitialized)
                InitDirectX();

            Running = true;
            Form.Run();

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

        private bool running;
        public bool Running
        {
            get { lock (this) return running; }
            private set { lock (this) running = value; }
        }

        public TWKeyboard Keyboard
        {
            get { return keyboard; }
        }

        public TWMouse Mouse
        {
            get { return mouse; }
        }

        public DX11Form Form
        {
            get { return form; }
        }

        void IGraphicsManager.AddBasicShader(BasicShader shader)
        {
            basicShaders.Add(shader);
        }

        [Obsolete]
        public void AddToWindowTitle(string text)
        {
            //TODO: remake? this is verrry slow?
            //Form.Form.BeginInvoke(new Action(delegate { Form.Form.Text += text; }));
        }
        [Obsolete]
        public void ResetWindowTitle()
        {
            //TODO: remake? this seems slow
            //Form.Form.BeginInvoke(new Action(delegate { Form.Form.Text = ""; }));
        }

        /// <summary>
        /// Sets the OutputMerger's targets and depthstencilview to the backbuffer's, and sets the viewport
        /// </summary>
        public void SetBackbuffer()
        {
            Form.SetBackbuffer();
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


                var blendStateDescription = new BlendStateDescription();

                blendStateDescription.RenderTargets[0].BlendEnable = true;
                blendStateDescription.RenderTargets[0].BlendOperation = BlendOperation.Add;
                blendStateDescription.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
                blendStateDescription.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
                blendStateDescription.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
                blendStateDescription.RenderTargets[0].SourceBlendAlpha = BlendOption.SourceAlpha;
                blendStateDescription.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
                blendStateDescription.RenderTargets[0].DestinationBlendAlpha = BlendOption.InverseSourceAlpha;

                AlphaBlend = BlendState.FromDescription(device, blendStateDescription);



                blendStateDescription.RenderTargets[0].BlendEnable = true;
                blendStateDescription.RenderTargets[0].BlendOperation = BlendOperation.Add;
                blendStateDescription.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
                blendStateDescription.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
                blendStateDescription.RenderTargets[0].SourceBlend = BlendOption.One;
                blendStateDescription.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
                blendStateDescription.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
                blendStateDescription.RenderTargets[0].DestinationBlendAlpha = BlendOption.InverseSourceAlpha;

                AlphaBlendPremultiplied = BlendState.FromDescription(device, blendStateDescription);

            }

            /// <summary>
            /// Culling is disabled and fillmode is solid
            /// </summary>
            public RasterizerState RasterizerShowAll { get; set; }

            /// <summary>
            /// Default alpha blending. Dest: InverseSourceAlpha and Source: SourceAlpha
            /// </summary>
            public BlendState AlphaBlend { get; set; }

            /// <summary>
            /// Alpha blending for premultiplied textures. Dest: InverseSourceAlpha and Source: One
            /// </summary>
            public BlendState AlphaBlendPremultiplied { get; set; }

        }
    }
}
