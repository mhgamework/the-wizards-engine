using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class node
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string sid;
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public node_type type;
        [XmlAttribute]
        public string layer;

        public asset asset;


        [XmlElement( "node", typeof( node ) )]
        public node[] nodes;

        //TODO: more transformation elements

        [XmlElement( "matrix", typeof( matrix ) )]
        public matrix[] matrices;

        [XmlElement( "rotate", typeof( rotate ) )]
        public rotate[] rotate;

        [XmlElement( "scale", typeof( scale ) )]
        public scale[] scale;

        [XmlElement( "translate", typeof( translate ) )]
        public translate[] translate;

        //TODO: more instance's

        [XmlElement( "instance_geometry", typeof( instance_geometry ) )]
        public instance_geometry[] instance_geometries;

        [XmlElement( "instance_controller", typeof( instance_controller ) )]
        public instance_controller[] instance_controllers;


        public override string ToString()
        {
            return string.Format( "Id: {0}, Sid: {1}, Name: {2}, Type: {3}", id, sid, name, type );
        }
    }

    public class rotate : IXmlSerializable
    {
        [XmlAttribute]
        public string sid;

        public Microsoft.Xna.Framework.Vector3 Axis;
        public float Angle;



        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml( System.Xml.XmlReader reader )
        {
            sid = reader.GetAttribute( "sid" );

            string s = reader.ReadString();
            s = s.Replace( '\n', ' ' );

            float[] floats = float_array.StringToFloatArray( s );

            Axis = new Microsoft.Xna.Framework.Vector3( floats[ 0 ], floats[ 1 ], floats[ 2 ] );
            Angle = floats[ 3 ];

            reader.ReadEndElement();


        }



        public void WriteXml( System.Xml.XmlWriter writer )
        {
            throw new System.Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
    public class scale : IXmlSerializable
    {
        [XmlAttribute]
        public string sid;

        public Microsoft.Xna.Framework.Vector3 Scale;


        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml( System.Xml.XmlReader reader )
        {
            sid = reader.GetAttribute( "sid" );

            string s = reader.ReadString();
            s = s.Replace( '\n', ' ' );

            float[] floats = float_array.StringToFloatArray( s );

            Scale = new Microsoft.Xna.Framework.Vector3( floats[ 0 ], floats[ 1 ], floats[ 2 ] );

            reader.ReadEndElement();


        }



        public void WriteXml( System.Xml.XmlWriter writer )
        {
            throw new System.Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
    public class translate : IXmlSerializable
    {
        [XmlAttribute]
        public string sid;

        public Microsoft.Xna.Framework.Vector3 Translation;


        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml( System.Xml.XmlReader reader )
        {
            sid = reader.GetAttribute( "sid" );

            string s = reader.ReadString();
            s = s.Replace( '\n', ' ' );

            float[] floats = float_array.StringToFloatArray( s );

            Translation = new Microsoft.Xna.Framework.Vector3( floats[ 0 ], floats[ 1 ], floats[ 2 ] );

            reader.ReadEndElement();


        }



        public void WriteXml( System.Xml.XmlWriter writer )
        {
            throw new System.Exception( "The method or operation is not implemented." );
        }

        #endregion
    }

}