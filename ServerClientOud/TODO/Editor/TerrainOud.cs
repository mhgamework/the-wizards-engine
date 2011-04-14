using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Texture = MHGameWork.TheWizards.ServerClient.Engine.Texture;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.GeoMipMap;
namespace MHGameWork.TheWizards.ServerClient.Editor
{
    /// <summary>
    /// CMHGW001
    /// </summary>
    public class TerrainOud : IGameObject2
    {
        private XNAGeoMipMap.Terrain baseTerrain;

        public XNAGeoMipMap.Terrain BaseTerrain
        {
            get { return baseTerrain; }
            //set { baseTerrain = value; }
        }

        public TerrainBlock[][] blocks;




        //private EffectParameter viewInverseParam;
        //private EffectParameter lightDirParam;
        //private EffectParameter texture1Param;
        //private EffectParameter texture2Param;
        //private EffectParameter texture3Param;
        //private EffectParameter texture4Param;
        //private EffectParameter brushTextureParam;
        //private EffectParameter weightMapParam;

        private List<Texture2D> weightMaps = new List<Texture2D>();








        private Engine.ShaderEffect editorEffect;
        private EffectTechnique techniqueColoredHeights;
        private EffectParameter paramWorld;
        private EffectParameter paramWorldViewProjection;
        private EffectParameter paramBrush;
        private EffectParameter paramBrushTexture;
        private EffectParameter paramWeightMap;
        private EffectParameter paramTexture1;
        private EffectParameter paramTexture2;
        private EffectParameter paramTexture3;
        private EffectParameter paramTexture4;

        private Brush brush;
        private Engine.Texture brushTexture;

        private Common.GeoMipMap.HeightMapOud heightMap;

        public MHGameWork.TheWizards.Common.GeoMipMap.HeightMapOud HeightMap
        {
            get { return heightMap; }
            set { heightMap = value; }
        }


        private List<TerrainTexture> textures = new List<TerrainTexture>();

        public List<TerrainTexture> Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        private List<TerrainMaterial> viewMaterials = new List<TerrainMaterial>();

        public List<TerrainMaterial> ViewMaterials
        {
            get { return viewMaterials; }
            set { viewMaterials = value; }
        }

        private List<Texture2D> viewWeightmaps = new List<Texture2D>();

        public List<Texture2D> ViewWeightmaps
        {
            get { return viewWeightmaps; }
            set { viewWeightmaps = value; }
        }



        public Brush Brush
        {
            get { return brush; }
            set { brush = value; }
        }
        public Engine.Texture BrushTexture
        {
            get { return brushTexture; }
            set { brushTexture = value; }
        }
        //private EffectParameter worldViewProjectionParam;
        //private EffectParameter cameraPositionParam;
        //private EffectParameter lightAngleParam;
        //private EffectParameter ambientParam;
        //private EffectParameter diffuseParam;


        //New version

        //private new HeightMap heightMap;
        //private new LightMap lightMap;
        //private new WeightMap weightMap;


        //private Texture[] textures;
        //private Texture2D lightMapTexture;
        //private Texture2D weightMapTexture;

        //private VertexDeclaration vertexDeclaration;
        //public Engine.ShaderEffect effect;


        public TerrainOud( XNAGeoMipMap.Terrain nBaseTerrain )
        {
            baseTerrain = nBaseTerrain;
        }



        #region IGameObject2 Members

        public void Initialize()
        {
            if ( blocks == null )
            {
                //Do a full load

                BuildBlocks();


                //TODO: Should load a heightmap actually

                if ( BaseTerrain.TerrainInfo.HeightMapFileID == -1 )
                {
                    //Maybe temporary, but there is no heightmap yet, so create a new one.
                    heightMap = new HeightMapOud( SizeX, SizeY );
                }
                else
                {
                    heightMap = new HeightMapOud( SizeX, SizeY, Engine.GameFileManager.GetGameFile( BaseTerrain.TerrainInfo.HeightMapFileID ).GetFullFilename() );
                }

            }

            //device = engine.XNAGame.Graphics.GraphicsDevice;
            //content = engine.XNAGame._content;

            //effect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( engine, @"Content\TerrainEditor.fx" );



        }

        public void LoadGraphicsContent()
        {
            if ( editorEffect == null )
            {
                //Load all data

                for ( int x = 0; x < NumBlocksX; x++ )
                {
                    for ( int z = 0; z < NumBlocksY; z++ )
                    {
                        GetEditorBlock( x, z ).BuildVertexBufferFromHeightmap( heightMap );
                        //GetBlock(x,z).LightMap = lightmapGen.Generate( device, x * blockSize, z * blockSize );
                    }
                }
            }


            if ( editorEffect != null ) editorEffect.Dispose();
            //Always reload effect

            //TODO: editorEffect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( Engine, baseTerrain.Content.RootDirectory + @"\Content\TerrainEditor.fx" );
            techniqueColoredHeights = editorEffect.GetTechnique( "ColoredHeights" );
            paramWorld = editorEffect.Effect.Parameters[ "World" ];
            paramWorldViewProjection = editorEffect.Effect.Parameters[ "WorldViewProjection" ];
            paramBrush = editorEffect.Effect.Parameters[ "Brush" ];
            paramBrushTexture = editorEffect.Effect.Parameters[ "BrushTexture" ];

            paramWeightMap = editorEffect.Effect.Parameters[ "WeightMap" ];
            paramTexture1 = editorEffect.Effect.Parameters[ "Texture1" ];
            paramTexture2 = editorEffect.Effect.Parameters[ "Texture2" ];
            paramTexture3 = editorEffect.Effect.Parameters[ "Texture3" ];
            paramTexture4 = editorEffect.Effect.Parameters[ "Texture4" ];

            //if ( effect.Effect != null )
            //{
            //    effect.Effect.Parameters[ "xLightMap" ].SetValue( lightMapTexture );
            //    weightMapParam = effect.Effect.Parameters[ "xWeightMap" ]; //.SetValue( weightMapTexture );

            //    lightDirParam = effect.Effect.Parameters[ "LightDir" ];

            //    texture1Param = effect.Effect.Parameters[ "Texture1" ];
            //    texture2Param = effect.Effect.Parameters[ "Texture2" ];
            //    texture3Param = effect.Effect.Parameters[ "Texture3" ];
            //    texture4Param = effect.Effect.Parameters[ "Texture4" ];
            //    brushTextureParam = effect.Effect.Parameters[ "BrushTexture" ];

            //    viewInverseParam = effect.Effect.Parameters[ "ViewInverse" ];
            //    worldParam = effect.Effect.Parameters[ "World" ];
            //    brushParam = effect.Effect.Parameters[ "Brush" ];
            //    worldViewProjectionParam = effect.Effect.Parameters[ "WorldViewProjection" ];
            //    cameraPositionParam = effect.Effect.Parameters[ "CameraPosition" ];
            //    lightAngleParam = effect.Effect.Parameters[ "LightAngle" ];
            //    ambientParam = effect.Effect.Parameters[ "Ambient" ];
            //    diffuseParam = effect.Effect.Parameters[ "Diffuse" ];


            //    effect.Effect.CurrentTechnique = effect.Effect.Techniques[ "TerrainWireframe" ];
            //}

            //if ( vertexDeclaration != null )
            //    vertexDeclaration.Dispose();
            //if ( lightMapTexture != null ) lightMapTexture.Dispose();
            //lightMapTexture = null;


            //vertexDeclaration = new VertexDeclaration( device, XNAGeoMipMap.VertexMultitextured.VertexElements );





            ////Dit is voor in een editor
            //if ( lightMap != null )
            //{
            //    lightMapTexture = new Texture2D( device, sizeX, SizeY, 1, TextureUsage.None, SurfaceFormat.Luminance8 );
            //    lightMapTexture.SetData<byte>( lightMap.Data );
            //}

            //GenerateWeightMaps();



            ///*textures[ 0 ] = Content.Load<Texture2D>( @"Content\Grass" );
            //textures[ 1 ] = Content.Load<Texture2D>( @"Content\sand" );
            //textures[ 2 ] = Content.Load<Texture2D>( @"Content\rock" );
            //textures[ 3 ] = Content.Load<Texture2D>( @"Content\snow" );*/
            ////lightMap = Texture2D.FromFile( device, Content.RootDirectory + @"\Content\Terrain\Lightmap.dds" );
            ////weightTexture = Texture2D.FromFile( device, Content.RootDirectory + @"\Content\Terrain\WeightMap.dds" );



            ////LoadFromDisk();



            //// initialize blocks


            //////TODO: waarom moet bij elke loadgraphicscontent?
            ////if ( trunk != null )
            ////    RebuildBounding( trunk );



            ////SaveToDisk();



            ////base.LoadGraphicsContent( loadAllContent );
        }

