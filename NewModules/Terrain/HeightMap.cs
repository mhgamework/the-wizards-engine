using System;
using System.IO;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Terrain
{
    /// <summary>
    /// TODO: should implement an interface IHeightMap, 
    /// which has an implementation that supports infinite heightmaps
    /// </summary>
    public class HeightMap : IDisposable
    {
        /*private float minimumHeight = 0f;
		private float maximumHeight = 1f;
		private float difference = 1f;
		private const float _multiplier = 255f / (float)byte.MaxValue;*/
        protected int width = 0;
        protected int length = 0;
        protected float[] data;

        public HeightMap( int nWidth, int nLength )
        {
            Create( nWidth, nLength );
        }

        public HeightMap( int width, int length, string filename )
        {
            Load( width, length, filename );
        }

        ~HeightMap()
        {
            Dispose( false );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            lock ( this )
            {
                if ( disposing )
                {
                }

                data = null;
            }
        }

     
        public bool Create( int width, int length )
        {

            lock ( this )
            {
                this.width = width;
                this.length = length;

                data = new float[ width * length ];

            }

            return true;
        }

        public bool Load( int width, int length, string filename )
        {
            if ( File.Exists( filename ) != true )
                return false;

            lock ( this )
            {
                this.width = width;
                this.length = length;
                data = new float[ width * length ];

                using ( FileStream stream = File.OpenRead( filename ) )
                {
                    BinaryReader reader = new BinaryReader( stream );
                    int i = 0;
                    while ( reader.BaseStream.Position < reader.BaseStream.Length )
                    {
                        data[ i ] = reader.ReadSingle();
                        i++;
                    }
                    reader.Close();
                }
            }

            return true;
        }

        public bool Save( string filename )
        {
            lock ( this )
            {
                using ( FileStream stream = File.Create( filename ) )
                {
                    BinaryWriter writer = new BinaryWriter( stream );
                    for ( int i = 0; i < data.Length; i++ )
                    {
                        writer.Write( data[ i ] );

                    }
                    writer.Close();
                }
            }

            return true;
        }


        /// <summary>
        /// Returns an array of byte with each 4 bytes representing a float. X are columns and Z are rows.
        /// Index formula: i = 4 * (z * width + x)
        /// </summary>
        /// <returns></returns>
        private byte[] ToByteArray()
        {
            TWByteWriter bw = new TWByteWriter();
            for ( int i = 0; i < data.Length; i++ )
            {
                float f = data[ i ];
                bw.Write( f );
            }
            return bw.ToBytesAndClose();
        }

        public void SaveToXml( TWXmlNode node )
        {
            byte[] bytes = ToByteArray();
            const string indexFormula = "i = 4 * (z * width + x)";

            node.AddAttributeInt( "Width", width );
            node.AddAttributeInt( "Length", length );
            node.AddAttribute( "IndexFormula", indexFormula );
            node.AddCData( Convert.ToBase64String( bytes ) );
        }

        public static HeightMap LoadFromXml( TWXmlNode node )
        {
            int width = node.GetAttributeInt( "Width" );
            int length = node.GetAttributeInt( "Length" );
            string indexFormula = node.GetAttribute( "IndexFormula" );

            if ( "i = 4 * (z * width + x)".Equals( indexFormula ) == false ) throw new Exception( "Invalid data format!!" );


            byte[] bytes = Convert.FromBase64String( node.ReadCData() );

            HeightMap map = new HeightMap( width, length );

            float[] d = new float[ width * length ];

            TWByteReader br = new TWByteReader( bytes );
            for ( int iz = 0; iz < length; iz++ )
            {
                for ( int ix = 0; ix < width; ix++ )
                {
                    d[ iz * width + ix ] = br.ReadSingle();
                }
            }

            map.data = d;


            br.Close();

            return map;
        }

        public void SetHeight( int x, int z, float height )
        {
            if ( x < 0 ) return;
            if ( z < 0 ) return;
            if ( x > width - 1 ) return;
            if ( z > length - 1 ) return;

            int index = z * width + x;

            if ( data == null || index < 0 || index >= data.Length )
                return;

            data[ index ] = height;
        }

        public float GetHeight( int x, int z )
        {
            if ( x < 0 ) x = 0;
            if ( z < 0 ) z = 0;
            if ( x > width - 1 ) x = width - 1;
            if ( z > length - 1 ) z = length - 1;

            int index = z * width + x;

            if ( data == null || index < 0 || index >= data.Length )
                return 0f;

            return data[ index ];
        }

        public float AddHeight( int x, int z, float addHeight )
        {
            if ( x < 0 ) return 0;
            if ( z < 0 ) return 0;
            if ( x > width - 1 ) return 0;
            if ( z > length - 1 ) return 0;

            int index = z * width + x;

            if ( data == null || index < 0 || index >= data.Length )
                return 0;

            data[ index ] += addHeight;
            return data[ index ];
        }

        public float CalculateHeight( float x, float z )
        {
            // the first thing we need to do is figure out where on the heightmap
            // "position" is. This'll make the math much simpler later.
            //Vector3 positionOnHeightmap = position - heightmapPosition;

            // we'll use integer division to figure out where in the "heights" array
            // positionOnHeightmap is. Remember that integer division always rounds
            // down, so that the result of these divisions is the indices of the "upper
            // left" of the 4 corners of that cell.
            int left, top;
            left = (int)Math.Floor( x ); //(int)positionOnHeightmap.X / (int)terrainScale;
            top = (int)Math.Floor( z );//(int)positionOnHeightmap.Z / (int)terrainScale;

            // next, we'll use modulus to find out how far away we are from the upper
            // left corner of the cell. Mod will give us a value from 0 to terrainScale,
            // which we then divide by terrainScale to normalize 0 to 1.
            float xNormalized = x - left;
            float zNormalized = z - top;

            // Now that we've calculated the indices of the corners of our cell, and
            // where we are in that cell, we'll use bilinear interpolation to calculuate
            // our height. This process is best explained with a diagram, so please see
            // the accompanying doc for more information.
            // First, calculate the heights on the bottom and top edge of our cell by
            // interpolating from the left and right sides.
            float topHeight = MathHelper.Lerp(
                GetHeight( left, top ),
                GetHeight( left + 1, top ),
                xNormalized );

            float bottomHeight = MathHelper.Lerp(
                GetHeight( left, top + 1 ),
                GetHeight( left + 1, top + 1 ),
                xNormalized );

            // next, interpolate between those two values to calculate the height at our
            // position.
            return MathHelper.Lerp( topHeight, bottomHeight, zNormalized );

        }

        public float this[ int x, int z ]
        {
            get { return GetHeight( x, z ); }
        }

        public int Width
        {
            get { return width; }
        }

        public int Length
        {
            get { return length; }
        }

        public float[] Data { get { return data; } }
    }
}