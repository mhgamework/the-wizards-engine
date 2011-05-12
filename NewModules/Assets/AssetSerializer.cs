using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Xml;

namespace MHGameWork.TheWizards.Assets
{
    public class AssetSerializer : ICustomSerializer
    {
        private readonly IAssetFactory factory;

        private AssetSerializer()
        {

        }
        public AssetSerializer(IAssetFactory factory)
        {
            this.factory = factory;
        }

        public bool SerializeElement(TWXmlNode node, Type type, object value, IInternalSerializer s)
        {
            if (factory != null)
                throw new InvalidOperationException("This is a deserializer");
            if (!(value is IAsset))
                return false;

            var asset = value as IAsset;

            XMLSerializer.WriteGuid(node, asset.Guid);


            return true;
        }

        public bool DeserializeElement(TWXmlNode node, Type type, out object value, IInternalSerializer s)
        {
            if (factory == null)
                throw new InvalidOperationException("This is a serializer");

            value = null;



            if (!typeof(IAsset).IsAssignableFrom(type)) return false;

            Guid g = XMLSerializer.ReadGuid(node);
            value = factory.GetAsset(type, g);

            if (value == null) return false;

            return true;

        }

        public static AssetSerializer CreateSerializer()
        {
            return new AssetSerializer();
        }

        public static AssetSerializer CreateDeserializer(IAssetFactory factory)
        {
            return new AssetSerializer(factory);
        }
    }
}
