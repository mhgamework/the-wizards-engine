using System;
using MHGameWork.TheWizards.Assets;

namespace MHGameWork.TheWizards.Tests.Features.Data.Assets
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
