using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.GodGame.Persistence;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Converts the world to and from an xml serializable class structure
    /// </summary>
    public class WorldPersister
    {
        private Func<string, GameVoxelType> typeFactory;
        private Func<string, ItemType> itemFactory;

        public WorldPersister(Func<string, GameVoxelType> typeFactory, Func<string, ItemType> itemFactory)
        {
            this.typeFactory = typeFactory;
            this.itemFactory = itemFactory;
        }

        public void Save(Model.World world, FileInfo file)
        {
            var serializer = createXmlSerializer();
            using (var fs = file.Create())
                serializer.Serialize(fs, SerializedWorld.FromWorld(world));

        }

        public void Load(Model.World world, FileInfo file)
        {
            SerializedWorld sWorld;

            try
            {
                using (var fs = file.OpenRead())
                    sWorld = (SerializedWorld)createXmlSerializer().Deserialize(fs);
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex,"WorldPersister");
                return;
            }


            sWorld.ToWorld(world,typeFactory,itemFactory);
        }

        private static XmlSerializer createXmlSerializer()
        {
            return new XmlSerializer(typeof(SerializedWorld));
        }
        public FileInfo GetDefaultSaveFile()
        {
            return TWDir.GameData.CreateChild("Saves//GodGame").CreateFile("Save.xml");
        }


      
    }
}