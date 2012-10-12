using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// This is the configuration file for the Serverclient startup project
    /// </summary>
    public class TWConfig
    {
        public bool TestMode;
        public string TestClass;
        public string TestMethod;

        public TWConfig()
        {
            TestMode = true;
            TestClass = "MHGameWork.TheWizards.Tests.CoreTest";
            TestMethod = "TestQuadtreeVisualizer";
        }


        public static string Filename { get { return "TWConfig.xml"; } }
        public static TWConfig Load()
        {
            if (!File.Exists(Filename))
                return new TWConfig();

            var serializer = new XmlSerializer(typeof(TWConfig));
            using (var fs = File.Create(Filename))
                return (TWConfig)serializer.Deserialize(fs);

        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof(TWConfig));
            using (var fs = File.Create(Filename))
                serializer.Serialize(fs, this);
        }
    }

}
