using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    public class LevelSerializer
    {
        public void Serialize(Level level, FileInfo file)
        {
            var xml = buildXmlLevel(level);
            var serializer = new XmlSerializer(typeof(XmlLevel));

            using (var fs = file.Create())
                serializer.Serialize(fs, xml);

        }


        public void Deserialize(Level level, FileInfo file)
        {
            XmlLevel xml;
            var serializer = new XmlSerializer(typeof(XmlLevel));

            try
            {
                using (var fs = file.OpenRead())
                    xml = (XmlLevel)serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw new FileFormatException("Level file is not in valid format or not readable", ex);
            }


            /*try
            {*/
            restoreXmlLevel(level, xml);
            /*}
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Can not deserialize the loaded level xml. Xml is valid but could not be converted into a runtime level.",ex);
            }*/


        }

        private void restoreXmlLevel(Level level, XmlLevel xmlLevel)
        {
            var objects = new List<object>();

            xmlLevel.Islands.ForEach(xmlI =>
                {
                    var i = level.CreateNewIsland(xmlI.Position);
                    //var constructionMethod = level.GetType().GetMethod(xmlI.ConstructionConstructor);
                    //i.Construction = (Construction)constructionMethod.Invoke(level, new[] { i });
                    xmlI.Island = i;

                    i.Type = (Island.IslandType)Enum.Parse(typeof(Island.IslandType), xmlI.Type);

                    objects.Add(i);
                });

            xmlLevel.Islands.ForEach(xmlI =>
                xmlI.ConnectedIslands.Select(i => objects[i]).Cast<Island>().ForEach(xmlN =>
                    xmlI.Island.AddBridgeTo(xmlN)));

        }

        private XmlLevel buildXmlLevel(Level level)
        {
            int nextID = 1;

            var xmlLevel = new XmlLevel();

            xmlLevel.Islands = level.Islands.Select(island => new XmlIsland()
                {
                    Island = island,
                    ID = nextID++,
                    Position = island.Position,
                    //ConstructionConstructor = island.Construction.LevelConstructorMethod,
                    Type = island.Type.ToString()
                }).ToArray();

            xmlLevel.Islands.ForEach(i =>
                                     i.ConnectedIslands = i.Island.ConnectedIslands.Select(
                                         connectedI => xmlLevel.Islands.TakeWhile(j => j.Island != connectedI).Count())
                                                           .ToArray());

            return xmlLevel;
        }


        public class XmlLevel
        {
            public XmlIsland[] Islands;
        }

        public class XmlIsland
        {
            [XmlIgnore]
            public Island Island;
            public int ID;
            public Vector3 Position;
            public int[] ConnectedIslands;
            public String ConstructionConstructor;
            public string Type;

        }
    }


}