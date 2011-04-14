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
using Cursor = System.Windows.Forms.Cursor;

namespace MHGameWork.TheWizards.ServerClient.Sky.Scattering
{
    public class Hoffman_Preethem : IScatter
    {
        #region Fields
        private GraphicsDevice mGDevice;

        // Atmosphere data
        float mHGgFunction;                   // g value in Henyey Greenstein approximation function
        float mInscatteringMultiplier;        // Multiply inscattering term with this factor
        float mBetaRayMultiplier;             // Multiply Rayleigh scattering coefficient with this factor
        float mBetaMieMultiplier;             // Multiply Mie scattering coefficient with this factor
        float mSunIntensity;                  // Current sun intensity
        float mTurbitity;                     // Current turbidity

        Vector3 mBetaRay;                        // Rayleigh total scattering coefficient
        Vector3 mBetaDashRay;                    // Rayleigh angular scattering coefficient without phase term
        Vector3 mBetaMie;                        // Mie total scattering coefficient
        Vector3 mBetaDashMie;                    // Mie angular scattering coefficient without phase term
        Vector4 mSunColorAndIntensity;           // Sun color and intensity packed for the shader
        Vector3 mDiffuseConstant;


        BasicShader mSkyEffect;
        string mTechnique;
        EffectParameter mEyePosHandle;                 // eye position parameter
        EffectParameter mSunDirHandle;                 // sun direction parameter
        EffectParameter mBetaRPlusBetaMHandle;         // scattering coefficients parameter
        EffectParameter mHGgHandle;                    // Henyey Greenstein parameter
        EffectParameter mBetaDashRHandle;              // scattering coefficients parameter
        EffectParameter mBetaDashMHandle;              // scattering coefficients parameter
        EffectParameter mOneOverBetaRPlusBetaMHandle;  // scattering coefficients parameter
        EffectParameter mMultipliersHandle;            // multiplier parameter
        EffectParameter mSunColorAndIntensityHandle;   // sun color/intensity parameter
        EffectParameter mWVPHandle;                    // world view projection matrix parameter
        EffectParameter mWorldViewHandle;              // world view matrix parameter

        private EffectParameter[] mTextureHandles;

        private Texture2D[] mTextures;

        private Vector3 mLastSunDirection = Vector3.Zero;
        #endregion

        public Vector4 SunColorAndIntensity
        {
            get { return mSunColorAndIntensity; }
        }

        public float Inscattering
        {
            get { return mInscatteringMultiplier; }
            set { mInscatteringMultiplier = value; }
        }
        public float Mie
        {
            get { return mBetaMieMultiplier; }
            set { mBetaMieMultiplier = value; }
        }
        public float Rayleigh
        {
            get { return mBetaRayMultiplier; }
            set { mBetaRayMultiplier = value; }
        }

        public Hoffman_Preethem( IXNAGame game, Texture2D[] terrainTextures )
        {
            mGDevice = game.GraphicsDevice;

            mTextures = terrainTextures;

            /*m_HGg = (0.89999998f);
            m_inscatteringMultiplier = ( 1.0f );
            m_betaRayMultiplier = ( 3.0f );
            m_betaMieMultiplier = (0.00060000003f);
            m_sunIntensity = ( 1.0f );
            m_turbitity = (0.8f);*/

            //ORIGINAL

            mHGgFunction = ( 0.89999998f );
            mInscatteringMultiplier = 0.909999967f;
            mBetaRayMultiplier = ( 5.0f );
            mBetaMieMultiplier = ( 0.00060000003f );
            mSunIntensity = ( 1.0f );
            mTurbitity = ( 0.8f );


            //MHGW
            /*mHGgFunction = ( 0.89999998f );
            mInscatteringMultiplier = 0.7f;
            mBetaRayMultiplier = ( 1.0f );
            mBetaMieMultiplier = ( 0.0030000003f );
            mSunIntensity = ( 1.0f );
            mTurbitity = ( 0.8f );*/


            calculateScatteringConstants();

            buildEffect( game );
        }

