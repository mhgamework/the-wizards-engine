using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics.DEPRECATED
{
    public interface ICursor
    {
        Vector2 Position { get;set;}
        void Load();
        void Update();
        void Render();
    }
}
