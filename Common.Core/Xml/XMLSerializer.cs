using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.XML
{
    public static class XMLSerializer
    {
        private static StringBuilder builder;

        static XMLSerializer()
        {
            builder = new StringBuilder();

        }
        public static void WriteFloatArray(TWXmlNode node, float[] data)
        {
            if (data == null) { node.Value = "NULL"; return; }
            builder.Clear();
            if (data.Length != 0)
            {
                builder.Append(data[0]);
                for (int i = 1; i < data.Length; i++)
                {
                    builder.Append(" ");

                    builder.Append(data[i].ToString());
                }
            }

            node.Value = builder.ToString();

        }

        public static float[] ReadFloatArray(TWXmlNode node)
        {
            if (node.Value == "NULL") return null;

            string val = node.Value;
            string[] floatStrings = val.Split(new char[] { ' ' });
            float[] floats = new float[floatStrings.Length];

            for (int i = 0; i < floatStrings.Length; i++)
            {
                floats[i] = Single.Parse(floatStrings[i]);
            }

            return floats;

        }

        public static void WriteIntArray(TWXmlNode node, int[] data)
        {
            if (data == null) { node.Value = "NULL"; return; }
            builder.Clear();
            if (data.Length != 0)
            {
                builder.Append(data[0]);
                for (int i = 1; i < data.Length; i++)
                {
                    builder.Append(" ");
                    builder.Append(data[i]);
                }
            }

            node.Value = builder.ToString();

        }

        public static int[] ReadIntArray(TWXmlNode node)
        {
            if (node.Value == "NULL") return null;

            string val = node.Value;
            string[] floatStrings = val.Split(new char[] { ' ' });
            int[] floats = new int[floatStrings.Length];

            for (int i = 0; i < floatStrings.Length; i++)
            {
                floats[i] = Int32.Parse(floatStrings[i]);
            }

            return floats;

        }

        public static void WriteGuid(TWXmlNode node, Guid guid)
        {
            var gNode = node.CreateChildNode("Guid");
            gNode.AddAttribute("Value", guid.ToString());
        }

        public static Guid ReadGuid(TWXmlNode node)
        {
            var gNode = node.FindChildNode("Guid");
            return new Guid(gNode.GetAttribute("Value"));
        }

        public static void WriteBoolean(TWXmlNode node, bool boolean)
        {
            node.Value = boolean.ToString();
        }

        public static bool ReadBoolean(TWXmlNode node)
        {
            return Boolean.Parse(node.Value);
        }

        public static void WriteMatrix(TWXmlNode node, Matrix mat)
        {
            TWXmlNode rowMatrixNode = node.CreateChildNode("MatrixRows");
            WriteFloatArray(rowMatrixNode, new float[] { 
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44
            });
        }

        public static Matrix ReadMatrix(TWXmlNode node)
        {
            TWXmlNode rowMatrixNode = node.FindChildNode("MatrixRows");
            float[] floats = ReadFloatArray(rowMatrixNode);
            Matrix mat = new Matrix(
                floats[0], floats[1], floats[2], floats[3],
                floats[4], floats[5], floats[6], floats[7],
                floats[8], floats[9], floats[10], floats[11],
                floats[12], floats[13], floats[14], floats[15]);

            return mat;

        }

        public static void WriteVector3Array(TWXmlNode node, Vector3[] data)
        {
            if (data == null) { node.Value = "NULL"; return; }
            node.AddAttributeInt("Count", data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                TWXmlNode nodeVector = node.CreateChildNode("Vector3");
                nodeVector.AddAttribute("X", data[i].X.ToString());
                nodeVector.AddAttribute("Y", data[i].Y.ToString());
                nodeVector.AddAttribute("Z", data[i].Z.ToString());
            }

        }

        public static Vector3[] ReadVector3Array(TWXmlNode node)
        {
            if (node.Value == "NULL") return null;
            int count = node.GetAttributeInt("Count");

            Vector3[] array = new Vector3[count];

            TWXmlNode[] vectorNodes = node.GetChildNodes();
            int i = 0;
            foreach (TWXmlNode nodeVector in vectorNodes)
            {
                Vector3 v = new Vector3();
                v.X = Single.Parse(nodeVector.GetAttribute("X"));
                v.Y = Single.Parse(nodeVector.GetAttribute("Y"));
                v.Z = Single.Parse(nodeVector.GetAttribute("Z"));

                array[i] = v;
                i++;
            }

            return array;

        }

        public static void WriteVector3(TWXmlNode node, Vector3 v)
        {
            node.AddChildNode("X", v.X.ToString());
            node.AddChildNode("Y", v.Y.ToString());
            node.AddChildNode("Z", v.Z.ToString());
        }

        public static Vector3 ReadVector3(TWXmlNode node)
        {
            Vector3 v = new Vector3();

            v.X = Single.Parse(node.ReadChildNodeValue("X"));
            v.Y = Single.Parse(node.ReadChildNodeValue("Y"));
            v.Z = Single.Parse(node.ReadChildNodeValue("Z"));

            return v;

        }

        public static void WriteVector2(TWXmlNode node, Vector2 v)
        {
            node.AddChildNode("X", v.X.ToString());
            node.AddChildNode("Y", v.Y.ToString());
        }

        public static Vector2 ReadVector2(TWXmlNode node)
        {
            Vector2 v = new Vector2();

            v.X = Single.Parse(node.ReadChildNodeValue("X"));
            v.Y = Single.Parse(node.ReadChildNodeValue("Y"));

            return v;

        }

        public static void WriteColor(TWXmlNode node, Color c)
        {
            node = node.CreateChildNode("Color");
            node.AddAttributeInt("A", c.A);
            node.AddAttributeInt("R", c.R);
            node.AddAttributeInt("G", c.G);
            node.AddAttributeInt("B", c.B);

        }

        public static Color ReadColor(TWXmlNode node)
        {
            node = node.FindChildNode("Color");
            Color c = new Color((byte)node.GetAttributeInt("R"), (byte)node.GetAttributeInt("G"), (byte)node.GetAttributeInt("B")
                , (byte)node.GetAttributeInt("A"));


            return c;

        }

        public static void WriteBoundingSphere(TWXmlNode node, BoundingSphere sphere)
        {
            WriteVector3(node.CreateChildNode("Center"), sphere.Center);
            node.AddChildNode("Radius", sphere.Radius.ToString());
        }

        public static BoundingSphere ReadBoundingSphere(TWXmlNode node)
        {
            Vector3 center = ReadVector3(node.FindChildNode("Center"));
            float radius = Single.Parse(node.ReadChildNodeValue("Radius"));
            return new BoundingSphere(center, radius);
        }

        public static void WriteBoundingBox(TWXmlNode node, BoundingBox box)
        {
            WriteVector3(node.CreateChildNode("Min"), box.Min);
            WriteVector3(node.CreateChildNode("Max"), box.Max);

        }

        public static BoundingBox ReadBoundingBox(TWXmlNode node)
        {
            Vector3 min = ReadVector3(node.FindChildNode("Min"));
            Vector3 max = ReadVector3(node.FindChildNode("Max"));

            return new BoundingBox(min, max);
        }

        public static void WriteQuaternion(TWXmlNode node, Quaternion rotation)
        {
            node.AddChildNode("X", rotation.X.ToString());
            node.AddChildNode("Y", rotation.Y.ToString());
            node.AddChildNode("Z", rotation.Z.ToString());
            node.AddChildNode("W", rotation.W.ToString());
        }

        public static Quaternion ReadQuaternion(TWXmlNode node)
        {
            Quaternion q = new Quaternion();
            q.X = Single.Parse(node.ReadChildNodeValue("X"));
            q.Y = Single.Parse(node.ReadChildNodeValue("Y"));
            q.Z = Single.Parse(node.ReadChildNodeValue("Z"));
            q.W = Single.Parse(node.ReadChildNodeValue("W"));

            return q;
        }
    }
}