using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;

namespace MHGameWork.TheWizards.Tests.Assets
{
    public class SimpleAssetFactory : IAssetFactory
    {
        public object GetAsset(Type type, Guid guid)
        {
            if (!(type == typeof(SimpleAsset))) throw new InvalidOperationException();

            return new SimpleAsset {Guid = guid};

        }
    }
}
