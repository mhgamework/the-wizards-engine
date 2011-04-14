using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Assets
{
    public interface IAssetFactory
    {
        object GetAsset(Type type, Guid guid);
    }
}
