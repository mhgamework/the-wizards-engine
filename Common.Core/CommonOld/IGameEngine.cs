using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common
{
    public interface IGameEngine
    {
        Engine.IGameFileManager GameFileManager
        {
            get;
        }
    }
}