        ///  Atmospheric Scattering Constants
        /// Sets the Atmospheric Scattering Constants
        private void calculateScatteringConstants()
        {
            const float n = 1.0003f;
            const float N = 2.545e25f;
            const float pn = 0.035f;

            float[] fLambda = new float[ 3 ];
            float[] fLambda2 = new float[ 3 ];
            float[] fLambda4 = new float[ 3 ];

            fLambda[ 0 ] = 1.0f / 650e-9f;   // red
            fLambda[ 1 ] = 1.0f / 570e-9f;   // green
            fLambda[ 2 ] = 1.0f / 475e-9f;   // blue

            for ( int i = 0; i < 3; ++i )
            {
                fLambda2[ i ] = fLambda[ i ] * fLambda[ i ];
                fLambda4[ i ] = fLambda2[ i ] * fLambda2[ i ];
            }

            Vector3 vLambda2 = new Vector3( fLambda2[ 0 ], fLambda2[ 1 ], fLambda2[ 2 ] );
            Vector3 vLambda4 = new Vector3( fLambda4[ 0 ], fLambda4[ 1 ], fLambda4[ 2 ] );

            // Rayleigh scattering constants

            float fTemp = (float)Math.PI * (float)Math.PI * ( n * n - 1.0f ) * ( n * n - 1.0f ) * ( 6.0f + 3.0f * pn ) / ( 6.0f - 7.0f * pn ) / N;
            float fBeta = 8.0f * fTemp * (float)Math.PI / 3.0f;

            mBetaRay = fBeta * vLambda4;

            float fBetaDash = fTemp / 2.0f;

            mBetaDashRay = fBetaDash * vLambda4;

            // Mie scattering constants

            float T = 2.0f;
            float c = ( 6.544f * T - 6.51f ) * 1e-17f;
            float fTemp2 = 0.434f * c * ( 2.0f * (float)Math.PI ) * ( 2.0f * (float)Math.PI ) * 0.5f;

            mBetaDashMie = fTemp2 * vLambda2;

            float[] K = { 0.685f, 0.679f, 0.670f };
            float fTemp3 = 0.434f * c * (float)Math.PI * ( 2.0f * (float)Math.PI ) * ( 2.0f * (float)Math.PI );

            Vector3 vBetaMieTemp = new Vector3( K[ 0 ] * fLambda2[ 0 ], K[ 1 ] * fLambda2[ 1 ], K[ 2 ] * fLambda2[ 2 ] );

            mBetaMie = fTemp3 * vBetaMieTemp;
            mDiffuseConstant = new Vector3( 0.138f, 0.113f, 0.08f );
        }

        private void computeAttenuation( float a_theta )
        {
            float fBeta = 0.04608365822050f * mTurbitity - 0.04586025928522f;
            float fTauR, fTauA;
            float[] fTau = new float[ 3 ];
            float m = 1.0f / ( (float)Math.Cos( a_theta ) + 0.15f * (float)Math.Pow( 93.885f - a_theta / (float)Math.PI * 180.0f, -1.253f ) );  // Relative Optical Mass
            float[] fLambda = { 0.65f, 0.57f, 0.475f };

            for ( int i = 0; i < 3; ++i )
            {
                // Rayleigh Scattering
                // lambda in um.

                fTauR = (float)Math.Exp( -m * 0.008735f * (float)Math.Pow( fLambda[ i ], ( -4.08f ) ) );

                // Aerosal (water + dust) attenuation
                // beta - amount of aerosols present 
                // alpha - ratio of small to large particle sizes. (0:4,usually 1.3)

                const float fAlpha = 1.3f;
                fTauA = (float)Math.Exp( -m * fBeta * (float)Math.Pow( fLambda[ i ], -fAlpha ) );  // lambda should be in um


                fTau[ i ] = fTauR * fTauA;

            }

            mSunColorAndIntensity = new Vector4( fTau[ 0 ], fTau[ 1 ], fTau[ 2 ], mSunIntensity * 100.0f );
        }

        public void BeginSkyLightRender(IXNAGame game, Camera camera, Matrix world, Vector3 sunDirection )
        {
            // Compute thetaS dependencies

            Vector3 vecZenith = new Vector3( 0.0f, 1.0f, 0.0f );
            float thetaS = Vector3.Dot( sunDirection, vecZenith );
            thetaS = (float)Math.Acos( thetaS );

            if ( mLastSunDirection != sunDirection )
            {
                computeAttenuation( thetaS );
            }

            mLastSunDirection = sunDirection;

            #region Set shader constants

            float fReflectance = 0.1f;

            Vector3 vecBetaR = mBetaRay * mBetaRayMultiplier;
            Vector3 vecBetaDashR = mBetaDashRay * mBetaRayMultiplier;
            Vector3 vecBetaM = mBetaMie * mBetaMieMultiplier;
            Vector3 vecBetaDashM = mBetaDashMie * mBetaMieMultiplier;
            Vector3 vecBetaRM = vecBetaR + vecBetaM;
            Vector3 vecOneOverBetaRM = new Vector3( 1.0f / vecBetaRM.X, 1.0f / vecBetaRM.Y, 1.0f / vecBetaRM.Z );
            Vector3 vecG = new Vector3( 1.0f - mHGgFunction * mHGgFunction, 1.0f + mHGgFunction * mHGgFunction, 2.0f * mHGgFunction );
            Vector4 vecTermMulitpliers = new Vector4( mInscatteringMultiplier, 0.138f * fReflectance, 0.113f * fReflectance, 0.08f * fReflectance );

            float[] temp = { camera.Position.X, camera.Position.Y, camera.Position.Z };
            mEyePosHandle.SetValue( temp );

            temp = new float[] { sunDirection.X, sunDirection.Y, sunDirection.Z };
            mSunDirHandle.SetValue( temp );

            temp = new float[] { vecBetaRM.X, vecBetaRM.Y, vecBetaRM.Z };
            mBetaRPlusBetaMHandle.SetValue( temp );

            temp = new float[] { vecG.X, vecG.Y, vecG.Z };
            mHGgHandle.SetValue( temp );

            temp = new float[] { vecBetaDashR.X, vecBetaDashR.Y, vecBetaDashR.Z };
            mBetaDashRHandle.SetValue( temp );

            temp = new float[] { vecBetaDashM.X, vecBetaDashM.Y, vecBetaDashM.Z };
            mBetaDashMHandle.SetValue( temp );

            temp = new float[] { vecOneOverBetaRM.X, vecOneOverBetaRM.Y, vecOneOverBetaRM.Z };
            mOneOverBetaRPlusBetaMHandle.SetValue( temp );

            mMultipliersHandle.SetValue( vecTermMulitpliers );
            mSunColorAndIntensityHandle.SetValue( mSunColorAndIntensity );


            mWVPHandle.SetValue( world * camera.ViewProj );
            mWVPHandle.SetValue( game.Camera.ViewProjection );
            mWorldViewHandle.SetValue( world * camera.View );
            #endregion

            mSkyEffect.effect.Begin( SaveStateMode.SaveState );
            mSkyEffect.effect.CurrentTechnique.Passes[ 0 ].Begin();
        }

