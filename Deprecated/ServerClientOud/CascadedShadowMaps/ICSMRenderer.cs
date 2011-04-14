using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.ServerClient.CascadedShadowMaps
{
    public interface ICSMRenderer
    {

        void Initialize( XNAGame game );

        void RenderDepth( XNAGame game, BasicShader depthShader );


        void RenderNormal( XNAGame game, BasicShader normalShader );
        void Update( XNAGame game );

        void RenderShadowMap( XNAGame game, BasicShader shadowMapShader );

    }
}
