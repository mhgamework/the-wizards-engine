using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Color = System.Drawing.Color;
using MHGameWork.TheWizards.ServerClient.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Water
{
    struct TerrainVertex
    {
        public static VertexDeclaration Decleration;

        /*public static VertexFormats Format = VertexFormats.Position | VertexFormats.Normal | 
            VertexFormats.Texture0 | VertexFormats.Texture1 | VertexFormats.Texture2 | VertexFormats.Texture3;*/

        public Vector3 Pos;
        public Vector3 Normal;
        public Vector2 TiledTexC;
        public Vector2 NonTiledTexC;

        TerrainVertex( float x, float y, float z, float nx, float ny, float nz,
                      float tiledU, float tiledV, float nonTiledU, float nonTiledV )
        {
            Decleration = null;
            Pos = new Vector3( x, y, z );
            Normal = new Vector3( nx, ny, nz );
            TiledTexC = new Vector2( tiledU, tiledV );
            NonTiledTexC = new Vector2( nonTiledU, nonTiledV );
        }

        TerrainVertex( Vector3 p, Vector3 n, Vector2 tiledUV, Vector2 nonTiledUV )
        {
            Decleration = null;
            Pos = p;
            Normal = n;
            TiledTexC = tiledUV;
            NonTiledTexC = nonTiledUV;
        }
    }

    public struct WaterDMapVertex
    {
        [VertexElementAttribute( VertexElementUsage.Position )]
        public Vector3 pos;
        [VertexElementAttribute( VertexElementUsage.TextureCoordinate )]
        public Vector2 scaledTexC;     // [a, b]
        [VertexElementAttribute( VertexElementUsage.TextureCoordinate )]
        public Vector2 normalizedTexC; // [0, 1]


        public static int StrideSize = 3 * 4 + 2 * 4 + 2 * 4;

        public static VertexElement[] CreateVertexElements()
        {
            return new VertexElement[] 
                            {new VertexElement(0,0, VertexElementFormat.Vector3,VertexElementMethod.Default , VertexElementUsage.Position,0),
                             new VertexElement(0,12,VertexElementFormat.Vector2,VertexElementMethod.Default,VertexElementUsage.TextureCoordinate,0),
                             new VertexElement(0,20,VertexElementFormat.Vector2,VertexElementMethod.Default,VertexElementUsage.TextureCoordinate, 1)};


        }

        //public static VertexFormats Format = CustomVertex.PositionColored.Format;
        //public static VertexDeclaration Decleration;
    };

    struct VertexPos
    {
        private static VertexDeclaration mDecleration;

        public static VertexDeclaration CreateVertexDeclaration( IXNAGame game )
        {
            VertexElement[] vertPos = new VertexElement[] {
                new VertexElement(0,0, 
                VertexElementFormat.Vector3, 
                VertexElementMethod.Default,
                VertexElementUsage.Position,
                0)};
            return new VertexDeclaration( game.GraphicsDevice, vertPos );
        }

        public static int StrideSize = 4 * 3;

        private Vector3 mPos;

        /*VertexPos()
        {
            mDecleration = null;
            mPos = new Vector3(0.0f, 0.0f, 0.0f);
        }*/

        VertexPos( float x, float y, float z )
        {
            mDecleration = null;
            mPos = new Vector3( x, y, z );
        }

        VertexPos( Vector3 pos )
        {
            mDecleration = null;
            mPos = pos;
        }

        public Vector3 Position
        {
            get { return mPos; }
            set { mPos = value; }
        }
    }

    struct VertexPosColor
    {
        public static VertexDeclaration mDecleration;
        Vector3 mPos;
        Color mColor;

        /*VertexPosColor()
        {
            mDecleration = null;
            mPos = new Vector3(0.0f, 0.0f, 0.0f);
            mColor = Color.Black;
        }*/

        VertexPosColor( float x, float y, float z, Color c )
        {
            mDecleration = null;
            mPos = new Vector3( x, y, z );
            mColor = c;
        }

        VertexPosColor( Vector3 p, Color c )
        {
            mDecleration = null;
            mPos = p;
            mColor = c;
        }
    }

    struct VertexPosNormal
    {
        public static VertexDeclaration mDecleration;
        Vector3 mPos;
        Vector3 mNormal;

        /*VertexPosNormal()
        {
            mDecleration = null;
            mPos = new Vector3(0.0f, 0.0f, 0.0f);
            mNormal = new Vector3(0.0f, 0.0f, 0.0f);
        }*/

        VertexPosNormal( float x, float y, float z, float nx, float ny, float nz )
        {
            mDecleration = null;
            mPos = new Vector3( x, y, z );
            mNormal = new Vector3( nx, ny, nz );
        }

        VertexPosNormal( Vector3 pos, Vector3 norm )
        {
            mDecleration = null;
            mPos = pos;
            mNormal = norm;
        }
    }

    struct VertexPosNormTex
    {
        public static VertexDeclaration Decleration;
        public Vector3 Pos;
        public Vector3 Normal;
        public Vector2 Tex0;

        public static int StrideSize { get { return 32; } }

        /*VertexPosNormTex()
        {
            mDecleration = null;
            mPos = new Vector3(0.0f, 0.0f, 0.0f);
            mNormal = new Vector3(0.0f, 0.0f, 0.0f);
            mTex0 = new Vector2(0.0f, 0.0f);
        }*/

        VertexPosNormTex( float x, float y, float z, float nx, float ny, float nz, float u, float v )
        {
            Decleration = null;
            Pos = new Vector3( x, y, z );
            Normal = new Vector3( nx, ny, nz );
            Tex0 = new Vector2( u, v );
        }

        VertexPosNormTex( Vector3 p, Vector3 n, Vector2 uv )
        {
            Decleration = null;
            Pos = p;
            Normal = n;
            Tex0 = uv;
        }
    }

    struct VertexPosTex
    {
        public static VertexDeclaration mDecleration;
        Vector3 mPos;
        Vector2 mTex0;

        /*VertexPosTex()
        {
            mDecleration = null;
            mPos = new Vector3(0.0f, 0.0f, 0.0f);
            mTex0 = new Vector2(0.0f, 0.0f);
        }*/

        VertexPosTex( float x, float y, float z, float u, float v )
        {
            mDecleration = null;
            mPos = new Vector3( x, y, z );
            mTex0 = new Vector2( u, v );
        }

        VertexPosTex( Vector3 p, Vector2 uv )
        {
            mDecleration = null;
            mPos = p;
            mTex0 = uv;
        }
    }
}
