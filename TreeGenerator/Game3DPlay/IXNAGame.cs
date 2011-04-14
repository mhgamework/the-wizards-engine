using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface IXNAGame
    {
        GraphicsDevice GraphicsDevice
        { get;}

        XNAGameFiles EngineFiles { get;}
        ICamera Camera { get;}
        TWMouse Mouse { get;}
        TWKeyboard Keyboard { get;}
        LineManager3D LineManager3D { get;}

        void SetCamera( ICamera cam );

        // Size of the window being rendered to.
        Point ClientSize { get;}

        bool IsActive { get;}

        Ray GetWereldViewRay( Vector2 cursorPos );
    }
}
