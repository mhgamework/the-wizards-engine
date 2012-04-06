using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Basic/temporary implementation for loading assets
    /// </summary>
    public class SimpleAssetFactory : IAssetFactory
    {
        public object GetAsset(Type type, Guid guid)
        {
            // Problem with the type parameter! a guid is global anyway, why specify by type?
            //if (type == typeof(IMesh))
                return TW.Model.GetSingleton<RenderingModel>().MeshFactory.GetMesh(guid);

            //return null;
        }
    }
}
