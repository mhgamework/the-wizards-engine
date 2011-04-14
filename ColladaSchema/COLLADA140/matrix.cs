using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class matrix : IXmlSerializable
    {
        [XmlAttribute]
        public string sid;

        public Microsoft.Xna.Framework.Matrix matrixValue;



        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            sid = reader.GetAttribute("sid");

            string s = reader.ReadString();
            s = s.Replace('\n', ' ');

            float[] floats = float_array.StringToFloatArray(s);

            // Invalid?
            matrixValue = FloatArrayToMatrix(floats, 0);

            reader.ReadEndElement();

         
        }

        public static Microsoft.Xna.Framework.Matrix FloatArrayToMatrix(float[] mat, int offset)
        {
            return new Microsoft.Xna.Framework.Matrix(
                mat[offset + 0], mat[offset + 4], mat[offset + 8], mat[offset + 12],
                mat[offset + 1], mat[offset + 5], mat[offset + 9], mat[offset + 13],
                mat[offset + 2], mat[offset + 6], mat[offset + 10], mat[offset + 14],
                mat[offset + 3], mat[offset + 7], mat[offset + 11], mat[offset + 15]);
        } 

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}