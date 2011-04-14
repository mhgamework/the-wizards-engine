using System;
using System.Collections.Generic;
using System.Xml.Serialization;
// ReSharper disable InconsistentNaming

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.XmlTypeAttribute( AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema" )]
    [System.Xml.Serialization.XmlRootAttribute( Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false )]
    public class COLLADA
    {
        [XmlAttribute]
        public string version;

        public asset asset;

        [XmlElement( "library_geometries", typeof( library_geometries ) )]
        public library_geometries[] library_geometries;

        [XmlElement( "library_visual_scenes", typeof( library_visual_scenes ) )]
        public library_visual_scenes[] library_visual_scenes;

        public scene scene;


        public static COLLADA FromStream( System.IO.Stream strm )
        {
            COLLADA collada;
            XmlSerializer serializer = null;
            serializer = new XmlSerializer( typeof( MHGameWork.TheWizards.Collada.COLLADA140.COLLADA ) );


            collada = (COLLADA)serializer.Deserialize( strm );

            return collada;
        }

    }

    public class scene
    {
        [XmlElement( "extra", typeof( extra ) )]
        public extra[] extras;

        public instance_visual_scene instance_visual_scene;

    }

    public class instance_visual_scene
    {
        [XmlElement( "extra", typeof( extra ) )]
        public extra[] extras;

        [XmlAttribute]
        public string sid;
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string url;

    }
}
