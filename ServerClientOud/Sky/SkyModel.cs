using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Text;
using System.Data;
using System.Windows.Forms;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.Sky.Scattering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MHGameWork.TheWizards.ServerClient.Sky
{
    public class SkyModel
    {
        private SkyDome mSkyDome;

        //private Mesh mSphere;
        private float mRadius;

        private IScatter mScatter;
        private Vector3 mSunDirection;

        private float DegToRad = (float)Math.PI / 180.0f;

        private float mSunPos;
        private float mSunAngle;

        #region Properties
        //public int NumTriangles { get { return mSphere.NumberFaces; } }
        //public int NumVertices { get { return mSphere.NumberVertices; } }

        public float Radius { get { return mRadius; } }
        public float SunPosition
        {
            get
            {
                return mSunPos;
            }
            set
            {
                if ( value < -90) value = -90;
                if ( value > 90 ) value = 90;
                    mSunPos = value;
            }
        }

        public float SunAngle
        {
            get
            {
                return mSunAngle;
            }
            set
            {
                if ( value > -90 && value < 90 )
                    mSunAngle = value;
            }
        }

        public Vector3 SunDirection
        {
            get
            {
                mSunDirection.X = (float)Math.Cos( mSunPos * DegToRad );
                mSunDirection.Z = (float)Math.Sin( mSunPos * DegToRad );
                mSunDirection.Y = 0.3f;
                mSunDirection.Normalize();


                return mSunDirection;
            }
        }

        public IScatter ScatterMethod { get { return mScatter; } }
        #endregion

        public SkyModel( IXNAGame game, float skyRadius, Type scatterType, Texture2D[] terrainTextures )
        {
            mRadius = skyRadius;

            //mSphere = Mesh.Sphere(gDevice, mRadius, 50, 50);

            if ( scatterType == typeof( Hoffman_Preethem ) )
            {
                mScatter = new Hoffman_Preethem( game, terrainTextures );
            }

            mSunPos = 38.6f;

            mSunDirection = new Vector3( (float)Math.Cos( 90 * DegToRad ),
                                        (float)Math.Cos( mSunPos * DegToRad ),
                                        (float)Math.Sin( mSunPos * DegToRad ) );

            mSkyDome = new SkyDome( game, new Vector3( 0, 0, 0 ), 2000.0f, 1500.0f );
        }

        public void Render( IXNAGame game, Camera camera, Plane reflectPlane )
        {
            Matrix reflMatrix = Matrix.Identity;
            if ( reflectPlane != default( Plane ) )
                reflMatrix = Matrix.CreateReflection( reflectPlane );
            //reflMatrix.Reflect(reflectPlane);

            Matrix world = Matrix.Identity;

            world = Matrix.CreateTranslation( camera.Position.X, 0.0f, camera.Position.Z );

            world = world * reflMatrix/* * camera.ViewProj*/;

            mSunDirection.Y = (float)Math.Cos( mSunPos * DegToRad );
            mSunDirection.Z = (float)Math.Sin( mSunPos * DegToRad );

            mScatter.BeginSkyLightRender(game, camera, world, mSunDirection );

            mSkyDome.Render( game );

            mScatter.EndSkyLightRender();
        }
    }
}