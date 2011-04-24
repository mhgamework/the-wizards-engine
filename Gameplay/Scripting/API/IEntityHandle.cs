using System;
using MHGameWork.TheWizards.Gameplay;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Scripting.API
{
    public interface IEntityHandle
    {
        Vector3 Position { get; set; }

        void RegisterUpdateHandler();
        void RegisterUseHandler(Action<IPlayer> handler);
    }
}
