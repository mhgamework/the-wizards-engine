using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Text;
using System.Data;
using System.Windows.Forms;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Sky
{
    public class SkyDome
    {
        private GraphicsDevice mGDevice;

        private VertexBuffer mVertexBuffer;
        private VertexDeclaration decl;

        float m_fRadiusHoriz;
        float m_fRadiusUp;
        int m_nThetaSides;
        int m_nPhiSides;			// Phi is angle from zenith, theta is azimuth angle.
        Vector3 m_vCenter;

        int mNumVertices;

        public SkyDome( IXNAGame game, Vector3 center, float radiusHoriz, float radiusUp )
        {
            mGDevice = game.GraphicsDevice;

            m_nThetaSides = 256; // Azimuth
            m_nPhiSides = 256;	// Zenith
            m_fRadiusHoriz = radiusHoriz;
            m_fRadiusUp = radiusUp;
            m_vCenter = center;

            mNumVertices = m_nThetaSides * 2 * ( m_nPhiSides + 1 );

            mVertexBuffer = new VertexBuffer( mGDevice, typeof( VertexPos ), mNumVertices, BufferUsage.WriteOnly );
            /*mVertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionOnly), mNumVertices, mGDevice,
                                             Usage.WriteOnly, CustomVertex.PositionOnly.Format, Pool.Managed);*/

            buildVertexBuffer();
        }

        public void Render( IXNAGame game )
        {
            //HRESULT hr;
            //g_ScatterVS = 2;

            // Choose shader and set up constants.
            //m_pShader1->Apply();

            // hr = g_Device->SetStreamSource(0, m_pVB, sizeof(SkyVertex));
            mGDevice.Vertices[ 0 ].SetSource( mVertexBuffer, 0, VertexPos.StrideSize );
            //mGDevice.SetStreamSource(0, mVertexBuffer, 0, CustomVertex.PositionOnly.StrideSize);

            if (decl == null) decl = VertexPos.CreateVertexDeclaration( game );
            mGDevice.VertexDeclaration = decl;
            //TODO: mGDevice.VertexFormat = CustomVertex.PositionOnly.Format;


            //if (FAILED(hr)) return hr;

            int startVertex = 0;
            int numFacesPerStrip = m_nPhiSides * 2;
            for ( int i = 0; i < m_nThetaSides; i++ )
            {
                mGDevice.DrawPrimitives( PrimitiveType.TriangleStrip, startVertex, numFacesPerStrip );
                //hr = g_Device->DrawPrimitive(D3DPT_TRIANGLESTRIP, startVertex, numFacesPerStrip);
                //if (FAILED(hr)) return hr;

                startVertex += ( m_nPhiSides + 1 ) * 2;
            }


            //return S_OK;
        }

        private void buildVertexBuffer()
        {
            // Fill in vertex data
            VertexPos[] pVertex = new VertexPos[ mNumVertices ];

            //m_pVB->Lock(0, 0, (BYTE**)&pVertex, 0);

            int nNumVertices = m_nThetaSides * 2 * ( m_nPhiSides + 1 );

            //m_pVB->Lock(0, 0, (BYTE**)&pVertex, 0);

            float degToRadian = (float)Math.PI / 180.0f;

            int index = 0;
            float x1, x2, y, z1, z2;
            for ( int i = 0; i < m_nThetaSides; i++ )
            {
                float fTheta1 = (float)i / m_nThetaSides * ( 360 * degToRadian );
                float fTheta2 = (float)( i + 1 ) / m_nThetaSides * ( 360 * degToRadian );
                for ( int j = 0; j <= m_nPhiSides; j++ )
                {
                    float fPhi = (float)j / m_nPhiSides * ( 120 * degToRadian );

                    x1 = (float)Math.Sin( fPhi ) * (float)Math.Cos( fTheta1 ) * m_fRadiusHoriz;
                    z1 = (float)Math.Sin( fPhi ) * (float)Math.Sin( fTheta1 ) * m_fRadiusHoriz;
                    x2 = (float)Math.Sin( fPhi ) * (float)Math.Cos( fTheta2 ) * m_fRadiusHoriz;
                    z2 = (float)Math.Sin( fPhi ) * (float)Math.Sin( fTheta2 ) * m_fRadiusHoriz;
                    y = (float)Math.Cos( fPhi ) * m_fRadiusUp;

                    pVertex[ index ] = new VertexPos();

                    pVertex[ index ].Position = new Vector3( x1, y, z1 ) + m_vCenter;
                    //pVertex[index].u = (float)i / m_nThetaSides;
                    //pVertex[index].v = (float)j / m_nPhiSides;
                    index++;
                    pVertex[ index ].Position = new Vector3( x2, y, z2 ) + m_vCenter;
                    //pVertex[index].u = (float)(i + 1) / m_nThetaSides;
                    //pVertex[index].v = (float)j / m_nPhiSides;
                    index++;
                }
            }

            mVertexBuffer.SetData( pVertex );
            //mVertexBuffer.SetData(pVertex, 0, LockFlags.None);

            //m_pVB->Unlock();
        }
    }
}
