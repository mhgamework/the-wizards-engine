using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Common.GeoMipMap
{
	public class Buffer2D : IDisposable
	{
		protected int width = 0;
		protected int length = 0;
		protected byte[] data;

		public Buffer2D(int nWidth, int nLength)
		{
			Create( nWidth, nLength );
		}

		public Buffer2D(int width, int length, string filename)
		{
			Load( width, length, filename );
		}

		~Buffer2D()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose(bool disposing)
		{
			lock ( this )
			{
				if ( disposing )
				{
				}

				data = null;
			}
		}


		public bool Create(int width, int length)
		{

			lock ( this )
			{
				this.width = width;
				this.length = length;

				data = new byte[ width * length ];

			}

			return true;
		}

		public bool Load(int width, int length, string filename)
		{
			if ( File.Exists( filename ) != true )
				return false;

			lock ( this )
			{
				this.width = width;
				this.length = length;

				using ( FileStream stream = File.OpenRead( filename ) )
				{
					BinaryReader reader = new BinaryReader( stream );
					data = reader.ReadBytes( (int)stream.Length );
					reader.Close();
				}
			}

			return true;
		}

		public bool Save(string filename)
		{
			lock ( this )
			{
				using ( FileStream stream = File.Create( filename ) )
				{
					BinaryWriter writer = new BinaryWriter( stream );
					writer.Write( data );
					writer.Close();
				}
			}

			return true;
		}

		public void SetSample(int x, int z, byte value)
		{
			if ( x < 0 ) return;
			if ( z < 0 ) return;
			if ( x > width - 1 ) return;
			if ( z > length - 1 ) return;

			int index = z * width + x;

			if ( data == null || index < 0 || index >= data.Length )
				return;

			data[ index ] = value;
		}

		public byte GetSample(int x, int z)
		{
			if ( x < 0 ) x = 0;
			if ( z < 0 ) z = 0;
			if ( x > width - 1 ) x = width - 1;
			if ( z > length - 1 ) z = length - 1;

			int index = z * width + x;

			if ( data == null || index < 0 || index >= data.Length )
				return 0;

			return data[ index ];
		}

		public float CalculateSample(float x, float z)
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
				GetSample( left, top ),
				GetSample( left + 1, top ),
				xNormalized );

			float bottomHeight = MathHelper.Lerp(
				GetSample( left, top + 1 ),
				GetSample( left + 1, top + 1 ),
				xNormalized );

			// next, interpolate between those two values to calculate the height at our
			// position.
			return MathHelper.Lerp( topHeight, bottomHeight, zNormalized );

		}

		public byte this[ int x, int z ]
		{
			get { return GetSample( x, z ); }
		}

		public int Width
		{
			get { return width; }
		}

		public int Length
		{
			get { return length; }
		}

	}
}