        public void UnloadGraphicsContent()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public void UnInitialize()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState OUDInitializeTask( Engine.LoadingTaskType taskType )
        {
            throw new Exception( "The method or operation is not implemented." );

            //if ( blocks == null )
            //{
            //    BuildBlocks();

            //    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            //}




            //return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState LoadGraphicsContentTask( Engine.LoadingTaskType taskType )
        {
            throw new Exception( "The method or operation is not implemented." );
            //return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState UnloadGraphicsContentTask( Engine.LoadingTaskType taskType )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState UnInitializeTask( Engine.LoadingTaskType taskType )
        {
            throw new Exception( "The method or operation is not implemented." );
        }


        public void OUDProcess( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public void OUDRender()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        /// <summary>
        /// Loads the blocks array based on the baseTerrain's blocks array
        /// </summary>
        public void BuildBlocks()
        {
            blocks = new TerrainBlock[ NumBlocksX ][];

            // create blocks
            for ( int x = 0; x < NumBlocksX; x++ )
            {
                blocks[ x ] = new TerrainBlock[ NumBlocksY ];

                for ( int z = 0; z < NumBlocksY; z++ )
                {
                    TerrainBlock patch = CreateBlock( baseTerrain.GetBlock( x, z ) );

                    blocks[ x ][ z ] = patch;

                    if ( z > 0 )
                    {
                        patch.North = blocks[ x ][ z - 1 ];
                        blocks[ x ][ z - 1 ].South = patch;
                    }

                    if ( x > 0 )
                    {
                        patch.West = blocks[ x - 1 ][ z ];
                        blocks[ x - 1 ][ z ].East = patch;
                    }
                }
            }

        }

        public TerrainBlock CreateBlock( XNAGeoMipMap.TerrainBlock baseBlock )
        {
            return new TerrainBlock( this, baseBlock );
        }







        public void RenderEditHeightsMode()
        {
            //Statistics.Reset();




            //device.RenderState.AlphaBlendEnable = false;


            //device.RenderState.FogStart = 3000f;
            //device.RenderState.FogEnd = 4000f - 2;
            //device.RenderState.FogTableMode = FogMode.None;
            //device.RenderState.FogVertexMode = FogMode.Exponent;
            //device.RenderState.FogDensity = 1f;
            //device.RenderState.FogColor = Color.CornflowerBlue;
            ////device.RenderState.FogEnable = true;
            //device.RenderState.FogEnable = false;
            //device.VertexDeclaration = vertexDeclaration;



            ///*device.Textures[ 0 ] = lightMap;
            //device.Textures[ 1 ] = weightTexture;

            //device.SamplerStates[ 0 ].AddressU = TextureAddressMode.Clamp;
            //device.SamplerStates[ 0 ].AddressV = TextureAddressMode.Clamp;
            //device.SamplerStates[ 0 ].AddressW = TextureAddressMode.Clamp;
            //device.SamplerStates[ 0 ].MinFilter = TextureFilter.Linear;
            //device.SamplerStates[ 0 ].MagFilter = TextureFilter.Linear;
            //device.SamplerStates[ 0 ].MipFilter = TextureFilter.Linear;

            //device.SamplerStates[ 1 ].AddressU = TextureAddressMode.Clamp;
            //device.SamplerStates[ 1 ].AddressV = TextureAddressMode.Clamp;
            //device.SamplerStates[ 1 ].AddressW = TextureAddressMode.Clamp;
            //device.SamplerStates[ 1 ].MinFilter = TextureFilter.Linear;
            //device.SamplerStates[ 1 ].MagFilter = TextureFilter.Linear;
            //device.SamplerStates[ 1 ].MipFilter = TextureFilter.Linear;*/


            //for ( int i = 2; i < 6; i++ )
            //{
            //    device.SamplerStates[ i ].AddressU = TextureAddressMode.Mirror;
            //    device.SamplerStates[ i ].AddressV = TextureAddressMode.Mirror;
            //    device.SamplerStates[ i ].AddressW = TextureAddressMode.Mirror;
            //    device.SamplerStates[ i ].MinFilter = TextureFilter.Anisotropic;
            //    device.SamplerStates[ i ].MagFilter = TextureFilter.Anisotropic;
            //    device.SamplerStates[ i ].MipFilter = TextureFilter.Linear;
            //}




            ////cameraPositionParam.SetValue( Camera.ActiveCamera.Position );
            ////worldViewProjectionParam.SetValue( Camera.ActiveCamera.View * Camera.ActiveCamera.Projection );

            ////effect.Effect.Parameters[ "CameraPosition" ].SetValue( engine.ActiveCamera.CameraPosition );

            //brushParam.SetValue( new Vector4( brush.Position, brush.Range ) );

            //cameraPositionParam.SetValue( engine.ActiveCamera.CameraPosition );
            //worldViewProjectionParam.SetValue(
            //    worldMatrix
            //    * engine.ActiveCamera.CameraInfo.ViewMatrix
            //    * engine.ActiveCamera.CameraInfo.ProjectionMatrix );

            Device.RenderState.AlphaBlendEnable = false;
            Device.RenderState.AlphaTestEnable = false;

            Device.VertexDeclaration = BaseTerrain.VertexDeclaration;


            paramWorld.SetValue( baseTerrain.WorldMatrix );
            paramWorldViewProjection.SetValue( baseTerrain.WorldMatrix * Engine.ActiveCamera.CameraInfo.ViewProjectionMatrix );

            paramBrush.SetValue( new Vector4( brush.Position, brush.Range ) );


            editorEffect.Effect.CurrentTechnique = techniqueColoredHeights;
            editorEffect.Effect.Begin();

            //visibleTriangles = 0;

            for ( int i = 0; i < editorEffect.Effect.CurrentTechnique.Passes.Count; i++ )
            {
                EffectPass pass = editorEffect.Effect.CurrentTechnique.Passes[ i ];

                pass.Begin();
                editorEffect.Effect.CommitChanges();


                //    if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.W ) )
                //    {

                //        //if ( ServerClientMain.instance.XNAGame.Graphics.device.RenderState.FillMode == FillMode.Solid )
                //        //{ device.RenderState.FillMode = FillMode.WireFrame; }
                //        //else { ServerClientMain.instance.XNAGame.Graphics.device.RenderState.FillMode = FillMode.Solid; }
                //    }


                //    if ( quadTreeNode != null )
                //        //visibleTriangles += DrawTreeNode( HoofdObject.ActiveCamera.CameraInfo.Frustum, trunk );
                //        visibleTriangles += DrawTreeNode( quadTreeNode );
                baseTerrain.DrawTreeNode( baseTerrain.QuadTreeNode );

                pass.End();
            }

            editorEffect.Effect.End();
            //device.RenderState.FogEnable = false;

        }























        public bool Raycast( Ray ray, out Vector3 hit )
        {
            hit = Vector3.Zero;


            //Ray current = ray;
            Ray localRay = ray;
            localRay.Position = Vector3.Transform( ray.Position, Matrix.Invert( baseTerrain.WorldMatrix ) );
            float step = 1f;
            Vector3 stepVector = ray.Direction * step;
            float threshold = 2f;
            bool inTerrainBounding = false;

            while ( true )
            {
                if ( baseTerrain.QuadTreeNode.BoundingBox.Intersects( ray ) == null ) return false;
                if ( baseTerrain.QuadTreeNode.BoundingBox.Contains( ray.Position ) == ContainmentType.Contains )
                {
                    inTerrainBounding = true;
                    float dist = (float)Math.Abs( localRay.Position.Y - heightMap.CalculateHeight( localRay.Position.X, localRay.Position.Z ) );
                    if ( dist < threshold )
                    {
                        hit = new Vector3( ray.Position.X, heightMap.CalculateHeight( localRay.Position.X, localRay.Position.Z ), ray.Position.Z );
                        return true;
                    }
                }
                else
                {
                    if ( inTerrainBounding )
                    {

                        return false;
                    }
                }

                ray.Position += stepVector;
                localRay.Position += stepVector;

            }


        }


        public void RaiseTerrain( Vector2 position, float range, float height )
        {
            Vector3 transformed = Vector3.Transform( new Vector3( position.X, 0, position.Y ), Matrix.Invert( WorldMatrix ) );
            Vector2 localPosition = new Vector2( transformed.X, transformed.Z );









            int minX = (int)Math.Floor( localPosition.X - range );
            int maxX = (int)Math.Floor( localPosition.X + range ) + 1;
            int minZ = (int)Math.Floor( localPosition.Y - range );
            int maxZ = (int)Math.Floor( localPosition.Y + range ) + 1;

            minX = (int)MathHelper.Clamp( minX, 0, SizeX - 1 );
            maxX = (int)MathHelper.Clamp( maxX, 0, SizeX - 1 );
            minZ = (int)MathHelper.Clamp( minZ, 0, SizeY - 1 );
            maxZ = (int)MathHelper.Clamp( maxZ, 0, SizeY - 1 );


            int areaSizeX = maxX - minX + 1;
            int areaSizeZ = maxZ - minZ + 1;

            //Rectangle rect = new Rectangle( minX, minZ, areaSizeX, areaSizeZ );

            //Color[] data = new Color[ ( areaSizeX ) * ( areaSizeZ ) ];

            //weightMapTexture.GetData<Color>( 0, rect, data, 0, data.Length );

            //float rangeSq = range * range;
            //x -= minX;
            //z -= minZ;

            for ( int x = minX; x <= maxX; x++ )
            {
                for ( int y = minZ; y <= maxZ; y++ )
                {


                    //for ( int y = 0; y < heightMap.Length; y++ )
                    //{
                    //    for ( int x = 0; x < heightMap.Width; x++ )
                    //    {
                    float dist = Vector2.Distance( localPosition, new Vector2( x, y ) );
                    if ( dist <= range )
                    {
                        float factor = 1 - dist / range;

                        heightMap.SetHeight( x, y, heightMap.GetHeight( x, y ) + height * factor );
                    }
                }

            }

            SetDirtyBlocks( BaseTerrain.QuadTreeNode, new BoundingSphere( new Vector3( position.X, 0, position.Y ), range ), false );

        }

        public void SetDirtyBlocks( Wereld.QuadTreeNode node, BoundingSphere sphere, bool skipBoundingCheck )
        {
            if ( skipBoundingCheck != true )
            {
                BoundingBox box = node.EntityBoundingBox;
                box.Min.Y = 0;
                box.Max.Y = 0.1f;
                ContainmentType containment = sphere.Contains( box );

                if ( containment == ContainmentType.Disjoint )
                    return;

                // if the entire node is contained within, then assume all children are as well
                if ( containment == ContainmentType.Contains )
                    skipBoundingCheck = true;
            }



            if ( node.TerrainBlock == null )
            {
                if ( node.UpperLeft != null )
                    SetDirtyBlocks( node.UpperLeft, sphere, skipBoundingCheck );

                if ( node.UpperRight != null )
                    SetDirtyBlocks( node.UpperRight, sphere, skipBoundingCheck );

                if ( node.LowerLeft != null )
                    SetDirtyBlocks( node.LowerLeft, sphere, skipBoundingCheck );

                if ( node.LowerRight != null )
                    SetDirtyBlocks( node.LowerRight, sphere, skipBoundingCheck );
            }
            else
            {
                TerrainBlock block = GetEditorBlock( node.TerrainBlock );
                block.SetVertexBufferDirty();
                block.SetHeightMapDirty();
            }
        }







        public void PickTerrain( GraphicsDevice device, Point mousecoords, bool Up )
        {
            throw new Exception( "TODO" );
            //RCCameras.RCCamera camera = RCCameras.RCCameraManager.ActiveCamera;

            //Vector3 nearSource = camera.Viewport.Unproject( new Vector3( mousecoords.X, mousecoords.Y, camera.Viewport.MinDepth ), camera.Projection, camera.View, Matrix.Identity );
            //Vector3 farSource = camera.Viewport.Unproject( new Vector3( mousecoords.X, mousecoords.Y, camera.Viewport.MaxDepth ), camera.Projection, camera.View, Matrix.Identity );
            //Vector3 direction = farSource - nearSource;

            //float zFactor = nearSource.Y / direction.Y;
            //Vector3 zeroWorldPoint = nearSource + direction * zFactor;

            //Ray ray = new Ray( nearSource, direction );

            //for ( int x = 0; x < myWidth; x++ )
            //    for ( int y = 0; y < myHeight; y++ )
            //    {
            //        BoundingBox tmp = new BoundingBox( myVertices[ x + y * myWidth ].Position + myPosition, ( myVertices[ x + y * myWidth ].Position + myPosition ) + new Vector3( 1f, 1f, 1f ) );

            //        if ( ray.Intersects( tmp ) != null )
            //        {
            //            float val = 0;
            //            if ( Up )
            //                val += .5f;
            //            else
            //                val -= .5f;

            //            if ( x > 0 )
            //                myHeightData[ x - 1, y ] += val;

            //            myHeightData[ x, y ] += val;

            //            if ( x < myWidth )
            //                myHeightData[ x + 1, y ] += val;

            //            if ( x > 0 && y > 0 )
            //                myHeightData[ x - 1, y - 1 ] += val;

            //            if ( y > 0 )
            //                myHeightData[ x, y - 1 ] += val;
            //            if ( x < myWidth && y > 0 )
            //                myHeightData[ x + 1, y - 1 ] += val;

            //            if ( x > 0 && y < myHeight )
            //                myHeightData[ x - 1, y + 1 ] += val;
            //            if ( y < myHeight )
            //                myHeightData[ x, y + 1 ] += val;
            //            if ( x < myWidth && y < myHeight )
            //                myHeightData[ x + 1, y + 1 ] += val;

            //            break;
            //        }
            //    }

            //BuildTerrain( device );
        }

















        protected void Dispose( bool disposing )
        {
            //base.Dispose( disposing );
            // lock (this) ???

            //if ( disposing )
            //{
            //    DisposeTerrain();

            //    if ( vertexDeclaration != null )
            //        vertexDeclaration.Dispose();
            //    vertexDeclaration = null;


            //}


        }


        public void BuildVerticesFromHeightmap()
        {
            //for ( int x = 0; x < numBlocksX; x++ )
            //{

            //    for ( int z = 0; z < numBlocksY; z++ )
            //    {
            //        ( (TerrainBlock)blocks[ x ][ z ] ).BuildVertexBufferFromHeightmap();

            //    }
            //}
        }


        //protected MHGameWork.TheWizards.Common.GeoMipMap.TerrainBlock CreateBlock( int x, int z )
        //{
        //    return new TerrainBlock( this, x, z );
        //}







        public void UpdateDirtyVertexbuffers()
        {
            for ( int x = 0; x < NumBlocksX; x++ )
            {
                for ( int z = 0; z < NumBlocksY; z++ )
                {
                    if ( GetEditorBlock( x, z ).VertexBufferDirty )
                    {
                        GetEditorBlock( x, z ).BuildVertexBufferFromHeightmap( heightMap );
                        GetEditorBlock( x, z ).CalculateMinDistances( heightMap, BaseTerrain.Engine.ActiveCamera.CameraInfo.ProjectionMatrix );



                    }

                }
            }
        }

        public void UpdateDirtyServerBlocks()
        {
            return;
            /*List<int[]> blocksData = new List<int[]>();
            for ( int x = 0; x < NumBlocksX; x++ )
            {
                for ( int z = 0; z < NumBlocksY; z++ )
                {
                    if ( GetEditorBlock( x, z ).ServerDirty )
                    {
                        TerrainBlock block = GetEditorBlock( x, z );

                        int[] data = new int[ 3 ];

                        data[ 0 ] = x;
                        data[ 1 ] = z;
                        data[ 2 ] = block.BaseBlock.Version + 1;
                        blocksData.Add( data );




                    }

                }
            }

            int[][] newBlocksData = Engine.ServerMain.Wereld.TerrainManager.Terrains[ 0 ].SetBlocksChanged( blocksData.ToArray() );

            for ( int i = 0; i < newBlocksData.Length; i++ )
            {
                TerrainBlock block = GetEditorBlock( newBlocksData[ i ][ 0 ], newBlocksData[ i ][ 1 ] );

                block.BaseBlock.Version = newBlocksData[ i ][ 2 ];


                block.ServerDirty = false;


            }*/

        }

        public void UpdateDirtyHeightMapBlocks()
        {
            for ( int x = 0; x < NumBlocksX; x++ )
            {
                for ( int z = 0; z < NumBlocksY; z++ )
                {
                    if ( GetEditorBlock( x, z ).HeightMapDirty )
                    {
                        //Sync

                        TerrainBlock block = GetEditorBlock( x, z );

                        float[] data = new float[ BaseTerrain.BlockSize * BaseTerrain.BlockSize ];
                        for ( int ix = block.BaseBlock.X; ix < block.BaseBlock.X + BaseTerrain.BlockSize; ix++ )
                        {
                            for ( int iz = block.BaseBlock.Z; iz < block.BaseBlock.Z + BaseTerrain.BlockSize; iz++ )
                            {
                                int relX = ix - block.BaseBlock.X;
                                int relZ = iz - block.BaseBlock.Z;

                                data[ relX * BaseTerrain.BlockSize + relZ ] = heightMap.GetHeight( ix, iz );
                            }
                        }

                        Engine.Server.Wereld.SetBlockHeightMapDataAsync( baseTerrain.ID, x, z, block.BaseBlock.Version, data );
                        //float[] outData;
                        //int outVersion;

                        //Engine.ServerMain.Wereld.TerrainManager.Terrains[ 0 ].SetBlockHeightMapData( x, z, GetEditorBlock( x, z ).BaseBlock.Version, data, out  outVersion, out outData );



                        //if ( outData != null )
                        //{
                        //    block.BaseBlock.Version = outVersion;

                        //    for ( int ix = block.BaseBlock.X; ix < block.BaseBlock.X + BaseTerrain.BlockSize; ix++ )
                        //    {
                        //        for ( int iz = block.BaseBlock.Z; iz < block.BaseBlock.Z + BaseTerrain.BlockSize; iz++ )
                        //        {
                        //            int relX = ix - block.BaseBlock.X;
                        //            int relZ = iz - block.BaseBlock.Z;
                        //            heightMap.SetHeight( ix, iz, data[ relX * BaseTerrain.BlockSize + relZ ] );
                        //        }
                        //    }
                        //}

                        block.HeightMapDirty = false;

                    }

                }
            }

        }


        public void AssignBlockToNode( MHGameWork.TheWizards.Common.GeoMipMap.TerrainBlock block, MHGameWork.TheWizards.Common.Wereld.QuadTreeNode node )
        {
            node.TerrainBlock = block;
            block.QuadTreeNode = node;
        }







        public void RenderWeightPaintMode()
        {
            //Statistics.Reset();

            Device.RenderState.FogEnable = false;
            Device.VertexDeclaration = BaseTerrain.VertexDeclaration;




            //Device.RenderState.AlphaBlendEnable = true;
            Device.RenderState.AlphaBlendEnable = false;
            //Device.RenderState.SourceBlend = Blend.Zero;
            //Device.RenderState.DestinationBlend = Blend.One;
            //Device.RenderState.AlphaTestEnable = false;

            //Device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            //Device.RenderState.AlphaSourceBlend = Blend.One;
            //Device.RenderState.AlphaDestinationBlend = Blend.One;
            //Device.RenderState.SeparateAlphaBlendEnabled = false;


            //Device.RenderState.AlphaDestinationBlend = Blend.One;

            Device.RenderState.SourceBlend = Blend.One;
            Device.RenderState.DestinationBlend = Blend.One;
            //Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            Device.RenderState.BlendFunction = BlendFunction.Add;
            //Device.RenderState.AlphaBlendOperation = BlendFunction.Subtract;
            Device.RenderState.AlphaFunction = CompareFunction.Always;

            //lightDirParam.SetValue( Vector3.Normalize( new Vector3( -1f, 1.5f, -1f ) ) );
            //viewInverseParam.SetValue( Engine.ActiveCamera.CameraInfo.InverseViewMatrix );
            paramBrush.SetValue( new Vector4( brush.Position, brush.Range ) );

            paramWorld.SetValue( BaseTerrain.WorldMatrix );
            //cameraPositionParam.SetValue( Engine.ActiveCamera.CameraPosition );
            paramWorldViewProjection.SetValue(
                BaseTerrain.WorldMatrix
                * Engine.ActiveCamera.CameraInfo.ViewProjectionMatrix );








            editorEffect.Effect.CurrentTechnique = editorEffect.Effect.Techniques[ "TexturedEditor" ];

            //int visibleTriangles;

            //Disable alphablending for the first layer
            Device.RenderState.AlphaBlendEnable = false;
            //Do not render the brush texture on the lower layers, but do use the brush itself to blacken the selected area
            paramBrushTexture.SetValue( (Texture2D)null );


            for ( int iWeightMap = 0; iWeightMap < weightMaps.Count; iWeightMap++ )
            {

                if ( iWeightMap == weightMaps.Count - 1 )
                {
                    paramBrushTexture.SetValue( BrushTexture == null ? null : brushTexture.XNATexture );
                }



                int iTex = iWeightMap * 4;

                paramWeightMap.SetValue( weightMaps[ iWeightMap ] );

                paramTexture1.SetValue( textures[ iTex + 0 ].DiffuseMap.XNATexture );
                paramTexture2.SetValue( iTex + 1 >= textures.Count ? null : textures[ iTex + 1 ].DiffuseMap.XNATexture );
                paramTexture3.SetValue( iTex + 2 >= textures.Count ? null : textures[ iTex + 2 ].DiffuseMap.XNATexture );
                paramTexture4.SetValue( iTex + 3 >= textures.Count ? null : textures[ iTex + 3 ].DiffuseMap.XNATexture );

                //worldParam.SetValue( worldMatrix * Matrix.CreateTranslation( 0, iWeightMap * 20, 0 ) );


                /*worldViewProjectionParam.SetValue(
                worldMatrix * Matrix.CreateTranslation( 0, iWeightMap * 20, 0 )
                * engine.ActiveCamera.CameraInfo.ViewProjectionMatrix );*/


                editorEffect.Effect.Begin();
                //visibleTriangles = 0;

                for ( int i = 0; i < editorEffect.Effect.CurrentTechnique.Passes.Count; i++ )
                {
                    EffectPass pass = editorEffect.Effect.CurrentTechnique.Passes[ i ];

                    pass.Begin();
                    editorEffect.Effect.CommitChanges();


                    if ( ServerClientMainOud.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.W ) )
                    {

                        if ( Device.RenderState.FillMode == FillMode.Solid )
                        { Device.RenderState.FillMode = FillMode.WireFrame; }
                        else { Device.RenderState.FillMode = FillMode.Solid; }
                    }


                    if ( BaseTerrain.QuadTreeNode != null )
                        // / / // / // // //visibleTriangles += DrawTreeNode( HoofdObject.ActiveCamera.CameraInfo.Frustum, trunk );
                        baseTerrain.DrawTreeNode( baseTerrain.QuadTreeNode );

                    //visibleTriangles += DrawTreeNode( BaseTerrain.QuadTreeNode );
                    //blocks[ 0 ][ 0 ].BaseBlock.Draw( Device );

                    pass.End();
                }

                editorEffect.Effect.End();


                //Enable alphablending for the upper layers
                if ( !Engine.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.U ) ) Device.RenderState.AlphaBlendEnable = true;

                if ( Engine.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.O ) ) break;

            }






            Device.RenderState.FogEnable = false;
            Device.RenderState.AlphaBlendEnable = false;

        }


