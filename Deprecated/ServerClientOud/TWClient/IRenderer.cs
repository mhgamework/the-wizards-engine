using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public interface IRenderer
    {

        void Render( XNAGame game );

        void Update( XNAGame game );

        void Initialize( XNAGame game );

    }
}
