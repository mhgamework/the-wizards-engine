using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface ICursor
    {
        Vector2 Position { get;set;}
        void Load();
        void Update();
        void Render();
    }
}
