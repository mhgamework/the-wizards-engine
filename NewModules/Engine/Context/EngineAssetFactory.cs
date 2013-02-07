using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Basic/temporary implementation for loading assets
    /// </summary>
    public class EngineAssetFactory : IAssetFactory
    {
        public object GetAsset(Type type, Guid guid)
        {
            return TW.Assets.GetMesh(guid);
        }
    }
}
