using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using System;
namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class polygons_p : IXmlSerializable
    {
        public int[] indices;

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            string s = reader.ReadString();
            s = s.Replace('\n', ' ');

            // Invalid?
            if (string.IsNullOrEmpty(s))
            {
                indices = null;
                return;
            }

            string[] splitted = s.Split(new char[] { ' ' });
            List<int> list = new List<int>(splitted.Length);
            for ( int i = 0; i < splitted.Length; i++ )
            {
                string current = splitted[i].Trim();
                if (!String.IsNullOrEmpty(current))
                {
                    try
                    {
                        list.Add(int.Parse(current));
                    } // try
                    catch
                    {
                    } // ignore
                }
            }

            indices = list.ToArray();

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}