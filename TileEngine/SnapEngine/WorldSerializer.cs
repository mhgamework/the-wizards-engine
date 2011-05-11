using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.TileEngine.SnapEngine
{
    public class WorldSerializer
    {
        private readonly IMeshFactory meshFactory;
        private readonly TileDataFactory tileDataFactory;
        private readonly IXNAGame game;
        private readonly SimpleMeshRenderer renderer;
        private readonly IWorldObjectTypeFactory worldObjectTypeFactory;
        private readonly TileSnapInformationBuilder builder;

        public WorldSerializer(IMeshFactory meshFactory, TileDataFactory tileDataFactory, IXNAGame game, SimpleMeshRenderer renderer, IWorldObjectTypeFactory worldObjectTypeFactory, TileSnapInformationBuilder builder)
        {
            this.meshFactory = meshFactory;
            this.tileDataFactory = tileDataFactory;
            this.game = game;
            this.renderer = renderer;
            this.worldObjectTypeFactory = worldObjectTypeFactory;
            this.builder = builder;
        }

        public void SerializeWorldObject(WorldObject obj, Stream stream)
        {
            var node = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "WorldObject");

            var position = node.CreateChildNode("Position");
            XMLSerializer.WriteVector3(position, obj.Position);

            var rotation = node.CreateChildNode("Rotation");
            XMLSerializer.WriteQuaternion(rotation, obj.Rotation);

            var type = node.CreateChildNode("Type");
            XMLSerializer.WriteGuid(type, obj.ObjectType.guid);

            node.XmlDocument.Save(stream);
        }

        public WorldObject DeserializeWorldObject(Stream stream)
        {
            var node = TWXmlNode.GetRootNodeFromStream(stream);
            var obj = new WorldObject(game, worldObjectTypeFactory.GetWorldObjectType(XMLSerializer.ReadGuid(node.FindChildNode("Type"))), renderer);

            obj.Position = XMLSerializer.ReadVector3(node.FindChildNode("Position"));
            obj.Rotation = XMLSerializer.ReadQuaternion(node.FindChildNode("Rotation"));

            return obj;
        }

        public void SerializeWorldObjectType(WorldObjectType type, Stream stream)
        {
            var node = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "WorldObjectType");

            var guid = node.CreateChildNode("Guid");
            XMLSerializer.WriteGuid(guid, type.guid);

            var mesh = node.CreateChildNode("Mesh");
            XMLSerializer.WriteGuid(mesh, type.Mesh.Guid);

            var tileData = node.CreateChildNode("TileData");
            XMLSerializer.WriteGuid(tileData, type.TileData.Guid);

            node.XmlDocument.Save(stream);
        }

        public WorldObjectType DeserializeWorldObjectType(Stream stream)
        {
            var node = TWXmlNode.GetRootNodeFromStream(stream);
            var type = new WorldObjectType(meshFactory.GetMesh(XMLSerializer.ReadGuid(node.FindChildNode("Mesh"))),
                                          XMLSerializer.ReadGuid(node.FindChildNode("Guide")),builder);

            type.TileData = tileDataFactory.GetTileData(XMLSerializer.ReadGuid(node.FindChildNode("TileData")));
           
            return type;
        }
    }
}