        public void EndSkyLightRender()
        {
            mSkyEffect.effect.CurrentTechnique.Passes[ 0 ].End();
            mSkyEffect.effect.End();
        }

        public void BeginAerialPerspectiveRender( IXNAGame game, Camera camera, Matrix world, Vector3 sunDirection, bool useWaterTech )
        {
            if ( useWaterTech )
                mTechnique = "TerrainUnderWater";
            else
                mTechnique = "HoffmanTerrain";

            mSkyEffect.SetTechnique( mTechnique );
            mSkyEffect.effect.CommitChanges();

            BeginSkyLightRender(game, camera, world, sunDirection );
        }

        public void EndAerialPerspectiveRender()
        {
            EndSkyLightRender();

            //mTechnique = mSkyEffect.GetTechnique( "HoffmanSky" );

            mSkyEffect.SetTechnique( "HoffmanSky" );
            mSkyEffect.effect.CommitChanges();
        }

        private void buildEffect( IXNAGame game )
        {
            string errorMessage;
            mSkyEffect = BasicShader.LoadFromFXFile( game, new GameFile( System.Windows.Forms.Application.StartupPath + @"\Shaders\Hoffman_Preethem.fx" ) );
            //mSkyEffect = Effect.FromFile( mGDevice, @"..\..\Effects\Hoffman_Preethem.fx", null, null, ShaderFlags.Debug, null, out errorMessage );

            /*if ( !String.IsNullOrEmpty( errorMessage ) )
            {
                Cursor.Show();
                mGDevice.Dispose();
                MessageBox.Show( errorMessage, "Error Building Effect" );

                Application.Exit();
            }*/

            #region Obatin Effect Handles
            //TODO: mTechnique = mSkyEffect.GetTechnique( "HoffmanSky" );
            mTechnique = "HoffmanSky";
            mSkyEffect.SetTechnique( mTechnique );

            mEyePosHandle = mSkyEffect.GetParameter( "EyePos" );
            mSunDirHandle = mSkyEffect.GetParameter( "SunDir" );
            mBetaRPlusBetaMHandle = mSkyEffect.GetParameter( "BetaRPlusBetaM" );
            mHGgHandle = mSkyEffect.GetParameter( "HGg" );
            mBetaDashRHandle = mSkyEffect.GetParameter( "BetaDashR" );
            mBetaDashMHandle = mSkyEffect.GetParameter( "BetaDashM" );
            mOneOverBetaRPlusBetaMHandle = mSkyEffect.GetParameter( "OneOverBetaRPlusBetaM" );
            mMultipliersHandle = mSkyEffect.GetParameter( "Multipliers" );
            mSunColorAndIntensityHandle = mSkyEffect.GetParameter( "SunColorAndIntensity" );
            mWVPHandle = mSkyEffect.GetParameter( "WorldViewProj" );
            mWorldViewHandle = mSkyEffect.GetParameter( "WorldView" );

            mTextureHandles = new EffectParameter[ mTextures.Length ];

            for ( int i = 0; i < mTextureHandles.Length; i++ )
            {
                if ( i == mTextureHandles.Length - 1 )
                    mTextureHandles[ i ] = mSkyEffect.GetParameter( "gBlendMap" );
                else
                    mTextureHandles[ i ] = mSkyEffect.GetParameter( "gTex" + i );
            }

            #endregion

            for ( int i = 0; i < mTextureHandles.Length; i++ )
            {
                //mSkyEffect.SetValue( mTextureHandles[ i ], mTextures[ i ] );
                mTextureHandles[ i ].SetValue( mTextures[ i ] );
            }
        }
    }
}
