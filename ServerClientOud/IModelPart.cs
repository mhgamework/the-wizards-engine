using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface IModelPart
    {
        void Render();
        void RenderPrimitives();

        void UpdateWorldMatrix( Microsoft.Xna.Framework.Matrix newWorldMatrix );

        void SaveToXML( TWXmlNode node );


        void LoadFromXML( TWXmlNode node );

    }
}
