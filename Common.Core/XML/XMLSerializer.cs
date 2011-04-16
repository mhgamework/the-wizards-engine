using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient
{
    /// <summary>
    /// Created by MHGameWork, part of The Wizards, Revision 138
    /// A static class containing functions for serializing custom data types to XML.
    /// </summary>
    public static class XMLSerializer
    {
        private static StringBuilder _builder;

        static XMLSerializer()
        {
            _builder = new StringBuilder();

        }

        public static void WriteVertexDeclaration(TWXmlNode node, VertexDeclaration decl)
        {
            if (decl == null) { node.Value = "NULL"; return; }
            //TWXmlNode node = parentNode.CreateChildNode( name );

            VertexElement[] elements = decl.GetVertexElements();

            TWXmlNode elementsNode = node.CreateChildNode("Elements");

            elementsNode.AddAttribute("count", elements.Length.ToString());

            for (int i = 0; i < elements.Length; i++)
            {
                TWXmlNode elementNode = elementsNode.CreateChildNode("VertexElement");
                elementNode.AddChildNode("Offset", elements[i].Offset.ToString());
                elementNode.AddChildNode("Stream", elements[i].Stream.ToString());
                elementNode.AddChildNode("UsageIndex", elements[i].UsageIndex.ToString());
                elementNode.AddChildNode("VertexElementFormat", elements[i].VertexElementFormat.ToString());
                elementNode.AddChildNode("VertexElementMethod", elements[i].VertexElementMethod.ToString());
                elementNode.AddChildNode("VertexElementUsage", elements[i].VertexElementUsage.ToString());
            }

        }
        public static VertexDeclaration ReadVertexDeclaration(XNAGame game, TWXmlNode node)
        {
            if (node.Value == "NULL") return null;

            TWXmlNode elementsNode = node.FindChildNode("Elements");
            VertexElement[] elements = new VertexElement[elementsNode.GetAttributeInt("count")];

            TWXmlNode[] elementNodes = elementsNode.FindChildNodes("VertexElement");
            if (elementNodes.Length != elements.Length) throw new InvalidOperationException("Invalid XML format!");
            for (int i = 0; i < elementNodes.Length; i++)
            {
                TWXmlNode elementNode = elementNodes[i];
                VertexElement element = new VertexElement();
                element.Offset = short.Parse(elementNode.ReadChildNodeValue("Offset"));
                element.Stream = short.Parse(elementNode.ReadChildNodeValue("Stream"));
                element.UsageIndex = byte.Parse(elementNode.ReadChildNodeValue("UsageIndex"));
                element.VertexElementFormat = (VertexElementFormat)System.Enum.Parse(typeof(VertexElementFormat), elementNode.ReadChildNodeValue("VertexElementFormat"));
                element.VertexElementMethod = (VertexElementMethod)System.Enum.Parse(typeof(VertexElementMethod), elementNode.ReadChildNodeValue("VertexElementMethod"));
                element.VertexElementUsage = (VertexElementUsage)System.Enum.Parse(typeof(VertexElementUsage), elementNode.ReadChildNodeValue("VertexElementUsage"));
                elements[i] = element;

            }

            return new VertexDeclaration(game.GraphicsDevice, elements);
        }

        public static void WriteVertexBuffer(TWXmlNode node, VertexBuffer vertexBuffer)
        {
            if (vertexBuffer == null) { node.Value = "NULL"; return; }
            byte[] data = new byte[vertexBuffer.SizeInBytes];
            vertexBuffer.GetData<byte>(data);

            if (data.Length != vertexBuffer.SizeInBytes) throw new Exception("While writing this method, i assumed those were equal");

            node.AddChildNode("BufferUsage", vertexBuffer.BufferUsage.ToString());
            TWXmlNode dataNode = node.CreateChildNode("Data");
            dataNode.AddAttribute("length", data.Length.ToString());

            //TODO: this data should be contained in a 'CData' block
            dataNode.AddCData(Convert.ToBase64String(data));




        }
        public static VertexBuffer ReadVertexBuffer(TWXmlNode node, XNAGame game)
        {
            if (node.Value == "NULL") return null;

            BufferUsage bufferUsage = (BufferUsage)Enum.Parse(typeof(BufferUsage), node.ReadChildNodeValue("BufferUsage"));

            TWXmlNode dataNode = node.FindChildNode("Data");
            int length = dataNode.GetAttributeInt("length");
            byte[] data = new byte[length];
            data = Convert.FromBase64String(dataNode.ReadCData());


            VertexBuffer vb = new VertexBuffer(game.GraphicsDevice, length, bufferUsage);
            vb.SetData<byte>(data);

            return vb;
        }

        public static void WriteIndexBuffer(TWXmlNode node, IndexBuffer indexBuffer)
        {
            if (indexBuffer == null) { node.Value = "NULL"; return; }
            byte[] data = new byte[indexBuffer.SizeInBytes];
            indexBuffer.GetData<byte>(data);

            if (data.Length != indexBuffer.SizeInBytes) throw new Exception("While writing this method, i assumed those were equal");

            node.AddChildNode("BufferUsage", indexBuffer.BufferUsage.ToString());
            node.AddChildNode("IndexElementSize", indexBuffer.IndexElementSize.ToString());
            TWXmlNode dataNode = node.CreateChildNode("Data");
            dataNode.AddAttribute("length", data.Length.ToString());

            //TODO: this data should be contained in a 'CData' block
            dataNode.AddCData(Convert.ToBase64String(data));
        }
        public static IndexBuffer ReadIndexBuffer(TWXmlNode node, XNAGame game)
        {
            if (node.Value == "NULL") return null;

            BufferUsage bufferUsage = (BufferUsage)Enum.Parse(typeof(BufferUsage), node.ReadChildNodeValue("BufferUsage"));

            IndexElementSize elementSize = (IndexElementSize)Enum.Parse(typeof(IndexElementSize), node.ReadChildNodeValue("IndexElementSize"));

            TWXmlNode dataNode = node.FindChildNode("Data");
            int length = dataNode.GetAttributeInt("length");
            byte[] data;// = new byte[ length ];
            data = Convert.FromBase64String(dataNode.ReadCData());


            IndexBuffer ib = new IndexBuffer(game.GraphicsDevice, length, bufferUsage, elementSize);
            ib.SetData<byte>(data);

            return ib;
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

        public static void WriteFloatArray(TWXmlNode node, float[] data)
        {
            if (data == null) { node.Value = "NULL"; return; }
            _builder.Clear();
            if (data.Length != 0)
            {
                _builder.Append(data[0]);
                for (int i = 1; i < data.Length; i++)
                {
                    _builder.Append(" ");

                    _builder.Append(data[i].ToString());
                }
            }

            node.Value = _builder.ToString();

        }
        public static float[] ReadFloatArray(TWXmlNode node)
        {
            if (node.Value == "NULL") return null;

            string val = node.Value;
            string[] floatStrings = val.Split(new char[] { ' ' });
            float[] floats = new float[floatStrings.Length];

            for (int i = 0; i < floatStrings.Length; i++)
            {
                floats[i] = float.Parse(floatStrings[i]);
            }

            return floats;

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
                v.X = float.Parse(nodeVector.GetAttribute("X"));
                v.Y = float.Parse(nodeVector.GetAttribute("Y"));
                v.Z = float.Parse(nodeVector.GetAttribute("Z"));

                array[i] = v;
                i++;
            }

            return array;

        }

        public static void WriteIntArray(TWXmlNode node, int[] data)
        {
            if (data == null) { node.Value = "NULL"; return; }
            string val = "";
            if (data.Length != 0)
            {
                val = data[0].ToString();
                for (int i = 1; i < data.Length; i++)
                {
                    val += " " + data[i].ToString();
                }
            }

            node.Value = val;

        }
        public static int[] ReadIntArray(TWXmlNode node)
        {
            if (node.Value == "NULL") return null;

            string val = node.Value;
            string[] floatStrings = val.Split(new char[] { ' ' });
            int[] floats = new int[floatStrings.Length];

            for (int i = 0; i < floatStrings.Length; i++)
            {
                floats[i] = int.Parse(floatStrings[i]);
            }

            return floats;

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

            v.X = float.Parse(node.ReadChildNodeValue("X"));
            v.Y = float.Parse(node.ReadChildNodeValue("Y"));
            v.Z = float.Parse(node.ReadChildNodeValue("Z"));

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

            v.X = float.Parse(node.ReadChildNodeValue("X"));
            v.Y = float.Parse(node.ReadChildNodeValue("Y"));

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
            float radius = float.Parse(node.ReadChildNodeValue("Radius"));
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

        public static void WriteTransformation(TWXmlNode node, Transformation transformation)
        {
            WriteVector3(node.CreateChildNode("Scaling"), transformation.Scaling);
            WriteQuaternion(node.CreateChildNode("Rotation"), transformation.Rotation);
            WriteVector3(node.CreateChildNode("Translation"), transformation.Translation);
        }
        public static Transformation ReadTransformation(TWXmlNode node)
        {
            Transformation transformation = new Transformation();
            transformation.Scaling = ReadVector3(node.FindChildNode("Scaling"));
            transformation.Rotation = ReadQuaternion(node.FindChildNode("Rotation"));
            transformation.Translation = ReadVector3(node.FindChildNode("Translation"));


            return transformation;
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
            q.X = float.Parse(node.ReadChildNodeValue("X"));
            q.Y = float.Parse(node.ReadChildNodeValue("Y"));
            q.Z = float.Parse(node.ReadChildNodeValue("Z"));
            q.W = float.Parse(node.ReadChildNodeValue("W"));

            return q;
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

    }
}
