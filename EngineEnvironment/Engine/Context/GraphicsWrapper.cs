using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Input;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for provides access to the Graphics subsystem.
    /// Currently also input, and not really a wrapper :P
    /// </summary>
    public class GraphicsWrapper : DX11Game
    {
        private DeferredRenderer renderer;

        public DeferredRenderer AcquireRenderer()
        {
            if (renderer == null)
                renderer = new DeferredRenderer(TW.Graphics);

            return renderer;
        }

        private NullKeyboard nullKeyboard = new NullKeyboard();

        public override TWKeyboard Keyboard
        {
            get
            {
                if (TW.Internal.ActiveSimulator == null || exclusiveAccess == null) return base.Keyboard;
                if (TW.Internal.ActiveSimulator == exclusiveAccess) return base.Keyboard;

                return nullKeyboard;
            }
        }


        private ISimulator exclusiveAccess = null;
        public void RequestExclusiveKeyboardAccess()
        {
            if (exclusiveAccess != null) throw new InvalidOperationException("Another sim has exclusive keyboard access: " + exclusiveAccess);
            exclusiveAccess = TW.Internal.ActiveSimulator;
            SpectaterCamera.EnableUserInput = false;
        }
        public void ReleaseExclusiveKeyboardAccess()
        {
            if (exclusiveAccess != TW.Internal.ActiveSimulator) throw new InvalidOperationException("Does not have exclusive access");
            exclusiveAccess = null;
            SpectaterCamera.EnableUserInput = true;
        }


        private class NullKeyboard : TWKeyboard
        {
            public bool IsKeyPressed(Key key)
            {
                return false;
            }

            public bool IsKeyHeld(Key key)
            {
                return false;
            }

            public bool IsKeyReleased(Key key)
            {
                return false;
            }

            public bool IsKeyDown(Key key)
            {
                return false;
            }

            private IList<Key> emptyList = new List<Key>();
            public IList<Key> ReleasedKeys { get { return emptyList; } }
            public IList<Key> PressedKeys { get { return emptyList; } }
            public IList<Key> AllKeys { get { throw new NotImplementedException(); } }
        }
    }
}
