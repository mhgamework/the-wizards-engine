using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class mesh
    {
        public vertices vertices;
        public extra extra;


        [XmlElement( "source", typeof( source ) )]
        public source[] sources;



        // primitive_elements
        [XmlElement( "polygons", typeof( polygons ) )]
        public polygons polygons;

        [XmlElement( "triangles", typeof( triangles ) )]
        public triangles triangles;


        //TODO: Implement other primitive_elements*/

        /// <summary>
        /// Reference ID: to get source with id mesh-normals, the referenceID is #mesh-normals
        /// </summary>
        /// <param name="referenceID"></param>
        /// <returns></returns>
        public source GetSourceByReferenceID( string referenceID )
        {
            referenceID = referenceID.Substring( 1 );
            for ( int i = 0; i < sources.Length; i++ )
            {
                if ( sources[ i ].id == referenceID ) return sources[ i ];
            }
            return null;
        }
    }

    public class triangles
    {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public int count;
        [XmlAttribute]
        public string material;


        public extra extra;

        [XmlElement( "input", typeof( input_shared ) )]
        public input_shared[] inputs;


        [XmlElement( "p", typeof( polygons_p ) )]
        public polygons_p p;




    }
}