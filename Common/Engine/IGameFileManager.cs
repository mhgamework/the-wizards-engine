using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common.Engine
{
    public interface IGameFileManager
    {
        IGameFile FindIGameFile(int id);
        
    }
}