        public void RenderViewMode()
        {
            //Statistics.Reset();

            Device.RenderState.FogEnable = false;
            Device.VertexDeclaration = BaseTerrain.VertexDeclaration;




            Device.RenderState.AlphaBlendEnable = true;
            Device.RenderState.AlphaBlendEnable = false;
            Device.RenderState.SourceBlend = Blend.Zero;
            Device.RenderState.DestinationBlend = Blend.One;
            Device.RenderState.AlphaTestEnable = false;

            Device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            Device.RenderState.AlphaSourceBlend = Blend.One;
            Device.RenderState.AlphaDestinationBlend = Blend.One;
            Device.RenderState.SeparateAlphaBlendEnabled = false;


            Device.RenderState.AlphaDestinationBlend = Blend.One;

            Device.RenderState.SourceBlend = Blend.One;
            Device.RenderState.DestinationBlend = Blend.One;
            Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            Device.RenderState.BlendFunction = BlendFunction.Add;
            Device.RenderState.AlphaBlendOperation = BlendFunction.Subtract;
            Device.RenderState.AlphaFunction = CompareFunction.Always;

            //lightDirParam.SetValue( Vector3.Normalize( new Vector3( -1f, 1.5f, -1f ) ) );
            //viewInverseParam.SetValue( Engine.ActiveCamera.CameraInfo.InverseViewMatrix );

            paramWorld.SetValue( BaseTerrain.WorldMatrix );
            //cameraPositionParam.SetValue( Engine.ActiveCamera.CameraPosition );
            paramWorldViewProjection.SetValue(
                BaseTerrain.WorldMatrix
                * Engine.ActiveCamera.CameraInfo.ViewProjectionMatrix );









            //int visibleTriangles;




            if ( BaseTerrain.QuadTreeNode != null )
                DrawTreeNodeViewBatched( baseTerrain.QuadTreeNode );

            for ( int i = 0; i < viewMaterials.Count; i++ )
            {
                viewMaterials[ i ].Render( Engine );
            }


        }



