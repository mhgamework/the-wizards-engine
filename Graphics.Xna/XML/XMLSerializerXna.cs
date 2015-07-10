using System;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.XML
{
    /// <summary>
    /// Created by MHGameWork, part of The Wizards, Revision 138
    /// A static class containing functions for serializing custom data types to XML.
    /// </summary>
    public static class XMLSerializerXna
    {
        private static StringBuilder builder;

        static XMLSerializerXna()
        {
            builder = new StringBuilder();

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
                element.Offset = Int16.Parse(elementNode.ReadChildNodeValue("Offset"));
                element.Stream = Int16.Parse(elementNode.ReadChildNodeValue("Stream"));
                element.UsageIndex = Byte.Parse(elementNode.ReadChildNodeValue("UsageIndex"));
                element.VertexElementFormat = (VertexElementFormat)Enum.Parse(typeof(VertexElementFormat), elementNode.ReadChildNodeValue("VertexElementFormat"));
                element.VertexElementMethod = (VertexElementMethod)Enum.Parse(typeof(VertexElementMethod), elementNode.ReadChildNodeValue("VertexElementMethod"));
                element.VertexElementUsage = (VertexElementUsage)Enum.Parse(typeof(VertexElementUsage), elementNode.ReadChildNodeValue("VertexElementUsage"));
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

        public static void WriteTransformation(TWXmlNode node, Transformation transformation)
        {
            XMLSerializer.WriteVector3(node.CreateChildNode("Scaling"), transformation.Scaling);
            XMLSerializer.WriteQuaternion(node.CreateChildNode("Rotation"), transformation.Rotation);
            XMLSerializer.WriteVector3(node.CreateChildNode("Translation"), transformation.Translation);
        }

        public static Transformation ReadTransformation(TWXmlNode node)
        {
            Transformation transformation = new Transformation();
            transformation.Scaling = XMLSerializer.ReadVector3(node.FindChildNode("Scaling"));
            transformation.Rotation = XMLSerializer.ReadQuaternion(node.FindChildNode("Rotation"));
            transformation.Translation = XMLSerializer.ReadVector3(node.FindChildNode("Translation"));


            return transformation;
        }
    }
}
