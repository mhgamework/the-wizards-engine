using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using XmlHelper = MHGameWork.TheWizards.ServerClient.Engine.XmlHelper;
using StringHelper = MHGameWork.TheWizards.ServerClient.Engine.StringHelper;

namespace MHGameWork.TheWizards.ServerClient
{
    public abstract class ColladaSource
    {
        /// <summary>
        /// The type of array_element used in the source. (example: float_array). 
        /// For more information see the source element in the collada specification     
        /// </summary>
        public enum SourceArrayType
        {
            float_array,
            /// <summary>
            /// Not implemented yet.
            /// </summary>
            bool_array,
            /// <summary>
            /// Not implemented yet.
            /// </summary>
            Name_array,
            /// <summary>
            /// Not implemented yet.
            /// </summary>
            int_array,
            /// <summary>
            /// Not implemented yet.
            /// </summary>
            IDREF_Array,
        }

        public string ID;

        private SourceArrayType arrayType;






        public static ColladaSource FromXmlNode( XmlNode sourceNode )
        {

            //source = new ColladaMesh.Source();
            string sourceID = XmlHelper.GetXmlAttribute( sourceNode, "id" );

            //Create the correct source corresponding the array_type

            //Check for float_array

            XmlNode floatArray = XmlHelper.GetChildNode( sourceNode, "float_array" );
            if ( floatArray != null )
            {
                ColladaSourceFloat source = new ColladaSourceFloat();

                source.ID = sourceID;

                //This was the old code, and i have no clue why to use the numberformatinfo, so i simplified it.
                //
                //int count = Convert.ToInt32( XmlHelper.GetXmlAttribute( floatArray,
                //    "count" ), NumberFormatInfo.InvariantInfo );

                int count = Int32.Parse( XmlHelper.GetXmlAttribute( floatArray, "count" ) );


                if ( count == 0 )
                {
                    //No data available
                    source.Data = null;
                }
                else
                {
                    List<float> floats = new List<float>( StringHelper.ConvertStringToFloatArray( floatArray.InnerText ) );

                    // Fill the array up

                    while ( floats.Count < count )
                        floats.Add( 0.0f );

                    source.Data = floats;
                }

                return source;
            }



            //At the moment only a source containing an float_array is supported.
            throw new Exception( "This source type is not yet implemented" );

        }





        public SourceArrayType ArrayType
        {
            get { return arrayType; }
            protected set { arrayType = value; }
        }



    }

    public class ColladaSourceFloat : ColladaSource
    {
        public List<float> Data;

        public ColladaSourceFloat()
        {
            ArrayType = ColladaSource.SourceArrayType.float_array;
        }

        public Vector3 GetVector3( int index )
        {
            return new Vector3( Data[ index * 3 ], Data[ index * 3 + 1 ], Data[ index * 3 + 2 ] );
        }

        public override string ToString()
        {
            return "ColladaSource: " + ID + " - float_array";
        }
    }
}