        public int DrawTreeNodeViewBatched( Wereld.QuadTreeNode node )
        {
            if ( node.Visible != true )
                return 0;

            int totalTriangles = 0;

            if ( node.TerrainBlock == null )
            {

                if ( node.UpperLeft != null )
                    //totalTriangles += DrawTreeNode( frustum, node.UpperLeft );
                    totalTriangles += DrawTreeNodeViewBatched( node.UpperLeft );

                if ( node.UpperRight != null )
                    totalTriangles += DrawTreeNodeViewBatched( node.UpperRight );

                if ( node.LowerLeft != null )
                    totalTriangles += DrawTreeNodeViewBatched( node.LowerLeft );

                if ( node.LowerRight != null )
                    totalTriangles += DrawTreeNodeViewBatched( node.LowerRight );
            }
            else
            {
                TerrainBlock block = GetEditorBlock( node.TerrainBlock );
                block.DrawViewBatched();
            }

            return totalTriangles;
        }









        /// <summary>
        /// Creates a new, empty terrain.
        /// </summary>
        /// <param name="nBlockSize"></param>
        /// <param name="nNumBlocksX"></param>
        /// <param name="nNumBlocksY"></param>
        public void CreateTerrain( int nBlockSize, int nNumBlocksX, int nNumBlocksY )
        {
            //DisposeTerrain();



            //BuildBlocks( nBlockSize, nNumBlocksX, nNumBlocksY );

            //heightMap = new HeightMap( sizeX, sizeY );
            //lightMap = new LightMap( sizeX, sizeY );
            //weightMap = new WeightMap( sizeX, sizeY );

            //for ( int ix = 0; ix < sizeX; ix++ )
            //{
            //    for ( int iy = 0; iy < sizeY; iy++ )
            //    {
            //        lightMap.SetSample( ix, iy, 255 );
            //    }
            //}

            //SaveMaps();

            ////lightMapTexture = new Texture2D( device, sizeX, sizeY, 1, ResourceUsage.None, SurfaceFormat.Luminance8 );
            ////weightMapTexture = new Texture2D( device, sizeX, sizeY, 1, ResourceUsage.None, SurfaceFormat.Color );

            ////byte[] data = new byte[ sizeX * SizeY ];


            ////lightMapTexture.SetData<byte>( data );
            ////( 0, new Rectangle( x, z, terrain.BlockSize + 1, terrain.BlockSize + 1 ), data, 0, data.Length, SetDataOptions.None );


        }

