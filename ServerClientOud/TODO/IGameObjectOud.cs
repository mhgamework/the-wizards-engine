using System;
namespace MHGameWork.TheWizards.ServerClient
{
    public interface IGameObjectOud
    {
        void Initialize();
        void LoadGraphicsContent();
        void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e );
        void Render();
        void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e );
    }
}
