using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.GodGame.Persistence;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame.Types.Towns.Data;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Converts the world to and from an xml serializable class structure
    /// Also saves the generic data store (currently a prototype concept)
    /// </summary>
    public class WorldPersisterService
    {
        private readonly GameplayObjectsSerializer gameplayObjectsSerializer;
        private readonly GenericDatastore genericDatastore;

        public WorldPersisterService(GameplayObjectsSerializer gameplayObjectsSerializer, GenericDatastore genericDatastore)
        {
            this.gameplayObjectsSerializer = gameplayObjectsSerializer;
            this.genericDatastore = genericDatastore;
        }

        public void Save(Model.World world, FileInfo file)
        {
            var serializer = createXmlSerializer();
            using (var fs = file.Create())
                serializer.Serialize(fs, SerializedWorld.FromWorld(world));

            var doc = new XDocument(genericDatastore.Serialize());
            doc.Save(Path.ChangeExtension(file.FullName, "data.xml"));


        }

        public void Load(Model.World world, FileInfo file)
        {
            SerializedWorld sWorld;

            try
            {
                using (var fs = file.OpenRead())
                    sWorld = (SerializedWorld)createXmlSerializer().Deserialize(fs);

                try
                {
                    var doc = XDocument.Load(Path.ChangeExtension(file.FullName, "data.xml"));
                    genericDatastore.Deserialize(doc.Elements().First());
                }
                catch (Exception ex2)
                {
                    DI.Get<IErrorLogger>().Log(ex2, "Genericdatastore");

                    
                }
        

            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "WorldPersister");
                return;
            }


            sWorld.ToWorld(world, gameplayObjectsSerializer);




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