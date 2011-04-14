using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class float_array : IXmlSerializable
    {
        [XmlAttribute]
        public int count;
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;


        public float[] values;

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            count = int.Parse(reader.GetAttribute("count"));

            id = reader.GetAttribute("id");
            if (id == null)
                id = "";

            string s = reader.ReadString();
            s = s.Replace('\n', ' ');

            // Invalid?
            values = StringToFloatArray(s);

            reader.ReadEndElement();
        }

        public static float[] StringToFloatArray(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            string[] splitted = s.Split(new char[] { ' ' });
            List<float> list = new List<float>(splitted.Length);
            for (int i = 0; i < splitted.Length; i++)
            {
                string current = splitted[i].Trim();
                if (!String.IsNullOrEmpty(current))
                {
                    try
                    {
                        list.Add(Convert.ToSingle(current, CultureInfo.InvariantCulture));
                    } // try
                    catch
                    {
                    } // ignore
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Not tested!
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
            StringBuilder builder = new StringBuilder(values.Length * 6);
            for (int i = 0; i < values.Length; i++)
            {
                builder.Append(values[i].ToString("#.00000"));
            }

            writer.WriteValue(builder.ToString());
        }

        #endregion
    }
}