        public void SaveMaps()
        {
            //heightMap.Save( heightMapFilename );
            //lightMap.Save( lightMapFilename );
            //weightMap.Save( weightMapFilename );
        }




        protected void CreateAndSaveToDisk()
        {
            //BuildBlocks( 16, 32, 32 );

            //TerrainLightmapGenerator lightmapGen = new TerrainLightmapGenerator( this );


            //lightMapTexture = new Texture2D( device, heightMap.Width, heightMap.Length, 1, TextureUsage.None, SurfaceFormat.Luminance8 );
            //weightMapTexture = new Texture2D( device, heightMap.Width, heightMap.Length, 1, TextureUsage.None, SurfaceFormat.Color );

            //for ( int x = 0; x < numBlocksX; x++ )
            //{
            //    for ( int z = 0; z < numBlocksY; z++ )
            //    {
            //        GetBlock( x, z ).GenerateLightmap( lightmapGen );
            //        GetBlock( x, z ).GenerateAutoWeights();
            //        /*FileStream FS = new FileStream( Content.RootDirectory + @"\Content\Terrain\Block" + x + "-" + z + ".txt", FileMode.CreateNew, FileAccess.Write );
            //        byte[] b = GetBlock(x,z).ToBytes();
            //        FS.Write( b, 0, b.Length );
            //        FS.Close();*/
            //    }
            //}

            //SaveToDisk();

        }

