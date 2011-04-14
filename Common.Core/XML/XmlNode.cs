using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace MHGameWork.TheWizards.ServerClient
{
    /// <summary>
    /// Created by MHGameWork, part of The Wizards, Revision 138
    /// </summary>
    public class TWXmlNode
    {
        public XmlDocument Document
        {
            get { return node.OwnerDocument; }
            //set { myVar = value; }
        }


        public string Name
        {
            get { return node.Name; }
        }



        XmlNode node;

        public TWXmlNode(XmlDocument document, string name)
        {
            XmlElement parentNode = document.CreateElement(name);
            document.AppendChild(parentNode);
            node = parentNode;

        }

        private TWXmlNode(XmlNode nNode)
        {
            node = nNode;
        }

        /// <summary>
        /// Returns true when there are child nodes in this node. A simple Value counts as a childnode too!
        /// </summary>
        public bool HasChildNodes
        {
            get { return node.HasChildNodes; }
        }

        public TWXmlNode CreateChildNode(string childNodeName)
        {
            XmlNode childNode = XmlDocument.CreateElement(childNodeName);

            node.AppendChild(childNode);

            return new TWXmlNode(childNode);
        }
        public void AddChildNode(string name, string value)
        {

            TWXmlNode childNode = CreateChildNode(name);


            childNode.Value = value;


        }
        public void AddChildNode(string name, int value)
        {
            AddChildNode(name, value.ToString());
        }

        public string ReadChildNodeValue(string name)
        {
            string value = FindChildNode(name).Value;
            return value;
        }

        public int ReadChildNodeValueInt(string name)
        {
            return int.Parse(ReadChildNodeValue(name));
        }

        public string ReadChildNodeValue(string name, string defaultValue)
        {
            TWXmlNode child = FindChildNode(name);
            if (child == null) return defaultValue;

            return child.Value;
        }
        public int ReadChildNodeValueInt(string name, int defaultValue)
        {
            return int.Parse(ReadChildNodeValue(name, defaultValue.ToString()));
        }

        public void AddAttribute(string attributeName, string attributeValue)
        {
            XmlAttribute attribute = XmlDocument.CreateAttribute(attributeName);
            attribute.Value = attributeValue;
            node.Attributes.Append(attribute);
        }
        public void AddAttributeInt(string attributeName, int attributeValue)
        {
            XmlAttribute attribute = XmlDocument.CreateAttribute(attributeName);
            attribute.Value = attributeValue.ToString();
            node.Attributes.Append(attribute);


        }

        public void AddCData(string cdata)
        {
            XmlCDataSection cdataSection = XmlDocument.CreateCDataSection(cdata);
            node.AppendChild(cdataSection);
        }

        public string ReadCData()
        {
            //if ( node.ChildNodes.Count == 0 ) throw new InvalidOperationException( "This node doesn't contain CData" );
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].NodeType == XmlNodeType.CDATA)
                    return node.ChildNodes[i].Value;
            }

            throw new InvalidOperationException("This node doesn't contain CData");
        }


        public TWXmlNode FindChildNode(string name)
        {
            if (node.ChildNodes.Count == 0) return null;
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Name == name)
                    return new TWXmlNode(node.ChildNodes[i]);
            }

            return null;
        }

        /// <summary>
        /// Gets an array of all the child nodes in this node. 
        /// Not a recursive method. Only first level childnodes.
        /// </summary>
        /// <returns></returns>
        public TWXmlNode[] GetChildNodes()
        {
            TWXmlNode[] children = new TWXmlNode[node.ChildNodes.Count];
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                children[i] = new TWXmlNode(node.ChildNodes[i]);
            }

            return children;
        }

        public TWXmlNode[] FindChildNodes(string name)
        {
            TWXmlNode[] children = new TWXmlNode[node.ChildNodes.Count];
            int outI = 0;

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Name != name)
                    continue;

                children[outI] = new TWXmlNode(node.ChildNodes[i]);
                outI++;


            }
            TWXmlNode[] ret = new TWXmlNode[outI];
            Array.Copy(children, ret, outI);
            return ret;
        }

        public string GetAttribute(string attName)
        {
            return node.Attributes[attName].Value;
        }


        public int GetAttributeInt(string attName)
        {
            return int.Parse(node.Attributes[attName].Value);
        }

        /// <summary>
        /// Removes all data from this node, except the node's name (thus making it like it was newly created)
        /// Equivalent to XmlNode.RemoveAll()
        /// </summary>
        public void Clear()
        {
            node.RemoveAll();
        }


        /// <summary>
        /// When Value is set to null, all child nodes will be deleted
        /// </summary>
        public string Value
        {
            get
            {
                // If this node is a self-closing node, value is by definition null
                if (node.HasChildNodes == false) return null;
                return node.InnerText;
            }
            set
            {

                if (value == null)
                {
                    // I have defined that a value of null is a self-closing node, thus without child nodes
                    while (node.HasChildNodes)
                    {
                        node.RemoveChild(node.FirstChild);
                    }
                    return;
                }
                node.InnerText = value;
            }
        }


        public XmlDocument XmlDocument
        {
            get { return node.OwnerDocument; }
        }

        public static XmlDocument CreateXmlDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Insert declaration as first element
            xmlDoc.InsertBefore(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null), xmlDoc.DocumentElement);
            return xmlDoc;
        }

        public static void SaveXmlFile(string filename, System.Xml.XmlDocument doc)
        {
            System.IO.FileStream fs = null;
            try
            {
                fs = new System.IO.FileStream(filename
                   , System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Delete);

                doc.Save(fs);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }


        }
        public static TWXmlNode GetRootNodeFromFile(string filename)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(filename);

            return new TWXmlNode(doc.DocumentElement);
        }
        public static TWXmlNode GetRootNodeFromStream(System.IO.Stream strm)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(strm);

            return new TWXmlNode(doc.DocumentElement);
        }
        public static TWXmlNode FromXmlNode(XmlNode _node)
        {
            return new TWXmlNode(_node);
        }


        public override string ToString()
        {
            return "Name: " + Name + " NumChildren: " + node.ChildNodes.Count.ToString();
        }
    }
}
