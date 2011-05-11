using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldSerializer
    {
        private readonly IMeshFactory meshFactory;
        private readonly TileDataFactory tileDataFactory;
        private readonly IXNAGame game;
        private readonly SimpleMeshRenderer renderer;
        private readonly SimpleWorldObjectTypeFactory worldObjectTypeFactory;

        public WorldSerializer(IMeshFactory meshFactory, TileDataFactory tileDataFactory, IXNAGame game, SimpleMeshRenderer renderer, SimpleWorldObjectTypeFactory worldObjectTypeFactory)
        {
            this.meshFactory = meshFactory;
            this.tileDataFactory = tileDataFactory;
            this.game = game;
            this.renderer = renderer;
            this.worldObjectTypeFactory = worldObjectTypeFactory;
        }

        public void SerializeWorld(World world, Stream stream)
        {
            var rootNode = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "World");

            var typesList = new List<WorldObjectType>();

            var worldObjectsNode = rootNode.CreateChildNode("WorldObjects");

            for (int i = 0; i < world.WorldObjectList.Count; i++)
            {
                var wObj = world.WorldObjectList[i];
                if (!typesList.Contains(wObj.ObjectType))
                    typesList.Add(wObj.ObjectType);

                SerializeWorldObject(wObj, worldObjectsNode.CreateChildNode("WorldObject"));
            }

            var typesNode = rootNode.CreateChildNode("WorldObjectTypes");

            for (int i = 0; i < typesList.Count; i++)
            {
                var type = typesList[i];

                SerializeWorldObjectType(type, typesNode.CreateChildNode("Type"));
            }


            rootNode.Document.Save(stream);
        }
        public void DeserializeWorld(World world, Stream stream)
        {
            var rootNode = TWXmlNode.GetRootNodeFromStream(stream);


            var typesNodes = rootNode.FindChildNode("WorldObjectTypes").GetChildNodes();

            for (int i = 0; i < typesNodes.Length; i++)
            {
                var nType= typesNodes[i];
                worldObjectTypeFactory.AddWorldObjectType(DeserializeWorldObjectType(nType));
            }

            var objectNodes = rootNode.FindChildNode("WorldObjects").GetChildNodes();

            for (int i = 0; i < objectNodes.Length; i++)
            {
                var nObj = objectNodes[i];

                // Auto-adds it to the world
                DeserializeWorldObject(world, nObj);

            }
            
        }



        public void SerializeWorldObject(WorldObject obj, TWXmlNode node)
        {
            var position = node.CreateChildNode("Position");
            XMLSerializer.WriteVector3(position, obj.Position);

            var rotation = node.CreateChildNode("Rotation");
            XMLSerializer.WriteQuaternion(rotation, obj.Rotation);

            var type = node.CreateChildNode("Type");
            XMLSerializer.WriteGuid(type, obj.ObjectType.Guid);

        }

        public WorldObject DeserializeWorldObject(World world, TWXmlNode node)
        {
            var obj = world.CreateNewWorldObject(game, worldObjectTypeFactory.GetWorldObjectType(XMLSerializer.ReadGuid(node.FindChildNode("Type"))), renderer);

            obj.Position = XMLSerializer.ReadVector3(node.FindChildNode("Position"));
            obj.Rotation = XMLSerializer.ReadQuaternion(node.FindChildNode("Rotation"));

            return obj;
        }

        public void SerializeWorldObjectType(WorldObjectType type, TWXmlNode node)
        {
            var guid = node.CreateChildNode("Guid");
            XMLSerializer.WriteGuid(guid, type.Guid);

            var mesh = node.CreateChildNode("Mesh");
            XMLSerializer.WriteGuid(mesh, type.Mesh.Guid);

            var tileData = node.CreateChildNode("TileData");
            XMLSerializer.WriteGuid(tileData, type.TileData.Guid);

        }

        public WorldObjectType DeserializeWorldObjectType(TWXmlNode node)
        {
            var type = new WorldObjectType(meshFactory.GetMesh(XMLSerializer.ReadGuid(node.FindChildNode("Mesh"))),
                                          XMLSerializer.ReadGuid(node.FindChildNode("Guid")));

            type.TileData = tileDataFactory.GetTileData(XMLSerializer.ReadGuid(node.FindChildNode("TileData")));
            if (type.TileData == null) throw new InvalidOperationException("TileData For WorldObjectType not found!!");
            return type;
        }
    }
}
