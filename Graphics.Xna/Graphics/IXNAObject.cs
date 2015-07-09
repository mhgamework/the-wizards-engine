using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Graphics
{
    /// <summary>
    /// Since objects containing initialize,render and update are scattered all over the wizards,
    /// I figured that they are XNA driven objects, and that this should be defined in code
    /// TODO: not sure whether the 'IXNAGame game' parameter is a good idea
    /// Edit: the paramater game is a good idea, since this more accurately defines the interface. With the parameter
    ///        the interface defines functions to 'Render inside an XNAGame' and 'Update inside an XNAGame'
    /// Note: sometimes we do not need 'Update inside an XNAGame', but just 'Update inside a game loop'. 
    ///        An interface should be designed for this purpose (applies to PhysX, running on Client (XNA) and Server (ordinary game loop)
    /// </summary>
    public interface IXNAObject
    {
        void Initialize( IXNAGame _game );
        void Render( IXNAGame _game );
        void Update( IXNAGame _game );
    }
}
