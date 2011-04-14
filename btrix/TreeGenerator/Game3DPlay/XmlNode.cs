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

        public TWXmlNode( XmlDocument document, string name )
        {
            XmlElement parentNode = document.CreateElement( name );
            document.AppendChild( parentNode );
            node = parentNode;

        }

        private TWXmlNode( XmlNode nNode )
        {
            node = nNode;
        }

        public TWXmlNode CreateChildNode( string childNodeName )
        {
            XmlNode childNode = XmlDocument.CreateElement( childNodeName );

            node.AppendChild( childNode );

            return new TWXmlNode( childNode );
        }
        public void AddChildNode( string name, string value )
        {

            TWXmlNode childNode = CreateChildNode( name );

           
                childNode.Value = value;


        }
        public string ReadChildNodeValue( string name )
        {
            string value = FindChildNode( name ).Value;
            return value;
        }

        public int ReadChildNodeValueInt( string name )
        {
            return int.Parse( ReadChildNodeValue( name ) );
        }

        public void AddAttribute( string attributeName, string attributeValue )
        {
            XmlAttribute attribute = XmlDocument.CreateAttribute( attributeName );
            attribute.Value = attributeValue;
            node.Attributes.Append( attribute );
        }
        public void AddAttributeInt( string attributeName, int attributeValue )
        {
            XmlAttribute attribute = XmlDocument.CreateAttribute( attributeName );
            attribute.Value = attributeValue.ToString();
            node.Attributes.Append( attribute );


        }

        public void AddCData( string cdata )
        {
            XmlCDataSection cdataSection = XmlDocument.CreateCDataSection( cdata );
            node.AppendChild( cdataSection );
        }

        public string ReadCData()
        {
            //if ( node.ChildNodes.Count == 0 ) throw new InvalidOperationException( "This node doesn't contain CData" );
            for ( int i = 0; i < node.ChildNodes.Count; i++ )
            {
                if ( node.ChildNodes[ i ].NodeType == XmlNodeType.CDATA )
                    return node.ChildNodes[ i ].Value;
            }

            throw new InvalidOperationException( "This node doesn't contain CData" );
        }


        public TWXmlNode FindChildNode( string name )
        {
            if ( node.ChildNodes.Count == 0 ) return null;
            for ( int i = 0; i < node.ChildNodes.Count; i++ )
            {
                if ( node.ChildNodes[ i ].Name == name )
                    return new TWXmlNode( node.ChildNodes[ i ] );
            }

            return null;
        }

        public TWXmlNode[] GetChildNodes()
        {
            TWXmlNode[] children = new TWXmlNode[ node.ChildNodes.Count ];
            for ( int i = 0; i < node.ChildNodes.Count; i++ )
            {
                children[ i ] = new TWXmlNode( node.ChildNodes[ i ] );
            }

            return children;
        }

        public TWXmlNode[] FindChildNodes( string name )
        {
            TWXmlNode[] children = new TWXmlNode[ node.ChildNodes.Count ];
            int outI = 0;

            for ( int i = 0; i < node.ChildNodes.Count; i++ )
            {
                if ( node.ChildNodes[ i ].Name != name )
                    continue;

                children[ outI ] = new TWXmlNode( node.ChildNodes[ i ] );
                outI++;


            }
            TWXmlNode[] ret = new TWXmlNode[ outI ];
            Array.Copy( children, ret, outI );
            return ret;
        }

        public string GetAttribute( string attName )
        {
            return node.Attributes[ attName ].Value;
        }


        public int GetAttributeInt( string attName )
        {
            return int.Parse( node.Attributes[ attName ].Value );
        }


        /// <summary>
        /// When Value is set to null, all child nodes will be deleted
        /// </summary>
        public string Value
        {
            get 
            {
                // If this node is a self-closing node, value is by definition null
                if ( node.HasChildNodes == false ) return null;
                return node.InnerText; 
            }
            set
            {
                
                if ( value == null )
                {
                    // I have defined that a value of null is a self-closing node, thus without child nodes
                    while (node.HasChildNodes)
                    {
                        node.RemoveChild( node.FirstChild );
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
            xmlDoc.InsertBefore( xmlDoc.CreateXmlDeclaration( "1.0", "UTF-8", null ), xmlDoc.DocumentElement );
            return xmlDoc;
        }

        public static void SaveXmlFile( string filename, System.Xml.XmlDocument doc )
        {
            System.IO.FileStream fs = null;
            try
            {
                fs = new System.IO.FileStream( filename
                   , System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Delete );

                doc.Save( fs );
            }
            finally
            {
                if ( fs != null )
                    fs.Close();
            }


        }
        public static TWXmlNode GetRootNodeFromFile( string filename )
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load( filename );
            return new TWXmlNode( doc.DocumentElement );
        }
    }
}
