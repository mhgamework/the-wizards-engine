using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.World
{
    public class World
    {
        private List<IWorldEntity> entities = new List<IWorldEntity>();

        /// <summary>
        /// This method could be overrided with custom factories.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Create<T>() where T : class, IWorldEntity, new()
        {
            var ent = new T();

            entities.Add(ent);
            return ent;
        }


        public void Save(DirectoryInfo dir)
        {
            TWXmlNode root = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "World");
            root.AddChildNode("SerializerVersion", "0.1");
            var entitiesNode = root.CreateChildNode("Entities");
            entitiesNode.AddAttributeInt("Count", entities.Count);

            dir.Create();
            for (int i = 0; i < entities.Count; i++)
            {
                var ent = entities[i];


                var relativePath = "Entity" + i.ToString("0000") + ".xml";

                var fi = new FileInfo(dir.FullName + "\\" + relativePath);

                saveEntity(ent, fi);

                var entityNode = entitiesNode.CreateChildNode("Entity");
                entityNode.AddAttribute("Type", ent.GetType().AssemblyQualifiedName);
                entityNode.AddAttribute("RelativePath", relativePath);


            }
            using (var fs = new FileStream(dir.FullName + "\\World.xml", FileMode.Create))
            {
                root.Document.Save(fs);
            }

        }

        private void saveEntity(IWorldEntity ent, FileInfo file)
        {
            using (FileStream fs = new FileStream(file.FullName, FileMode.Create))
            {
                var serializer = new XmlSerializer(ent.GetType());
                serializer.Serialize(fs, ent);
            }

        }
        private IWorldEntity loadEntity(FileInfo file, Type entityType)
        {
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
            {
                var serializer = new XmlSerializer(entityType);
                return serializer.Deserialize(fs) as IWorldEntity;
            }

        }


        public void Load(DirectoryInfo dir)
        {
            TWXmlNode root = TWXmlNode.GetRootNodeFromFile(dir.FullName + "\\World.xml");

            var entitiesNode = root.FindChildNode("Entities");
            var entityNodes = entitiesNode.GetChildNodes();

            for (int i = 0; i < entityNodes.Length; i++)
            {
                var entityNode = entityNodes[i];
                var type = entityNode.GetAttribute("Type");
                var relativePath = entityNode.GetAttribute("RelativePath");

                var serializer = new XmlSerializer(Type.GetType(type));

                var fi = new FileInfo(dir.FullName + "\\" + relativePath);

                var ent = loadEntity(fi, Type.GetType(type));

                entities.Add(ent);



            }
            using (var fs = new FileStream(dir.FullName + "\\World.xml", FileMode.OpenOrCreate))
            {
                root.Document.Save(fs);
            }
        }



    }
}
