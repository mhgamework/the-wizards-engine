using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public class vertices
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;


        [XmlElement( "input", typeof( input_unshared ) )]
        public input_unshared[] inputs;

        public extra extra;

        public input_unshared FindInputBySemantic( InputSemantic semantic )
        {
            for ( int k = 0; k < inputs.Length; k++ )
            {
                if ( inputs[ k ].semantic == InputSemantic.Position )
                    return inputs[ k ];


            }
            return null;
        }

    }
}