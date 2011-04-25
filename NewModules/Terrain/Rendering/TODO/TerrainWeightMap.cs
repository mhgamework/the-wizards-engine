using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    public class TerrainWeightMap : IDisposable
    {
        protected int width = 0;
        protected int length = 0;
        protected ServerClient.Editor.EditorTerrainAlphaMap[] alphaMaps;

        public TerrainWeightMap( int nWidth, int nLength )
        {
            width = nWidth;
            length = nLength;

            alphaMaps = new EditorTerrainAlphaMap[ 4 ];
            alphaMaps[ 0 ] = new EditorTerrainAlphaMap( nWidth, nLength );
            alphaMaps[ 1 ] = new EditorTerrainAlphaMap( nWidth, nLength );
            alphaMaps[ 2 ] = new EditorTerrainAlphaMap( nWidth, nLength );
            alphaMaps[ 3 ] = new EditorTerrainAlphaMap( nWidth, nLength );
        }

        //public EditorTerrainAlphaMap(int width, int length, string filename)
        //{
        //    Load( width, length, filename );
        //}

        ~TerrainWeightMap()
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

                if ( alphaMaps != null )
                {
                    alphaMaps[ 0 ].Dispose();
                    alphaMaps[ 1 ].Dispose();
                    alphaMaps[ 2 ].Dispose();
                    alphaMaps[ 3 ].Dispose();
                }
                alphaMaps = null;
            }
        }

        public void SaveTextureToDisk( GraphicsDevice device, string filename, ImageFileFormat format )
        {
            Color[] data = new Color[ width * length ];

            for ( int ix = 0; ix < width; ix++ )
            {
                for ( int iz = 0; iz < length; iz++ )
                {
                    byte a, r, g, b;
                    r = alphaMaps[ 0 ].GetSample( ix, iz );
                    g = alphaMaps[ 1 ].GetSample( ix, iz );
                    b = alphaMaps[ 2 ].GetSample( ix, iz );
                    a = alphaMaps[ 3 ].GetSample( ix, iz );

                    data[ iz * length + ix ] = new Color( r, g, b, a );
                }
            }


            Texture2D tex = new Texture2D( device, width, length, 0, TextureUsage.None, SurfaceFormat.Color );
            tex.SetData( data );
            tex.GenerateMipMaps( TextureFilter.Linear );

            tex.Save( filename, format );

        }


        public void SetSample( int texNum, int x, int z, byte value )
        {
            alphaMaps[ texNum ].SetSample( x, z, value );

        }

        public byte GetSample( int texNum, int x, int z )
        {
            return alphaMaps[ texNum ].GetSample( x, z );
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