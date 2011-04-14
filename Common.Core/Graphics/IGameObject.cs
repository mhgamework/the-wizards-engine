using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    /// <summary>
    /// DEPRECATED: should be determined if this is of use or not.
    /// Maybe this doesn't have a purpose for the final game, but for testing it might come in handy.
    /// </summary>
    [Obsolete( "Do not use this anymore, use MHGameWork.TheWizards.Graphics.IXNAObject" )]
    public interface IGameObject
    {
        void Render();
        void Process();
    }
}
