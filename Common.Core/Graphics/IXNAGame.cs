using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics
{
    public interface IXNAGame
    {
        GraphicsDevice GraphicsDevice
        { get;}

        float Elapsed { get;}
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
        void AddXNAObject( IXNAObject obj );
        bool IsCursorInWindow();

        void AddBasicShader(BasicShader basicShader);
    }
}
