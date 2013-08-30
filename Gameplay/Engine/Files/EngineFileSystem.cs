using System;
using System.IO;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Engine.Files
{
    /// <summary>
    /// Responsible for implementing a file system for use in the engine.
    /// Takes care of permissions etc. 
    /// </summary>
    public class EngineFileSystem : IEngineFilesystem
    {
        private readonly string rootFolder;

        public EngineFileSystem(string rootFolder)
        {
            this.rootFolder = rootFolder;
        }

        public void StoreXml(object data, string filename)
        {
            var s = new XmlSerializer(data.GetType());
            var file = rootFolder + "\\" + filename + ".xml";
            using (var fs = openWrite(file))
            {
                s.Serialize(fs, data);
            }
        }

        public T LoadXml<T>(string filename)
        {
            var s = new XmlSerializer(typeof(T));
            var file = rootFolder + "\\" + filename + ".xml";
            
            if (!File.Exists(file)) return default(T);

            using (var fs = openRead(file))
            {
                return (T)s.Deserialize(fs);
            }
        }

        private FileStream openWrite(string s)
        {
            return File.Create(s);
        }
        private FileStream openRead(string s)
        {
            var fi = new FileInfo(s);
            fi.Directory.Create();
            
            return File.OpenRead(s);
        }
    }
}