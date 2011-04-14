using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace MHGameWork.TheWizards.Common.GeoMipMap
{
    public class TerrainLightmapGenerator
    {
        private Terrain terrain;
        private Vector3 lightDirection;
        private float ambientLight;

        public TerrainLightmapGenerator( Terrain terrain )
        {
            this.terrain = terrain;
            this.lightDirection = Vector3.Normalize( new Vector3( 1f, 0.1f, 1f ) );
            this.ambientLight = 0.3f;
        }

        public Texture2D Generate( GraphicsDevice device, int x, int z )
        {
            int size = terrain.BlockSize +1;

            Texture2D texture = new Texture2D(device, size, size, 1, TextureUsage.None, SurfaceFormat.Luminance8);
			byte[] data = Generate( x, z );
        
            texture.SetData<byte>( data );

            return texture;
        }

		public byte[] Generate(int x, int z)
		{
			int size = terrain.BlockSize + 1;

			byte[] data = new byte[ size * size ];
			float offset = terrain.Scale * 0.5f;

			for ( int tx = 0; tx < size; tx++ )
			{
				for ( int tz = 0; tz < size; tz++ )
				{
					int cx = x + tx;
					int cz = z + tz;

					Vector3 position = new Vector3( cx * terrain.Scale, terrain.HeightMap[ cx, cz ] * terrain.HeightScale, cz * terrain.Scale );
					Ray ray = new Ray( position + lightDirection * 2000f, lightDirection );

					float light = ambientLight;
					float hits = CheckIntersection( position, ray );

					Vector3 normal = terrain.GetAveragedNormal( cx, cz );
					float angleLight = Vector3.Dot( normal, lightDirection );
					light += angleLight - (float)hits / ( terrain.HeightScale * 15f );

					if ( light < ambientLight )
						light = ambientLight;

					if ( light > 1f )
						light = 1f;

					data[ tz * size + tx ] = (byte)( light * 255 );
				}
			}



			return data;
		}

        protected float CheckIntersection( Vector3 position, Ray ray )
        {
            Vector3 movement = Vector3.Normalize( new Vector3( ray.Direction.X, 0f, ray.Direction.Z ) ) * terrain.Scale;
            Vector3 target = position + movement;
            float hits = 0f;

            while( true )
            {
                // length of lightdir's projection
                float dist2d = Vector3.Distance( new Vector3( target.X, 0, target.Z ), new Vector3( ray.Position.X, 0, ray.Position.Z ) );
                float dist3d = Vector3.Distance( position, target );            // light direction
                float height = position.Y + ( dist3d * ray.Position.Y ) / dist2d;  // X(P) point

                // if the height has gone past off the scale then nothing can possibily hit it anymore
                if( height >= terrain.HeightScale )
                    break;

                int cx = (int)Math.Round( target.X / terrain.Scale );
                int cz = (int)Math.Round( target.Z / terrain.Scale );

                if( cx < 0 || cx >= terrain.HeightMap.Width ||
                    cz < 0 || cz >= terrain.HeightMap.Length )
                    break;

                float targetHeight = terrain.HeightMap[ cx, cz ] * terrain.HeightScale;

                // check if height in point P is bigger than point X's height
                if( targetHeight > height )
                    hits += targetHeight - height;

                target += movement;   // fetch new working point
            }

            return hits;
        }
    }
}