        public void SaveToDisk()
        {
            //FileStream FS = new FileStream( Content.RootDirectory + @"\Content\Terrain\Data.txt", FileMode.Create, FileAccess.Write );
            //ByteWriter BW = new ByteWriter( FS );

            ////BW.Write( size );

            //for ( int x = 0; x < numBlocksX; x++ )
            //{
            //    for ( int z = 0; z < numBlocksY; z++ )
            //    {
            //        byte[] blockData;
            //        blockData = GetBlock( x, z ).ToBytes();

            //        BW.Write( blockData.Length );
            //        BW.Write( blockData );

            //    }
            //}


            //BW.Close();
            //FS.Close();
            //BW = null;
            //FS = null;


            //LightMap.Save( Content.RootDirectory + @"\Content\Terrain\Lightmap.dds", ImageFileFormat.Dds );
            //WeightMap.Save( Content.RootDirectory + @"\Content\Terrain\WeightMap.dds", ImageFileFormat.Dds );


        }

        public void LoadFromDisk()
        {



            //FileStream FS = new FileStream( filename, FileMode.Open, FileAccess.Read );
            //ByteReader BR = new ByteReader( FS );

            ////size = BR.ReadInt32();


            //BuildBlocks( 16, 32, 32 );

            //for ( int x = 0; x < numBlocksX; x++ )
            //{
            //    for ( int z = 0; z < numBlocksY; z++ )
            //    {
            //        FilePointer pointer = new FilePointer();
            //        pointer.Length = BR.ReadInt32();
            //        pointer.Pos = (int)FS.Position;

            //        GetBlock( x, z ).FilePointer = pointer;

            //        FS.Position += pointer.Length;


            //    }
            //}


            //BR.Close();
            //FS.Close();
            //BR = null;
            //FS = null;


            ////Here, the texture objects should be created. (textures managed by game3Dplay)





        }

        protected void UnloadGraphicsContent( bool unloadAllContent )
        {
            //if ( unloadAllContent )
            //{
            //    if ( heightMap != null )
            //        heightMap.Dispose();

            //    if ( vertexDeclaration != null )
            //        vertexDeclaration.Dispose();

            //    heightMap = null;
            //    vertexDeclaration = null;
            //}

            //// initialize blocks
            //if ( blocks != null )
            //{
            //    for ( int x = 0; x < numBlocksX; x++ )
            //    {
            //        for ( int z = 0; z < numBlocksY; z++ )
            //            GetBlock( x, z ).UnloadGraphicsContent( unloadAllContent );

            //    }
            //}


        }




