using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface IGameMode : IGameObjectOud
    {
        void StartGameMode();
        void StopGameMode();

        event EventHandler Completed;
    }
}
