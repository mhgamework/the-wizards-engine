using System;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Serialization;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// Responsible for string-serializing Assets, using GUIDs
    /// </summary>
    public class AssetSerializer : IConditionalSerializer
    {
        private IAssetFactory assetFactory;

        public AssetSerializer(IAssetFactory assetFactory)
        {
            this.assetFactory = assetFactory;
        }

        public bool CanOperate(Type type)
        {
            return typeof(IAsset).IsAssignableFrom(type);
        }

        public string Serialize(object obj, Type type, StringSerializer stringSerializer)
        {
            var asset = (IAsset)obj;
            return stringSerializer.Serialize(asset.Guid);
        }

        public object Deserialize(string value, Type type, StringSerializer stringSerializer)
        {
            var guid = (Guid)stringSerializer.Deserialize(value, typeof(Guid));
            return assetFactory.GetAsset(null, guid);
        }
    }
}