        public void Update( GameTime gameTime )
        {
            Update();

        }
        public void Update()
        {
            //This line is called by updating the world
            //BaseTerrain.Update();


            ////heightMap.Save( "Content\\TestTerrain001Heightmap.twf" );
            ////temporary
            //if ( tempFrustum == null ) tempFrustum = engine.ActiveCamera.CameraInfo.Frustum;

            ///*tempFrustum = new BoundingFrustum( Matrix.CreateLookAt( new Vector3( 0, 0, 0 )
            //    , new Vector3( 1, 0, 1 ), Vector3.Up )
            //    * engine.ActiveCamera.CameraInfo.ProjectionMatrix );*/


            ////Convert to local coordinates
            //BoundingFrustum localFrustum = new BoundingFrustum( worldMatrix * tempFrustum.Matrix );

            ////cameraPostion = new Vector3( 512, 0, 512 );

            //localCameraPosition = Vector3.Transform( cameraPostion, Matrix.Invert( worldMatrix ) );


            //if ( quadTreeNode != null )
            //    //UpdateTreeNode( HoofdObject.ActiveCamera.CameraInfo.Frustum, trunk, false, gameTime );
            //    UpdateTreeNode( tempFrustum, localFrustum, QuadTreeNode, false );


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frustum">This frustum is in LOCAL coordinates</param>
        /// <param name="node"></param>
        /// <param name="skipFrustumCheck"></param>
        public void UpdateTreeNode( BoundingFrustum frustum, BoundingFrustum localFrustum, Common.Wereld.QuadTreeNode nNode, bool skipFrustumCheck )
        {
            //Wereld.QuadTreeNode node = (Wereld.QuadTreeNode)nNode;
            ///*
            //node.Visible = false;

            //if ( skipFrustumCheck != true )
            //{

            //    ContainmentType containment = frustum.Contains( node.EntityBoundingBox );

            //    if ( containment == ContainmentType.Disjoint )
            //        return;

            //    // if the entire node is contained within, then assume all children are as well
            //    if ( containment == ContainmentType.Contains )
            //        skipFrustumCheck = true;
            //}

            //node.Visible = true;

            //*/

            //if ( node.Visible == false ) return;
            //if ( node.TerrainBlock == null )
            //{
            //    if ( node.UpperLeft != null )
            //        UpdateTreeNode( frustum, localFrustum, node.UpperLeft, skipFrustumCheck );

            //    if ( node.UpperRight != null )
            //        UpdateTreeNode( frustum, localFrustum, node.UpperRight, skipFrustumCheck );

            //    if ( node.LowerLeft != null )
            //        UpdateTreeNode( frustum, localFrustum, node.LowerLeft, skipFrustumCheck );

            //    if ( node.LowerRight != null )
            //        UpdateTreeNode( frustum, localFrustum, node.LowerRight, skipFrustumCheck );
            //}
            //else
            //{
            //    TerrainBlock block = (TerrainBlock)node.TerrainBlock;
            //    block.Update();
            //}
        }

        //public void PaintWeight(int x, int y, int range, int texNum)
        //{
        //    for ( int ix = 0; ix < numPatchesX; ix++ )
        //    {
        //        for ( int iz = 0; iz < numPatchesZ; iz++ )
        //        {
        //            blocks[ ix ][ iz ].PaintWeight( x, y, range, texNum );
        //        }
        //    }
        //}

        //public void Draw( GameTime gameTime )
        //{
        //    Draw();

        //}


        /// <summary>
        /// TODO
        /// </summary>
        public void Save()
        {
            if ( BaseTerrain.TerrainInfo.HeightMapFileID != -1 )
                heightMap.Save( Engine.GameFileManager.GetGameFile( BaseTerrain.TerrainInfo.HeightMapFileID ).GetFullFilename() );
        }






        public void PaintWeight( float x, float z, float range, int texNum, float weight )
        {
            Vector3 transformed = Vector3.Transform( new Vector3( x, 0, z ), Matrix.Invert( WorldMatrix ) );
            x = transformed.X;
            z = transformed.Z;



            //int iWeightMap = (int)Math.Floor( texNum / (double)4 );
            int relativeTexNum = texNum % 4;

            //Texture2D weightMap = weightMaps[ iWeightMap ];



            int minX = (int)Math.Floor( x - range );
            int maxX = (int)Math.Floor( x + range ) + 1;
            int minZ = (int)Math.Floor( z - range );
            int maxZ = (int)Math.Floor( z + range ) + 1;

            minX = (int)MathHelper.Clamp( minX, 0, SizeX - 1 );
            maxX = (int)MathHelper.Clamp( maxX, 0, SizeX - 1 );
            minZ = (int)MathHelper.Clamp( minZ, 0, SizeY - 1 );
            maxZ = (int)MathHelper.Clamp( maxZ, 0, SizeY - 1 );


            int areaSizeX = maxX - minX + 1;
            int areaSizeZ = maxZ - minZ + 1;

            Rectangle rect = new Rectangle( minX, minZ, areaSizeX, areaSizeZ );

            Color[][] data = new Color[ weightMaps.Count ][];

            for ( int iWeight = 0; iWeight < weightMaps.Count; iWeight++ )
            {
                data[ iWeight ] = new Color[ ( areaSizeX ) * ( areaSizeZ ) ];
            }

            //weightMap.GetData<Color>( 0, rect, data, 0, data.Length );

            float rangeSq = range * range;
            x -= minX;
            z -= minZ;

            for ( int ix = 0; ix < areaSizeX; ix++ )
            {
                for ( int iz = 0; iz < areaSizeZ; iz++ )
                {
                    float distSq = Vector2.DistanceSquared( new Vector2( ix, iz ), new Vector2( x, z ) );
                    if ( distSq < rangeSq )
                    {
                        float dist = (float)Math.Sqrt( distSq );

                        float factor = 1 - ( dist / range );
                        factor *= 255;


                        ////Use some sort of maxing algorithm
                        //Color c = data[ iz * ( areaSizeX ) + ix ];
                        //float a = c.A;
                        //float r = c.R;
                        //float g = c.G;
                        //float b = c.B;

                        //float curVal = 0;
                        //switch ( texNum )
                        //{
                        //    case 0:
                        //        curVal = r;
                        //        break;
                        //    case 1:
                        //        curVal = g;
                        //        break;
                        //    case 2:
                        //        curVal = b;
                        //        break;
                        //    case 3:
                        //        curVal = a;
                        //        break;
                        //}


                        //byte maxVal = (byte)Math.Floor( factor );




                        factor *= weight;
                        factor += 5;
                        //if ( factor > maxVal - curVal ) factor = (float)( maxVal - curVal );
                        factor = MathHelper.Clamp( factor, 0, 255 );
                        //factor = MathHelper.Clamp( factor, 0, maxVal );


                        byte total = 0;

                        //Deel elke kleur door het nieuwe totaal * 255
                        for ( int iTex = 0; iTex < textures.Count; iTex++ )
                        {
                            float val = textures[ iTex ].AlphaMap.GetSample( ix + minX, iz + minZ );
                            val = val / ( 255 + factor ) * 255;
                            val = (float)Math.Floor( val );
                            textures[ iTex ].AlphaMap.SetSample( ix + minX, iz + minZ, (byte)val );
                            if ( iTex != texNum ) total += (byte)val;
                        }
                        //a = a / ( 255 + factor ) * 255;
                        //r = r / ( 255 + factor ) * 255;
                        //g = g / ( 255 + factor ) * 255;
                        //b = b / ( 255 + factor ) * 255;

                        //a = (float)Math.Floor( a );
                        //r = (float)Math.Floor( r );
                        //g = (float)Math.Floor( g );
                        //b = (float)Math.Floor( b );

                        //Zorgt dat de som exact 255 is, de overschot gaat naar de gekozen weight

                        //byte oldVal = textures[ texNum ].AlphaMap.GetSample( ix + minX, iz + minZ );

                        textures[ texNum ].AlphaMap.SetSample( ix + minX, iz + minZ, (byte)( 255 - total ) );
                        //switch ( texNum )
                        //{
                        //    case 0:
                        //        r = 255 - g - b - a;
                        //        break;
                        //    case 1:
                        //        g = 255 - r - b - a;
                        //        break;
                        //    case 2:
                        //        b = 255 - r - g - a;
                        //        break;
                        //    case 3:
                        //        a = 255 - r - g - b;
                        //        break;
                        //}



                        //data[ iz * ( areaSizeX ) + ix ] = CalculateWeightMapColor( ix + minX, iz + minZ, iWeightMap );
                        //data[ iz * ( areaSizeX ) + ix ] = new Color( (byte)r, (byte)g, (byte)b, (byte)a );

                    }
                    for ( int iWeightMap = 0; iWeightMap < weightMaps.Count; iWeightMap++ )
                    {
                        data[ iWeightMap ][ iz * ( areaSizeX ) + ix ] = CalculateWeightMapColor( ix + minX, iz + minZ, iWeightMap );
                    }
                }
            }

            for ( int iWeightMap = 0; iWeightMap < weightMaps.Count; iWeightMap++ )
            {
                BaseTerrain.Device.Textures[ 0 ] = null;

                weightMaps[ iWeightMap ].SetData<Color>( 0, rect, data[ iWeightMap ], 0, data[ iWeightMap ].Length, SetDataOptions.None );


            }
            //weightMap.SetData<Color>( 0, rect, data, 0, data.Length, SetDataOptions.None );


        }
        public void PaintWeightOud( float x, float z, float range, int texNum, float weight )
        {
            //Vector3 transformed = Vector3.Transform( new Vector3( x, 0, z ), Matrix.Invert( worldMatrix ) );
            //x = transformed.X;
            //z = transformed.Z;



            //int iWeightMap = (int)Math.Floor( texNum / (double)4 );
            //texNum = texNum % 4;

            //Texture2D weightMap = weightMaps[ iWeightMap ];



            //int minX = (int)Math.Floor( x - range );
            //int maxX = (int)Math.Floor( x + range ) + 1;
            //int minZ = (int)Math.Floor( z - range );
            //int maxZ = (int)Math.Floor( z + range ) + 1;

            //minX = (int)MathHelper.Clamp( minX, 0, sizeX - 1 );
            //maxX = (int)MathHelper.Clamp( maxX, 0, sizeX - 1 );
            //minZ = (int)MathHelper.Clamp( minZ, 0, sizeY - 1 );
            //maxZ = (int)MathHelper.Clamp( maxZ, 0, sizeY - 1 );


            //int areaSizeX = maxX - minX + 1;
            //int areaSizeZ = maxZ - minZ + 1;

            //Rectangle rect = new Rectangle( minX, minZ, areaSizeX, areaSizeZ );

            //Color[] data = new Color[ ( areaSizeX ) * ( areaSizeZ ) ];

            //weightMap.GetData<Color>( 0, rect, data, 0, data.Length );

            //float rangeSq = range * range;
            //x -= minX;
            //z -= minZ;

            //for ( int ix = 0; ix < areaSizeX; ix++ )
            //{
            //    for ( int iz = 0; iz < areaSizeZ; iz++ )
            //    {
            //        float distSq = Vector2.DistanceSquared( new Vector2( ix, iz ), new Vector2( x, z ) );
            //        if ( distSq < rangeSq )
            //        {
            //            float dist = (float)Math.Sqrt( distSq );

            //            float factor = 1 - ( dist / range );
            //            factor *= 255;


            //            //Use some sort of maxing algorithm
            //            Color c = data[ iz * ( areaSizeX ) + ix ];
            //            float a = c.A;
            //            float r = c.R;
            //            float g = c.G;
            //            float b = c.B;

            //            float curVal = 0;
            //            switch ( texNum )
            //            {
            //                case 0:
            //                    curVal = r;
            //                    break;
            //                case 1:
            //                    curVal = g;
            //                    break;
            //                case 2:
            //                    curVal = b;
            //                    break;
            //                case 3:
            //                    curVal = a;
            //                    break;
            //            }


            //            byte maxVal = (byte)Math.Floor( factor );




            //            factor *= weight;
            //            factor += 5;
            //            if ( factor > maxVal - curVal ) factor = (float)( maxVal - curVal );
            //            factor = MathHelper.Clamp( factor, 0, 255 );
            //            //factor = MathHelper.Clamp( factor, 0, maxVal );


            //            //Deel elke kleur door het nieuwe totaal * 255
            //            a = a / ( 255 + factor ) * 255;
            //            r = r / ( 255 + factor ) * 255;
            //            g = g / ( 255 + factor ) * 255;
            //            b = b / ( 255 + factor ) * 255;

            //            a = (float)Math.Floor( a );
            //            r = (float)Math.Floor( r );
            //            g = (float)Math.Floor( g );
            //            b = (float)Math.Floor( b );

            //            //Zorgt dat de som exact 255 is, de overschot gaat naar de gekozen weight

            //            switch ( texNum )
            //            {
            //                case 0:
            //                    r = 255 - g - b - a;
            //                    break;
            //                case 1:
            //                    g = 255 - r - b - a;
            //                    break;
            //                case 2:
            //                    b = 255 - r - g - a;
            //                    break;
            //                case 3:
            //                    a = 255 - r - g - b;
            //                    break;
            //            }


            //            data[ iz * ( areaSizeX ) + ix ] = new Color( (byte)r, (byte)g, (byte)b, (byte)a );


            //        }
            //    }
            //}


            //weightMap.SetData<Color>( 0, rect, data, 0, data.Length, SetDataOptions.None );


        }

        public void AddTexture( TerrainTexture texture )
        {
            texture.AlphaMap = new Buffer2D( SizeX, SizeY );
            textures.Add( texture );

            if ( textures.Count % 4 == 1 )
            {
                //This texture is the first of a new weightmap
                //normally we should just create the new weightmap, but we regenerate all of them just for easyness

                GenerateWeightMaps();
            }
        }

        public void AddTexture( Engine.Texture diffuseMap )
        {
            TerrainTexture texture = new TerrainTexture( Engine );
            texture.DiffuseMap = diffuseMap;
            AddTexture( texture );
        }

        public void GenerateWeightMaps()
        {
            for ( int i = 0; i < weightMaps.Count; i++ )
            {
                weightMaps[ i ].Dispose();
            }
            weightMaps.Clear();


            for ( int iTex = 0; iTex < textures.Count; iTex += 4 )
            {
                Texture2D weightMap;


                weightMap = new Texture2D( Device, SizeX, SizeY, 1, TextureUsage.None, SurfaceFormat.Color );

                Color[] data = new Color[ SizeX * SizeY ];

                for ( int y = 0; y < SizeY; y++ )
                {
                    for ( int x = 0; x < SizeX; x++ )
                    {
                        //byte r = textures[ iTex ].AlphaMap.GetSample( x, y );
                        //byte g = iTex + 1 >= textures.Count ? (byte)0 : textures[ iTex + 1 ].AlphaMap.GetSample( x, y );
                        //byte b = iTex + 2 >= textures.Count ? (byte)0 : textures[ iTex + 2 ].AlphaMap.GetSample( x, y );
                        //byte a = iTex + 3 >= textures.Count ? (byte)0 : textures[ iTex + 3 ].AlphaMap.GetSample( x, y );

                        //data[ y * SizeX + x ] = new Color( r, g, b, a );
                        data[ y * SizeX + x ] = CalculateWeightMapColorByStartTex( x, y, iTex );
                    }
                }

                weightMap.SetData<Color>( data );

                weightMaps.Add( weightMap );

            }
        }

        public Color CalculateWeightMapColor( int x, int y, int iWeightMap )
        {
            return CalculateWeightMapColorByStartTex( x, y, iWeightMap * 4 );
        }
        private Color CalculateWeightMapColorByStartTex( int x, int y, int iStartTex )
        {
            byte r = textures[ iStartTex ].AlphaMap.GetSample( x, y );
            //The bottom texture should always be all over the terrain, otherwise we get alpha artifacts
            //if ( iStartTex == 0 ) r = 255;
            byte g = iStartTex + 1 >= textures.Count ? (byte)0 : textures[ iStartTex + 1 ].AlphaMap.GetSample( x, y );
            byte b = iStartTex + 2 >= textures.Count ? (byte)0 : textures[ iStartTex + 2 ].AlphaMap.GetSample( x, y );
            byte a = iStartTex + 3 >= textures.Count ? (byte)0 : textures[ iStartTex + 3 ].AlphaMap.GetSample( x, y );

            return new Color( r, g, b, a );
        }

        public int FindCorrespondingTextureIndex( Engine.Texture diffuseMap )
        {
            for ( int i = 0; i < textures.Count; i++ )
            {
                if ( textures[ i ].DiffuseMap == diffuseMap ) return i;
            }
            return -1;
        }

        //public int DrawTreeNode(BoundingFrustum frustumk TreeNode node)
        public int DrawTreeNode( Common.Wereld.QuadTreeNode nNode )
        {
            //Wereld.QuadTreeNode node = (Wereld.QuadTreeNode)nNode;
            //if ( node.Visible != true )
            //    return 0;

            //int totalTriangles = 0;

            //if ( node.TerrainBlock == null )
            //{

            //    if ( node.UpperLeft != null )
            //        //totalTriangles += DrawTreeNode( frustum, node.UpperLeft );
            //        totalTriangles += DrawTreeNode( node.UpperLeft );

            //    if ( node.UpperRight != null )
            //        totalTriangles += DrawTreeNode( node.UpperRight );

            //    if ( node.LowerLeft != null )
            //        totalTriangles += DrawTreeNode( node.LowerLeft );

            //    if ( node.LowerRight != null )
            //        totalTriangles += DrawTreeNode( node.LowerRight );
            //}
            //else
            //{
            //    TerrainBlock block = (TerrainBlock)node.TerrainBlock;
            //    totalTriangles += block.Draw( device );
            //}

            //return totalTriangles;

            return 0;
        }

        public TerrainBlock GetEditorBlock( int x, int z )
        {
            return blocks[ x ][ z ];
        }

        public TerrainBlock GetEditorBlock( XNAGeoMipMap.TerrainBlock baseBlock )
        {
            return GetEditorBlock( baseBlock.BlockNumX, baseBlock.BlockNumZ );
        }

        public TerrainTexture GetTexture( int index )
        {
            return textures[ index ];
        }

        //public float WaterLevel
        //{
        //    get { return waterLevel; }
        //    set { waterLevel = value; }
        //}

        //public Texture2D GetTextureOld( int i )
        //{
        //    return textures[ i ].XNATexture;
        //}

        //public Texture2D LightMap
        //{
        //    get { return lightMapTexture; }
        //    set { lightMapTexture = value; }
        //}

        //public Texture2D WeightMap
        //{
        //    get { return weightMapTexture; }
        //    set { weightMapTexture = value; }
        //}

        public GraphicsDevice Device { get { return BaseTerrain.Device; } }
        public int NumBlocksY
        {
            get { return baseTerrain.NumBlocksY; }
        }
        public int NumBlocksX
        {
            get { return baseTerrain.NumBlocksX; }
        }
        public ServerClientMainOud Engine
        {
            get { return baseTerrain.Engine; }
        }
        public Matrix WorldMatrix
        {
            get { return BaseTerrain.WorldMatrix; }
        }
        public int SizeX
        {
            get { return BaseTerrain.SizeX; }
        }
        public int SizeY
        {
            get { return BaseTerrain.SizeY; }
        }



        //New

        private ServerClientMainOud engine;

        private Engine.GameFileOud terrainInfoFile;
        private TerrainInfoFile terrainInfo;

        Engine.LoadTaskRef initializeTaskRef;

        public TerrainOud( ServerClientMainOud nEngine, Engine.GameFileOud nTerrainInfoFile )
        {
            engine = nEngine;
            terrainInfoFile = nTerrainInfoFile;
        }

        public Engine.LoadingTaskState InitializeTask( Engine.LoadingTaskType nType )
        {
            terrainInfo = TerrainInfoFile.LoadFromDisk( terrainInfoFile );

            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;
        }


        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            if ( !Initialized )
            {
                //Not yet initialized.

                if ( initializeTaskRef.IsEmpty )
                {
                    initializeTaskRef = engine.LoadingManager.AddLoadTaskAdvanced( InitializeTask, ServerClient.Engine.LoadingTaskType.Normal );
                }
                else if ( initializeTaskRef.State == ServerClient.Engine.LoadingTaskState.Completed )
                {
                    //shouldnt happen, because this if wouldn't be entered otherwise
                    throw new Exception();
                }
            }
        }




        public void Render()
        {
            if ( !Initialized ) return;





        }

        private bool Initialized { get { return terrainInfo != null; } }

    }
}



