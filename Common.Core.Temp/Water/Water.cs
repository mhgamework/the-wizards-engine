using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Water
{
    public struct WaterInfoTest
    {
        public Vector3 sunDirection;
        public string fxFilename;
        public int vertRows;
        public int vertCols;
        public float dx;
        public float dz;
        public string waveMapFileName0;
        public string waveMapFileName1;
        public Vector2 waveNMapVelocity0;
        public Vector2 waveNMapVelocity1;
        public Vector2 scaleHeights;
        public float texScale;
        public Matrix toWorld;
    }

    public class Water
    {

        public Matrix WorldMatrix = Matrix.Identity;
        public DepthStencilBuffer DepthBuffer;

        #region Fields
        private GraphicsDevice mGDevice;
        private Mesh mMesh;

        private VertexBuffer mVertexBuffer;

        public RenderTarget2D mReflectMap;
        public RenderTarget2D mRefractMap;

        private TextureCube mEnvironmentMap;

        //normal maps
        private Texture mWaveMap0;
        private Texture mWaveMap1;

        //normal map offsets
        private Vector2 mWaveNMapOffset0;
        private Vector2 mWaveNMapOffset1;

        //displacement map offsets
        private Vector2 mWaveDMapOffset0;
        private Vector2 mWaveDMapOffset1;

        private SubGrid mRootSubGrid;
        private List<SubGrid> mVisibleSubgrids;
        private int numLeaves;

        private WaterInfoTest mInfo;
        private float mWidth;
        private float mHeight;
        private int mNumTris;
        private int mNumVertices;
        #endregion

        #region FX Handles
        IXNAGame game;

        public BasicShader mEffect;

        EffectTechnique mTechHandle;
        EffectParameter mWVPHandle;
        EffectParameter mWorldHandle;
        EffectParameter mWorldInvHandle;
        EffectParameter mSunDirHandle;
        EffectParameter mSunColorHandle;
        EffectParameter mEyePosHandle;
        EffectParameter mWaveMap0Handle;
        EffectParameter mWaveMap1Handle;
        EffectParameter mWaveNMapOffset0Handle;
        EffectParameter mWaveNMapOffset1Handle;
        EffectParameter mWaveDMapOffset0Handle;
        EffectParameter mWaveDMapOffset1Handle;
        EffectParameter mWaveDispMap0Handle;
        EffectParameter mWaveDispMap1Handle;
        EffectParameter mScaleHeightsHandle;
        EffectParameter mGridStepSizeHandle;

        EffectParameter mReflectMapHandle;
        EffectParameter mRefractMapHandle;
        EffectParameter mEnvMapHandle;
        EffectParameter mFoamTexHandle;

        #endregion

        #region properties
        public WaterInfoTest WaterInfo { get { return mInfo; } }
        public int NumTriangles { get { return mMesh.NumberFaces; } }
        public int NumVertices { get { return mMesh.NumberVertices; } }

        public RenderTarget2D ReflectMap { get { return mReflectMap; } }
        public RenderTarget2D RefractMap { get { return mRefractMap; } }
        public TextureCube EnvironmentMap
        {
            get { return mEnvironmentMap; }
            set { mEnvironmentMap = value; }
        }
        #endregion

        public Water( IXNAGame _game, WaterInfoTest waterInfo, TextureCube envMap )
        {

            numLeaves = 0;
            game = _game;
            mGDevice = game.GraphicsDevice;

            mInfo = waterInfo;

            mWidth = ( waterInfo.vertCols - 1 ) * waterInfo.dx;
            mHeight = ( waterInfo.vertRows - 1 ) * waterInfo.dz;

            mWaveNMapOffset0 = new Vector2( 0, 0 );
            mWaveNMapOffset1 = new Vector2( 0, 0 );

            mWaveDMapOffset0 = new Vector2( 0, 0 );
            mWaveDMapOffset1 = new Vector2( 0, 0 );

            mNumTris = ( waterInfo.vertRows - 1 ) * ( waterInfo.vertCols - 1 ) * 2;
            mNumVertices = waterInfo.vertRows * waterInfo.vertCols;

            WorldMatrix = waterInfo.toWorld;

            #region Build the Mesh

            VertexElement[] decleration = new VertexElement[] 
                            {new VertexElement(0,0,VertexElementFormat.Vector3, VertexElementMethod.Default,VertexElementUsage .Position,0),
                             new VertexElement(0,12,VertexElementFormat.Vector2,VertexElementMethod.Default,VertexElementUsage.TextureCoordinate,0),
                             new VertexElement(0,20,VertexElementFormat.Vector2,VertexElementMethod.Default,VertexElementUsage.TextureCoordinate, 1)};

            VertexElement[] elems = WaterDMapVertex.CreateVertexElements();

            // Generate the water grid and write to water mesh

            //mMesh = new Mesh( mNumTris, mNumVertices, MeshFlags.Managed | MeshFlags.Use32Bit, elems, mGDevice );
            mMesh = new Mesh( game, mNumTris, mNumVertices, WaterDMapVertex.StrideSize
                , Graphics.AttributeSystem.CreateVertexDeclaration( game.GraphicsDevice, typeof( WaterDMapVertex ) ) );

            //Mesh temp = mMesh.Clone(MeshFlags.Managed, decleration, mGDevice);
            //mesh.Dispose();

            Vector3[] verts;
            int[] indices;

            Utils.GenTriGridOld( mInfo.vertRows, mInfo.vertCols, mInfo.dx, mInfo.dz, new Vector3( 0, 0, 0 ), out verts, out indices );

            WaterDMapVertex[] v = new WaterDMapVertex[ mNumVertices ];
            for ( int i = 0; i < mInfo.vertRows; ++i )
            {
                for ( int j = 0; j < mInfo.vertCols; ++j )
                {
                    int index = i * mInfo.vertCols + j;
                    v[ index ].pos = verts[ index ];
                    v[ index ].scaledTexC = new Vector2( (float)j / mInfo.vertCols,
                                                (float)i / mInfo.vertRows ) * mInfo.texScale;
                    v[ index ].normalizedTexC = new Vector2( (float)j / mInfo.vertCols,
                                                (float)i / mInfo.vertRows );
                }
            }

            //set the vertexbuffer data
            mMesh.SetVertexBufferData( v );

            //set the index data
            mMesh.SetIndexBufferData( indices );

            int[] adjac = new int[ mMesh.NumberFaces * 3 ];
            mMesh.GenerateAdjacency( 0.001f, adjac );
            //mMesh.OptimizeInPlace( MeshFlags.OptimizeVertexCache | MeshFlags.OptimizeAttributeSort, adjac );
            mMesh.OptimizeInPlace( 0, adjac );
            adjac = null;

            #endregion

            /*--------------------------------------------------------------------
             * Create the Textures
            */
            int m = mInfo.vertRows;
            int n = mInfo.vertCols;

            mWaveMap0 = Texture2D.FromFile( mGDevice, mInfo.waveMapFileName0 );
            mWaveMap1 = Texture2D.FromFile( mGDevice, mInfo.waveMapFileName1 );

            //mReflectMap = new RenderToTexture( mGDevice, 1024, 1024, 0, Format.A8R8G8B8, true, DepthFormat.D24X8, true );
            //mRefractMap = new RenderToTexture( mGDevice, 1024, 1024, 0, Format.A8R8G8B8, true, DepthFormat.D24X8, true );
            mReflectMap = new RenderTarget2D( game.GraphicsDevice, 1024, 1024, 0, SurfaceFormat.Color );
            mRefractMap = new RenderTarget2D( game.GraphicsDevice, 1024, 1024, 0, SurfaceFormat.Color );
            DepthBuffer = new DepthStencilBuffer( game.GraphicsDevice,
                                    1024,
                                    1024,
                                    game.GraphicsDevice.DepthStencilBuffer.Format );

            mVisibleSubgrids = new List<SubGrid>();

            buildGeometryHeirarchy();
            buildEffect( game, envMap );
        }

        ~Water()
        {
            if ( mMesh != null )
                mMesh.Dispose();
            mMesh = null;

            if ( mEffect != null )
                mEffect.Dispose();
            mEffect = null;

            if ( mWaveMap0 != null )
                mWaveMap0.Dispose();
            mWaveMap0 = null;

            if ( mWaveMap1 != null )
                mWaveMap1.Dispose();
            mWaveMap1 = null;

            mRefractMap = null;
            mRefractMap = null;
        }

        /*public void OnLostDevice()
        {
            mEffect.OnLostDevice();

            mReflectMap.OnLostDevice();
            mRefractMap.OnLostDevice();
        }

        public void OnResetDevice()
        {
            mEffect.OnResetDevice();

            mRefractMap.OnResetDevice();
            mReflectMap.OnResetDevice();
        }*/

        public void Update( float timeElapsed )
        {
            //update offsets
            mWaveNMapOffset0 += mInfo.waveNMapVelocity0 * timeElapsed;
            mWaveNMapOffset1 += mInfo.waveNMapVelocity1 * timeElapsed;

            if ( mWaveNMapOffset0.X >= 1.0f || mWaveNMapOffset0.X <= -1.0f )
                mWaveNMapOffset0.X = 0.0f;
            if ( mWaveNMapOffset1.X >= 1.0f || mWaveNMapOffset1.X <= -1.0f )
                mWaveNMapOffset1.X = 0.0f;
            if ( mWaveNMapOffset0.Y >= 1.0f || mWaveNMapOffset0.Y <= -1.0f )
                mWaveNMapOffset0.Y = 0.0f;
            if ( mWaveNMapOffset1.Y >= 1.0f || mWaveNMapOffset1.Y <= -1.0f )
                mWaveNMapOffset1.Y = 0.0f;
        }

        public void Render( Camera camera, Vector3 sunDirection, Vector4 sunColor )
        {
            recursiveFrustumCheck( mRootSubGrid, camera );

            mVisibleSubgrids.Sort();

            mWVPHandle.SetValue( mInfo.toWorld * camera.ViewProj );

            float[] float3 = new float[] { camera.Position.X, camera.Position.Y, camera.Position.Z };
            mEyePosHandle.SetValue( float3 );

            float[] float2 = new float[] { mWaveNMapOffset0.X, mWaveNMapOffset0.Y };
            mWaveNMapOffset0Handle.SetValue( float2 );

            float3 = new float[] { sunDirection.X, sunDirection.Y, sunDirection.Z };
            mSunDirHandle.SetValue( float3 );

            mSunColorHandle.SetValue( sunColor );

            float2[ 0 ] = mWaveNMapOffset1.X;
            float2[ 1 ] = mWaveNMapOffset1.Y;
            mWaveNMapOffset1Handle.SetValue( float2 );


            mReflectMapHandle.SetValue( mReflectMap.GetTexture() );
            mRefractMapHandle.SetValue( mRefractMap.GetTexture() );

            if ( mEnvironmentMap != null )
                mEnvMapHandle.SetValue( mEnvironmentMap );

            game.GraphicsDevice.SetRenderTarget( 0, null );

            mEffect.effect.Begin( SaveStateMode.SaveState );
            mEffect.effect.CurrentTechnique.Passes[ 0 ].Begin();

            foreach ( SubGrid subGrid in mVisibleSubgrids )
            {
                subGrid.mesh.DrawSubset( 0 );
            }

            mEffect.effect.CurrentTechnique.Passes[ 0 ].End();
            mEffect.effect.End();

            mVisibleSubgrids.Clear();
        }

        private void buildGeometryHeirarchy()
        {
            //VertexElement[] elems = WaterDMapVertex.Decleration.GetDeclaration();

            /*Mesh systemMesh = new Mesh( (int)mNumTris, (int)mNumVertices, MeshFlags.SystemMemory | MeshFlags.Use32Bit,
                                        elems, mGDevice );*/

            Vector3[] vertices;
            int[] indices;
            Utils.GenTriGrid( (int)mInfo.vertRows, (int)mInfo.vertCols, mInfo.dx, mInfo.dz, new Vector3( 0, 0, 0 ), out vertices, out indices );

            #region get data for the vertices
            WaterDMapVertex[] verts = new WaterDMapVertex[ mNumVertices ];
            for ( int i = 0; i < mInfo.vertRows; ++i )
            {
                for ( int j = 0; j < mInfo.vertCols; ++j )
                {
                    int index = i * mInfo.vertCols + j;
                    verts[ index ].pos = vertices[ index ];
                    verts[ index ].scaledTexC = new Vector2( (float)j / mInfo.vertCols,
                                                (float)i / mInfo.vertRows ) * mInfo.texScale;
                    verts[ index ].normalizedTexC = new Vector2( (float)j / mInfo.vertCols,
                                                (float)i / mInfo.vertRows );
                }
            }
            #endregion

            /*#region mesh index data, compute normals, optimize
            //write the vertex data
            systemMesh.SetVertexBufferData( verts, LockFlags.None );

            //write the index data
            systemMesh.SetIndexBufferData( indices, LockFlags.None );

            int[] adjacency = new int[ systemMesh.NumberFaces * 3 ];
            systemMesh.GenerateAdjacency( .001f, adjacency );
            systemMesh.OptimizeInPlace( MeshFlags.OptimizeVertexCache | MeshFlags.OptimizeAttributeSort, adjacency );
            #endregion*/

            /*mVertexBuffer = new VertexBuffer( mGDevice,
                mNumVertices * CustomVertex.PositionNormalTextured.StrideSize * 4,
                Usage.WriteOnly,
                CustomVertex.PositionNormalTextured.Format,
                Pool.Managed );*/

            mVertexBuffer = new VertexBuffer( mGDevice, typeof( VertexPositionNormalTexture ), mNumVertices, BufferUsage.WriteOnly );

            mVertexBuffer.SetData( verts );

            Rectangle Rec = new Rectangle(
                                0,
                                0,
                                ( mInfo.vertCols - 1 ),
                                ( mInfo.vertRows - 1 ) );

            mRootSubGrid = new SubGrid( mInfo.vertRows, mInfo.vertCols );

            recursiveTerrainBuild( mRootSubGrid, Rec, verts );

            /*systemMesh.Dispose();
            systemMesh = null;*/
        }

        private void buildSubGridMesh( Rectangle R, WaterDMapVertex[] gridVerts, bool buildMesh, SubGrid subGrid )
        {
            VertexElement[] elems = WaterDMapVertex.CreateVertexElements();

            //Mesh subMesh = new Mesh( subGrid.NumTris, subGrid.NumVerts, MeshFlags.Managed | MeshFlags.Use32Bit, elems, mGDevice );
            Mesh subMesh = new Mesh( game, subGrid.NumTris, subGrid.NumVerts, WaterDMapVertex.StrideSize
                , Graphics.AttributeSystem.CreateVertexDeclaration( game.GraphicsDevice, typeof( WaterDMapVertex ) ) );


            #region set subgrid verts & generate the bounding box
            WaterDMapVertex[] v = new WaterDMapVertex[ subMesh.NumberVertices ];

            Vector3[] positions = new Vector3[ subMesh.NumberVertices ];

            //use a stream so we can pass it to computeBoundingBox
            //GraphicsStream vertexStream = subMesh.LockVertexBuffer( LockFlags.None );

            int k = 0;
            for ( int i = R.Top; i <= R.Bottom; i++ )
            {
                for ( int j = R.Left; j <= R.Right; j++ )
                {
                    v[ k ] = gridVerts[ i * mInfo.vertCols + j ];
                    positions[ k ] = gridVerts[ i * mInfo.vertCols + j ].pos;
                    k++;
                }
            }

            //vertexStream.Write( v );

            Utils.BoundingBox bndBox = new Utils.BoundingBox();

            subMesh.SetVertexBufferData( v );

            BoundingBox bb = BoundingBox.CreateFromPoints( positions );
            bndBox.min = bb.Min;
            bndBox.max = bb.Max;


            //Geometry.ComputeBoundingBox( vertexStream, subMesh.NumberVertices, subMesh.VertexFormat, out bndBox.min, out bndBox.max );

            //subMesh.UnlockVertexBuffer();

            subGrid.box = bndBox;

            if ( buildMesh == false )
            {
                subMesh.Dispose();
                subMesh = null;
                return;
            }

            #endregion

            #region generate Indices for the subgrid and Optimize
            Vector3[] tempVerts;
            int[] tempIndices;

            Utils.GenTriGrid( subGrid.NumRows, subGrid.NumCols, mInfo.dx, mInfo.dz,
                new Vector3( 0, 0, 0 ), out tempVerts, out tempIndices );

            /*int[] attributes = subMesh.LockAttributeBufferArray( LockFlags.None );
            for ( int i = 0; i < subGrid.NumTris; i++ )
            {
                attributes[ i ] = 0; // All in subset 0.
            }

            subMesh.UnlockAttributeBuffer();*/
            subMesh.SetIndexBufferData( tempIndices );

            int[] adjacency = new int[ subMesh.NumberFaces * 3 ];
            subMesh.GenerateAdjacency( .001f, adjacency );
            //subMesh.OptimizeInPlace( MeshFlags.OptimizeVertexCache | MeshFlags.OptimizeAttributeSort, adjacency );
            subMesh.OptimizeInPlace( 0, adjacency );
            #endregion

            subGrid.mesh = subMesh;
        }

        private void recursiveTerrainBuild( SubGrid node, Rectangle R, WaterDMapVertex[] gridVerts )
        {
            if ( R.Width <= 32 )
            {
                numLeaves++;
                buildSubGridMesh( R, gridVerts, true, node );
                node.IsLeaf = true;

                int[] indices = new int[ node.NumTris * 3 ];

                int k = 0;
                for ( int i = R.Top; i < R.Bottom; i++ )
                {
                    for ( int j = R.Left; j < R.Right; j++ )
                    {
                        indices[ k ] = ( i * mInfo.vertCols + j );
                        indices[ k + 1 ] = ( i * mInfo.vertCols + j + 1 );
                        indices[ k + 2 ] = ( ( i + 1 ) * mInfo.vertCols + j );

                        indices[ k + 3 ] = ( ( i + 1 ) * mInfo.vertCols + j );
                        indices[ k + 4 ] = ( i * mInfo.vertCols + j + 1 );
                        indices[ k + 5 ] = ( ( i + 1 ) * mInfo.vertCols + j + 1 );

                        // next quad
                        k += 6;
                    }
                }

                return;
            }

            buildSubGridMesh( R, gridVerts, false, node );

            int newWidth = R.Width / 2;
            int newHeight = R.Height / 2;
            int newSubRows = R.Width / newWidth;
            int newSubCols = R.Height / newHeight;

            int index = 0;
            ///  SubGrid        r | c
            ///  Top-Left    :  0   0
            ///  Top-Right   :  0   1
            ///  Bottom-Left :  1   0
            ///  Bottom-Right:  1   1
            ///  
            for ( int r = 0; r < newSubRows; r++ )
            {
                for ( int c = 0; c < newSubCols; c++ )
                {
                    Rectangle rec = new Rectangle(
                                        R.Left + c * ( newWidth ),
                                        R.Top + r * ( newHeight ),
                                        ( newWidth ),
                                        ( newHeight ) );

                    node.children[ index ] = new SubGrid( newWidth + 1, newHeight + 1 );

                    recursiveTerrainBuild( node.children[ index ], rec, gridVerts );

                    index++;
                }
            }
        }

        private void recursiveFrustumCheck( SubGrid node, Camera camera )
        {
            //game.LineManager3D.AddBox( new BoundingBox( node.box.min, node.box.max ), Color.Red );
            //game.LineManager3D.AddViewFrustum( new BoundingFrustum( camera.ViewProj ), Color.Green );
            if ( !camera.IsBoundingBoxVisible( new BoundingBox( node.box.min, node.box.max ) ) )
                return;

            if ( node.IsLeaf )
            {
                node.camPos = camera.Position;
                mVisibleSubgrids.Add( node );
            }

            for ( int i = 0; i < 4; i++ )
            {
                if ( node.children[ i ] != null )
                    recursiveFrustumCheck( node.children[ i ], camera );
            }
        }

        public void buildEffect( IXNAGame _game, TextureCube envMap )
        {
            string errorMessage;
            if ( mEffect != null ) mEffect.Dispose();
            mEffect = BasicShader.LoadFromFXFile( _game, new GameFile( mInfo.fxFilename ) );

            /*if ( errorMessage != null && errorMessage != "" )
            {
                Cursor.Show();
                MessageBox.Show( errorMessage, "Error Building Effect" );
            }*/

            mTechHandle = mEffect.GetTechnique( "WaterTech" );
            mWorldHandle = mEffect.GetParameter( "gWorld" );
            mWorldInvHandle = mEffect.GetParameter( "gWorldInv" );
            mWVPHandle = mEffect.GetParameter( "gWVP" );
            mEyePosHandle = mEffect.GetParameter( "gEyePosW" );
            mSunDirHandle = mEffect.GetParameter( "gSunDir" );
            mSunColorHandle = mEffect.GetParameter( "gSunColor" );
            mWaveMap0Handle = mEffect.GetParameter( "gWaveMap0" );
            mWaveMap1Handle = mEffect.GetParameter( "gWaveMap1" );
            mWaveNMapOffset0Handle = mEffect.GetParameter( "gWaveNMapOffset0" );
            mWaveNMapOffset1Handle = mEffect.GetParameter( "gWaveNMapOffset1" );
            mReflectMapHandle = mEffect.GetParameter( "gReflectMap" );
            mRefractMapHandle = mEffect.GetParameter( "gRefractMap" );

            mWorldHandle.SetValue( mInfo.toWorld );
            Matrix invWorld = Matrix.Invert( mInfo.toWorld );
            mWorldInvHandle.SetValue( invWorld );

            mEffect.effect.CurrentTechnique = mTechHandle;
            mWaveMap0Handle.SetValue( mWaveMap0 );
            mWaveMap1Handle.SetValue( mWaveMap1 );

            float[] sunDir = new float[] { mInfo.sunDirection.X, mInfo.sunDirection.Y, mInfo.sunDirection.Z };
            mSunDirHandle.SetValue( sunDir );
        }


        public delegate void RenderDelegate( IXNAGame game );



        public void BuildRefractMap( Camera mCamera, RenderDelegate renderDelegate )
        {
            DepthStencilBuffer oldDS = game.GraphicsDevice.DepthStencilBuffer;
            game.GraphicsDevice.DepthStencilBuffer = DepthBuffer;
            mGDevice.RenderState.DepthBufferWriteEnable = true;
            mGDevice.RenderState.DepthBufferEnable = true;
            //mGDevice.SetRenderState(RenderStates.ClipPlaneEnable, true);
            //mGDevice.RenderState.CullMode = Cull.CounterClockwise;

            game.GraphicsDevice.SetRenderTarget( 0, RefractMap );
            //RefractMap.BeginScene();
            mGDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, new Color( (int)( 255 * .13f ), (int)( 255 * .19f ), (int)( 255 * .22f ) ), 1.0f, 0 );

            //reflection plane in local space
            //Plane waterPlaneL = new Plane( 0.0f, -1.0f, 0.0f, 2.5f * 3.0f );
            Plane waterPlaneL = new Plane( 0.0f, -1.0f, 0.0f, 2.5f * 3.0f );

            //Matrix wInvTrans = Matrix.Invert( WorldMatrix );
            //wInvTrans = Matrix.Transpose( wInvTrans );


            //reflection plane in world space
            //Plane waterPlaneW = Plane.Transform( waterPlaneL, wInvTrans );

            Matrix wvpInvTrans = Matrix.Invert( WorldMatrix * mCamera.View * mCamera.Projection );
            wvpInvTrans = Matrix.Transpose( wvpInvTrans );

            //reflection plane in homogeneous space
            Plane waterPlaneH = Plane.Transform( waterPlaneL, wvpInvTrans );
            //waterPlaneH.Normalize();


            // MHGWEdit: i have no clue why to use the inverse and transformed viewproj matrix, so i dont and it works.

            //if we're below the water line, don't perform clipping.
            //this allows us to see the distorted objects from under the water
            if ( mCamera.Position.Y >= 0.0f )
            {
                mGDevice.ClipPlanes[ 0 ].IsEnabled = true;
                mGDevice.ClipPlanes[ 0 ].Plane = Plane.Transform( waterPlaneL, WorldMatrix * mCamera.ViewProj );// waterPlaneH;
            }

            //( (XNAGame)game ).Window.Title = waterPlaneH.Normal.ToString() + "    " + waterPlaneH.D.ToString();


            //CustomCamera cam = new CustomCamera( game );
            //cam.SetViewProjectionMatrix( mCamera.View, mCamera.Projection );
            //game.SetCamera( cam );

            //render any scene objects
            renderDelegate( game );
            /*if ( mCamera.Position.Y < 0.0f )
                mSkyModel.Render( mGDevice, mCamera, default( Plane ) );

            drawTerrain( true, default( Plane ) );*/


            mGDevice.ClipPlanes.DisableAll();
            //mGDevice.SetRenderState( RenderStates.ClipPlaneEnable, false );
            //mGDevice.RenderState.CullMode = Cull.CounterClockwise;
            game.GraphicsDevice.SetRenderTarget( 0, null );
            game.GraphicsDevice.DepthStencilBuffer = oldDS;
        }

        public void BuildReflectMap( Camera mCamera, RenderDelegate renderDelegate )
        {
            DepthStencilBuffer oldDS = game.GraphicsDevice.DepthStencilBuffer;
            game.GraphicsDevice.DepthStencilBuffer = DepthBuffer;


            /*------------------------------------------------------------------------------------------
             * Render to the Reflection Map
             */

            //mGDevice.SetRenderState( RenderStates.ClipPlaneEnable, true );
            mGDevice.RenderState.CullMode = CullMode.CullClockwiseFace; // Cull.Clockwise;

            mGDevice.SetRenderTarget( 0, mReflectMap );
            //ReflectMap.BeginScene();
            //ColorValue color = new ColorValue( 0, 0, 0, 0 );
            mGDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer,
                            new Color( (int)( 255 * .13f ), (int)( 255 * .19f ), (int)( 255 * .22f ) ), 1.0f, 0 );
            //mGDevice.Clear( ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb( (int)( 255 * .13f ), (int)( 255 * .19f ), (int)( 255 * .22f ) ), 1.0f, 0 );

            //reflection plane in local space
            //Plane waterPlaneL = new Plane( 0.0f, -1.0f, 0.0f, 2.7f * 1.5f );
            Plane waterPlaneL = new Plane( 0.0f, -1.0f, 0.0f, 1f * 1.5f );

            Matrix wInvTrans = Matrix.Invert( WorldMatrix );
            wInvTrans = Matrix.Transpose( wInvTrans );

            //reflection plane in world space
            Plane waterPlaneW = Plane.Transform( waterPlaneL, wInvTrans );

            Matrix wvpInvTrans = Matrix.Invert( WorldMatrix * mCamera.ViewProj );
            wvpInvTrans = Matrix.Transpose( wvpInvTrans );

            //reflection plane in homogeneous space
            Plane waterPlaneH = Plane.Transform( waterPlaneL, wvpInvTrans );

            mGDevice.ClipPlanes[ 0 ].IsEnabled = true;

            //no clue why to use this
            //mGDevice.ClipPlanes[ 0 ].Plane = waterPlaneH;
            mGDevice.ClipPlanes[ 0 ].Plane = Plane.Transform( waterPlaneL, WorldMatrix * mCamera.ViewProj );

            Matrix reflMatrix = Matrix.Identity;

            reflMatrix = Matrix.CreateReflection( waterPlaneW );

            //mTerrain.ReflectionPass = true;


            ICamera oldCam = game.Camera;
            CustomCamera cam = new CustomCamera( game );
            cam.SetViewProjectionMatrix( reflMatrix * mCamera.View, mCamera.Projection );
            game.SetCamera( cam );


            //render scene objects
            renderDelegate( game );

            game.SetCamera( oldCam );
            /*mSkyModel.Render( mGDevice, mCamera, waterPlaneW );
            drawTerrain( false, waterPlaneW );

            mWater.ReflectMap.EndScene();*/

            mGDevice.ClipPlanes.DisableAll();
            //mGDevice.SetRenderState( RenderStates.ClipPlaneEnable, false );
            //mGDevice.RenderState.CullMode = Cull.CounterClockwise;
            mGDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace; // Cull.Clockwise;
            game.GraphicsDevice.SetRenderTarget( 0, null );

            game.GraphicsDevice.DepthStencilBuffer = oldDS;
        }

        public void BuildMaps( Camera mCamera, RenderDelegate renderDelegate )
        {

            BuildRefractMap( mCamera, renderDelegate );
            BuildReflectMap( mCamera, renderDelegate );
        }
    }
}
