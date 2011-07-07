using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11.Graphics;
using DirectX11.Input;
using SlimDX.DirectInput;

namespace DirectX11
{
    /// <summary>
    /// This is actually a helper class used in testing. It can be used across all classes to simplify development.
    /// However, this class might be removed later, or this class could be a front-end class (facade).
    /// Currently, this is implemented as a (partial) facade for DX11 (partial because it exposes the Device)
    /// </summary>
    public class DX11Game
    {
        public DX11Game()
        {
            form = new DX11Form();
            form.GameLoopEvent += new Action(form_GameLoopEvent);
        }

        void form_GameLoopEvent()
        {
            updateElapsed();

            updateInput();

            if (keyboard.IsKeyDown(Key.Escape)) Exit();

            SpecaterCamera.Update(Elapsed);
            LineManager3D.Render(Camera);

        }

        private void updateInput()
        {
            mouse.UpdateMouseState(diMouse.GetCurrentState());
            keyboard.UpdateKeyboardState(diKeyboard.GetCurrentState());
        }

        private void updateElapsed()
        {
            var nextFrameTime = SlimDX.Configuration.Timer.Elapsed;
            Elapsed = (float)(nextFrameTime - lastFrameTime).TotalSeconds;
            lastFrameTime = nextFrameTime;
        }

        private TimeSpan lastFrameTime;
        private TWKeyboard keyboard;
        private TWMouse mouse;
        private DX11Form form;
        private Keyboard diKeyboard;
        private Mouse diMouse;
        private DirectInput diDevice;
        public float Elapsed { get; private set; }
        public void Run()
        {

            keyboard = new TWKeyboard();
            diDevice = new SlimDX.DirectInput.DirectInput();
            diKeyboard = new SlimDX.DirectInput.Keyboard(diDevice);
            diKeyboard.Acquire();

            mouse = new TWMouse();
            diMouse = new SlimDX.DirectInput.Mouse(diDevice);
            diMouse.Acquire();

            SpecaterCamera = new SpectaterCamera(keyboard, mouse);
            Camera = SpecaterCamera;



            form.Run();

        }
        public void Exit()
        {
            form.Exit();

            diKeyboard.Dispose();
            diMouse.Dispose();
            diDevice.Dispose();
            

        }
        public ICamera Camera { get; set; }



        // Helper

        public SpectaterCamera SpecaterCamera { get; private set; }
        public LineManager3D LineManager3D { get; private set; }

        public TWKeyboard Keyboard
        {
            get { return keyboard; }
        }

        public TWMouse Mouse
        {
            get { return mouse; }
        }
    }
}
