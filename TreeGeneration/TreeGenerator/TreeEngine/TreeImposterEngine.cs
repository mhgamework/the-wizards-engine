using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TreeGenerator.help;
using TreeGenerator.Imposter;

namespace TreeGenerator.TreeEngine
{
    public class TreeImposterEngine
    {
        //todo for the lod function
        //check where there is free space on both textures
        //
        public List<TangentVertex> Vertices = new List<TangentVertex>();
        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;
        public ColladaShader Shader;
        IXNAGame game;
        GraphicsDevice device;

        public List<ImposterStruct> Imposters = new List<ImposterStruct>();
        private DepthStencilBuffer depthBuffer;
        SpriteBatch batch;
        public RenderTarget2D RenderTargetOne;
        public Texture2D textureOne;
        public Texture2D textureTwo;// this is used for the lod system
        // should be tuned, size of the textures must be equally because switch renderdivice asks to much performance
        public int SizeOne = 64;
        public int SizeTwo = 32;
        FullScreenQuad quad;
        public int NumImpOne = 16, NumImpTwo;
        public int ImpostersTextureSizeOne;
        public int ImpostersTextureSizeTwo;

        // for lod
        private int levelThreeCount = 10000;//value needs to be evaluated
        private bool[] freePlaceOne, freePlaceTwo, freePlaceThree;
        public TangentVertex[] VertOne, VertTwo, VertThree;

        public VertexBuffer vertexBufferOne, vertexBufferTwo, vertexBufferThree;
        public VertexDeclaration declOne, declTwo, declThree;
        public int vertexCountOne, vertexCountTwo, vertexCountThree;
        public int triangleCountOne, triangleCountTwo, lineCount;
        public int vertexStrideOne, vertexStrideTwo, vertexStrideThree;
        public ColladaShader ShaderOne, ShaderTwo;

        public RenderTarget2D RenderTargetTwo;

        public float Time = 0;
        public float TimeForUpdate = 0.5f;

        public float ImposterAngleThreshold = 4f;
        public int ImposterUpdatesPerFrame = 4;
        private int VerticesIndex1;
        private int VerticesIndex2;
        private int VerticesIndex3;
        private int sortingIndex = -1;



        //for testing only
        public bool test = false;
        //

        public TreeImposterEngine( IXNAGame _game )
        {
            game = _game;
            device = game.GraphicsDevice;

        }


        public void Initialze()
        {
            //lod
            NumImpTwo = NumImpOne * ( SizeOne / SizeTwo );
            ImpostersTextureSizeOne = SizeOne * NumImpOne;
            ImpostersTextureSizeTwo = SizeTwo * NumImpTwo;

            freePlaceOne = new bool[ NumImpOne * NumImpOne ];
            freePlaceTwo = new bool[ NumImpTwo * NumImpTwo ];
            freePlaceThree = new bool[ levelThreeCount ];//value has to be evaluated
            for ( int i = 0; i < freePlaceOne.Length; i++ )
            {
                freePlaceOne[ i ] = true;
            }
            for ( int i = 0; i < freePlaceTwo.Length; i++ )
            {
                freePlaceTwo[ i ] = true;
            }
            for ( int i = 0; i < freePlaceThree.Length; i++ )
            {
                freePlaceThree[ i ] = true;
            }

            VertOne = new TangentVertex[ NumImpOne * NumImpOne * 6 ];

            VertTwo = new TangentVertex[ NumImpTwo * NumImpTwo * 6 ];
            VertThree = new TangentVertex[ levelThreeCount * 2 ];
            lineCount = levelThreeCount;//
            // perhaps not necesarry I can just use the same shader but just change the texture although could be dangerous loading this in the memory everytime
            ShaderOne = new ColladaShader( game, new Microsoft.Xna.Framework.Graphics.EffectPool() );
            ShaderOne.Technique = ColladaShader.TechniqueType.Textured;
            ShaderOne.ViewInverse = Matrix.Identity;
            ShaderOne.ViewProjection = Matrix.Identity;
            ShaderOne.LightDirection = Vector3.Normalize( new Vector3( 0.6f, 1f, 0.6f ) );
            ShaderOne.LightColor = new Vector3( 1, 1, 1 );
            ShaderOne.AmbientColor = new Vector4( 1f, 1f, 1f, 1f );
            ShaderOne.DiffuseColor = new Vector4( 1f, 1f, 1f, 1f );
            ShaderOne.SpecularColor = new Vector4( 0.1f, 0.1f, 0.1f, 0.1f );
            ShaderOne.Technique = ColladaShader.TechniqueType.Textured;

            ShaderTwo = new ColladaShader( game, new Microsoft.Xna.Framework.Graphics.EffectPool() );
            ShaderTwo.Technique = ColladaShader.TechniqueType.Textured;
            ShaderTwo.ViewInverse = Matrix.Identity;
            ShaderTwo.ViewProjection = Matrix.Identity;
            ShaderTwo.LightDirection = Vector3.Normalize( new Vector3( 0.6f, 1f, 0.6f ) );
            ShaderTwo.LightColor = new Vector3( 1, 1, 1 );
            ShaderTwo.AmbientColor = new Vector4( 1f, 1f, 1f, 1f );
            ShaderTwo.DiffuseColor = new Vector4( 1f, 1f, 1f, 1f );
            ShaderTwo.SpecularColor = new Vector4( 0.1f, 0.1f, 0.1f, 0.1f );
            ShaderTwo.Technique = ColladaShader.TechniqueType.Textured;


            //old 
            device = game.GraphicsDevice;
            Shader = new ColladaShader( game, null );

            batch = new SpriteBatch( device );
            depthBuffer = new DepthStencilBuffer( device, ImpostersTextureSizeOne, ImpostersTextureSizeOne, device.DepthStencilBuffer.Format );
            RenderTargetOne = new RenderTarget2D( device, ImpostersTextureSizeOne, ImpostersTextureSizeOne, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents );
            RenderTargetTwo = new RenderTarget2D( device, ImpostersTextureSizeTwo, ImpostersTextureSizeTwo, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents );


            Shader = new ColladaShader( game, new Microsoft.Xna.Framework.Graphics.EffectPool() );

            Shader.Technique = ColladaShader.TechniqueType.Textured;

            Shader.ViewInverse = Matrix.Identity;
            Shader.ViewProjection = Matrix.Identity;
            Shader.LightDirection = Vector3.Normalize( new Vector3( 0.6f, 1f, 0.6f ) );
            Shader.LightColor = new Vector3( 1, 1, 1 );

            Shader.AmbientColor = new Vector4( 1f, 1f, 1f, 1f );
            Shader.DiffuseColor = new Vector4( 1f, 1f, 1f, 1f );
            Shader.SpecularColor = new Vector4( 0.1f, 0.1f, 0.1f, 0.1f );

            Shader.Technique = ColladaShader.TechniqueType.Textured;

            declOne = TangentVertexExtensions.CreateVertexDeclaration( game );
            vertexStrideOne = TangentVertex.SizeInBytes;
            vertexCountOne = VertOne.Length;
            triangleCountOne = vertexCountOne / 3;

            vertexBufferOne = new DynamicVertexBuffer( device, typeof( TangentVertex ), vertexCountOne, BufferUsage.WriteOnly );


            declTwo = TangentVertexExtensions.CreateVertexDeclaration( game );
            vertexStrideTwo = TangentVertex.SizeInBytes;
            vertexCountTwo = VertTwo.Length;
            triangleCountTwo = vertexCountTwo / 3;

            vertexBufferTwo = new DynamicVertexBuffer( device, typeof( TangentVertex ), vertexCountTwo, BufferUsage.WriteOnly );

            declThree = TangentVertexExtensions.CreateVertexDeclaration( game );
            vertexStrideThree = TangentVertex.SizeInBytes;
            vertexCountThree = VertThree.Length;
            vertexBufferThree = new DynamicVertexBuffer( device, typeof( TangentVertex ), vertexCountThree, BufferUsage.WriteOnly );

            //for pointspriteTest
            //IntializeForBillBoard();

        }


        public delegate void TreeRenderDelegate( Matrix viewProjection);
        protected void ComputeBoundingBoxFromPoints( Vector3[] vertices, out Vector3 min, out Vector3 max )
        {
            min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

            for ( int i = 0; i < vertices.Length; i++ )
            {
                min = Vector3.Min( min, vertices[ i ] );
                max = Vector3.Max( max, vertices[ i ] );
            }
        }



        // new lod functions must of them consist of copies clean up is for later

        public void UpdateLod()
        {
            #region keys
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.U ) )
            {
                for ( int i = 0; i < Imposters.Count; i++ )
                {
                    Imposters[ i ].NeedsUpdate = true;
                }
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.I ) )
            {
                Imposters[ 0 ].NeedsUpdate = true;

            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.O ) )
            {
                Imposters[ 1 ].NeedsUpdate = true;

            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.M ) )
            {
                textureOne.Save( game.EngineFiles.RootDirectory + "tex1.jpg", ImageFileFormat.Jpg );
                textureTwo.Save( game.EngineFiles.RootDirectory + "tex2.jpg", ImageFileFormat.Jpg );
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.P ) )
            {
                test = true;
            }
            //if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            //{
            //    InitializeBillBoardShader();
            //}
            #endregion

            /* What must be evaluated in the update methode?
             * the further away the less often it needs to be evaluated
             * this implies also the distance checking
             * the more the angle differces the bigger the urge to update
             * 
             * 
            */

            //NOTE: Places test. komen de free places wel overeen met de imposters?
            //TODO: WARNING: momenteel dus niet!
            for ( int i = 0; i < Imposters.Count; i++ )
            {
                if ( Imposters[ i ].CurrentLODLevel == 2 )
                {
                    if ( freePlaceTwo[ Imposters[ i ].Index ] ) throw new Exception();
                }
            }


            bool calculateUrge = false;
            Time += game.Elapsed;
            if ( Time > TimeForUpdate )
            {
                calculateUrge = true;
                Time = 0;
            }


            for ( int i = 0; i < Imposters.Count; i++ )
            {
                ImposterStruct iImposter = Imposters[ i ];
                iImposter.Distance = Vector3.Distance( iImposter.BoundingBoxPosition, game.Camera.ViewInverse.Translation );


                if ( calculateUrge && ( iImposter.CurrentLODLevel == 1 || iImposter.CurrentLODLevel == 2 ) )
                {
                    iImposter.CalculatedUrgeForUpdate( game.Camera.ViewInverse.Translation );
                }

                SetImposterLOD( iImposter );
                searchFreePlace( iImposter, i );

            }


            //Imposters.Sort();
            sortMichiel();
            RemoveNoSortNeeded();
            NewUpdateImposterLod();

        }

        private void SetImposterLOD( ImposterStruct iImposter )
        {
            //to far to see so trees are not rendered
            iImposter.NewLODLevel = 4;
            if ( iImposter.Distance > iImposter.MinDistanceFour ) return;

            //trees are in range to be rendered as lines
            iImposter.NewLODLevel = 3;
            if ( iImposter.Distance > iImposter.MinDistanceThree ) return;

            //imposterTextureTwo
            iImposter.NewLODLevel = 2;
            if ( iImposter.Distance > iImposter.MinDistanceTwo ) return;

            //imposterTextureOne
            iImposter.NewLODLevel = 1;
            if ( iImposter.Distance > iImposter.MinDistanceOne ) return;

            //drawMesh
            iImposter.NewLODLevel = 0;
            //iImposter.Normal = game.Camera.ViewInverse.Translation - iImposter.BoundingBoxPosition;
        }


        // still needs to make a smooth transition between mesh-->imposter

        private void searchFreePlace( ImposterStruct imp, int index )
        {
            //NOTE: Wat de free places betreft
            //      speedup: sla de index eerste lege plaats op in een int. Als een plaats wordt geleegd voor die eerste plaats
            //               wordt dat de eerste plaats. Is die eerste plaats niet meer leeg, zoek dan de eerstvolgende lege.
            //               Start dus met zoeken vanaf de index in die variabele naar een lege plaats


            //if ( imp.WantsToGoToOne && imp.DrawImposterOne && imp.TextureLodChanged )////TextureOne


            //NOTE: belangrijk idee!!! Implementeer die verschillende lod levels met behulp van een (State) Strategy Pattern
            //      Dit vervangt dan de CurrentLODLevel integer door een Strategy/State




            if ( imp.NewLODLevel == 1 && imp.CurrentLODLevel != 1 )
            {
                for ( int i = 0; i < freePlaceOne.Length; i++ )
                {
                    if ( !freePlaceOne[ i ] ) continue;


                    UnsetImposterLodLevel( imp );

                    freePlaceOne[ i ] = false;
                    imp.InitializeForLod( i, NumImpOne, 1, SizeOne );
                    imp.NeedsUpdate = true;
                    MoveImposterInSortFunction( index );
                    imp.CurrentLODLevel = 1;


                    return;



                }

            }
            if ( imp.NewLODLevel == 2 && imp.CurrentLODLevel != 2 )
            {
                for ( int i = 0; i < freePlaceTwo.Length; i++ )
                {
                    if ( !freePlaceTwo[ i ] ) continue;

                    // Check if this place actually is free!
                    /*for ( int j = 0; j < Imposters.Count; j++ )
                    {
                        if ( Imposters[ j ] != imp && Imposters[ j ].CurrentLODLevel == imp.NewLODLevel && Imposters[ j ].Index == i )
                            throw new Exception();
                    }*/


                    UnsetImposterLodLevel( imp );


                    freePlaceTwo[ i ] = false;
                    imp.InitializeForLod( i, NumImpTwo, 2, SizeTwo );
                    imp.NeedsUpdate = true;
                    MoveImposterInSortFunction( index );
                    imp.CurrentLODLevel = 2;


                    return;

                }
            }


            if ( imp.NewLODLevel == 3 && imp.CurrentLODLevel != 3 )
            {
                for ( int i = 0; i < freePlaceThree.Length; i++ )
                {
                    if ( !freePlaceThree[ i ] ) continue;

                    UnsetImposterLodLevel( imp );

                    freePlaceThree[ i ] = false;

                    imp.Index = i;
                    CreateLine( imp );

                    imp.CurrentLODLevel = 3;

                    imp.NeedsImposterRender = false;
                    imp.NeedsUpdate = false;
                    imp.UrgeForUpdate = 0;

                    return;
                }



            }

            if ( imp.NewLODLevel == 0 && imp.CurrentLODLevel != 0 )
            {
                UnsetImposterLodLevel( imp );

                imp.CurrentLODLevel = 0;

                imp.NeedsImposterRender = false;
                imp.NeedsUpdate = false;
                imp.UrgeForUpdate = 0;

            }
            if ( imp.NewLODLevel == 4 && imp.CurrentLODLevel != 4 )
            {

                UnsetImposterLodLevel( imp );

                imp.CurrentLODLevel = 4;

                imp.NeedsImposterRender = false;
                imp.NeedsUpdate = false;
                imp.UrgeForUpdate = 0;
            }
        }
        /// <summary>
        /// Deze functie cleart de huidige imposter status: 
        /// imp.currentLOD wordt -1
        /// De vertices worden gecleared van de imposter/lines
        ///         /// </summary>
        /// <param name="imp"></param>
        private void UnsetImposterLodLevel( ImposterStruct imp )
        {
            if ( imp.CurrentLODLevel == 1 )
            {
                freePlaceOne[ imp.Index ] = true;
                DiscartVertImp( 1, imp.Index );

            }
            else if ( imp.CurrentLODLevel == 2 )
            {
                freePlaceTwo[ imp.Index ] = true;
                DiscartVertImp( 2, imp.Index );


            }
            else if ( imp.CurrentLODLevel == 3 )
            {
                freePlaceThree[ imp.Index ] = true;
                DiscartVertImp( 3, imp.Index );

            }
            imp.CurrentLODLevel = -1;
            imp.Index = -1;
        }

        private void MoveImposterInSortFunction( int index )
        {
            sortingIndex++;
            ImposterStruct tmp = Imposters[ index ];
            if ( index < sortingIndex )
            { sortingIndex--; return; }


            Imposters[ index ] = Imposters[ sortingIndex ];
            Imposters[ sortingIndex ] = tmp;
        }
        private void MoveImposterOutSortFunction( int index )
        {
            ImposterStruct tmp = Imposters[ sortingIndex ];
            Imposters[ sortingIndex ] = Imposters[ index ];
            Imposters[ index ] = tmp;
            sortingIndex--;
        }
        public void RemoveNoSortNeeded()
        {
            for ( int i = sortingIndex; i > 0; i-- )
            {
                if ( !( Imposters[ i ].CurrentLODLevel == 1 ) && !( Imposters[ i ].CurrentLODLevel == 2 ) )
                {
                    sortingIndex--;
                }
                else
                {
                    return;
                }
            }
        }



        private void DiscartVertImp( int WhichLevel, int index )
        {
            if ( WhichLevel == 1 )
            {
                VertOne[ index * 6 + 0 ].pos = Vector3.Zero;
                VertOne[ index * 6 + 1 ].pos = Vector3.Zero;
                VertOne[ index * 6 + 2 ].pos = Vector3.Zero;
                VertOne[ index * 6 + 3 ].pos = Vector3.Zero;
                VertOne[ index * 6 + 4 ].pos = Vector3.Zero;
                VertOne[ index * 6 + 5 ].pos = Vector3.Zero;
            }
            if ( WhichLevel == 2 )
            {
                VertTwo[ index * 6 + 0 ].pos = Vector3.Zero;
                VertTwo[ index * 6 + 1 ].pos = Vector3.Zero;
                VertTwo[ index * 6 + 2 ].pos = Vector3.Zero;
                VertTwo[ index * 6 + 3 ].pos = Vector3.Zero;
                VertTwo[ index * 6 + 4 ].pos = Vector3.Zero;
                VertTwo[ index * 6 + 5 ].pos = Vector3.Zero;
            }
            if ( WhichLevel == 3 )
            {
                VertThree[ index * 2 + 0 ].pos = Vector3.Zero;
                VertThree[ index * 2 + 1 ].pos = Vector3.Zero;

            }
        }
        private void RenderPrimitivesLodOne()
        {
            //TODO: hou ook de laatste volle plaats bij, want niet alle imposters moeten altijd gerendered worden

            //GraphicsDevice device = vertexBuffer.GraphicsDevice;
            device.Vertices[ 0 ].SetSource( vertexBufferOne, 0, vertexStrideOne );
            device.VertexDeclaration = declOne;
            device.DrawPrimitives( PrimitiveType.TriangleList, 0, triangleCountOne );

        }
        private void RenderPrimitivesLodTwo()
        {
            //TODO: hou ook de laatste volle plaats bij, want niet alle imposters moeten altijd gerendered worden

            //GraphicsDevice device = vertexBuffer.GraphicsDevice;
            device.Vertices[ 0 ].SetSource( vertexBufferTwo, 0, vertexStrideTwo );
            device.VertexDeclaration = declTwo;
            device.DrawPrimitives( PrimitiveType.TriangleList, 0, triangleCountTwo );

        }
        private void RenderPrimitivesLodThree()
        {
            //GraphicsDevice device = vertexBuffer.GraphicsDevice;
            device.Vertices[ 0 ].SetSource( vertexBufferThree, 0, vertexStrideThree );
            device.VertexDeclaration = declThree;
            //device.DrawPrimitives(PrimitiveType.LineList, 0, lineCount);
            device.DrawUserPrimitives<TangentVertex>( PrimitiveType.LineList, VertThree, 0, lineCount );
        }

        private void SetVerticesIndex()
        {
            for ( int i = vertexCountOne - 1; i > 0; )
            {
                if ( VertOne[ i ].pos.Length() < 0.1f )
                {
                    i -= 3;
                }
                else { VerticesIndex1 = i; break; }
            }
            for ( int i = vertexCountTwo - 1; i > 0; )
            {
                if ( VertTwo[ i ].pos.Length() < 0.1f )
                {
                    i -= 3;
                }
                else { VerticesIndex2 = i; break; }
            }
            for ( int i = vertexCountThree - 1; i > 0; )
            {
                if ( VertThree[ i ].pos.Length() < 0.1f )
                {
                    i -= 3;
                }
                else { VerticesIndex3 = i; break; }
            }
        }


        public void RenderLod()
        {
            //SetupForRender();
            RenderImpostersLod();

            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.CullMode = CullMode.None;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.ReferenceAlpha = 80;
            device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;

            //don't think the shader matters so
            //Shader.Shader.RenderMultipass(RenderPrimitivesLodThree);
            //RenderPrimitivesLodThree();//since I can't manage to get this to work
            for ( int i = 0; i < VertThree.Length - 1; i += 2 )
            {
                //game.LineManager3D.AddLine( VertThree[ i ].pos, VertThree[ i + 1 ].pos, new Color( 90, 60, 50 ) );
            }


            ShaderTwo.ViewProjection = game.Camera.ViewProjection;
            ShaderTwo.ViewInverse = game.Camera.ViewInverse;
            ShaderTwo.World = Matrix.Identity;

            game.GraphicsDevice.RenderState.DepthBufferEnable = true;

            ShaderTwo.Shader.RenderMultipass( RenderPrimitivesLodTwo );

            // normaly I thnik i should be possible to use one shader   although I don't know what is the mlost intensive loading the new texture every time or this way
            ShaderOne.ViewProjection = game.Camera.ViewProjection;
            ShaderOne.ViewInverse = game.Camera.ViewInverse;
            ShaderOne.World = Matrix.Identity;

            game.GraphicsDevice.RenderState.DepthBufferEnable = true;

            //ShaderOne.Shader.RenderMultipass( RenderPrimitivesLodOne );
            //device.RenderState.FillMode = FillMode.WireFrame;
            //device.RenderState.CullMode = CullMode.None;
            //device.RenderState.AlphaTestEnable = true;
            //device.RenderState.ReferenceAlpha = 150;
            //device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
            //device.RenderState.AlphaBlendEnable = false;
            //device.RenderState.DepthBufferWriteEnable = true;
            //RenderBillBoard();


            //device.RenderState.CullMode = CullMode.None;
            //device.RenderState.AlphaTestEnable = true;
            //device.RenderState.ReferenceAlpha = 150;
            //device.RenderState.AlphaFunction = CompareFunction.Less;
            //device.RenderState.AlphaBlendEnable = true;
            //device.RenderState.DepthBufferWriteEnable = false;
            //RenderBillBoard();

            game.GraphicsDevice.RenderState.DepthBufferEnable = true;

            for ( int i = 0; i < Imposters.Count; i++ )
            {
                if ( Imposters[ i ].DrawMesh )
                {
                    //Imposters[ i ].RenderObject( game.Camera.ViewProjection, Imposters[ i ].MatrixIndex );
                }
            }

            // for point sprite


        }

        public void AddRenderObjectLod( TreeRenderDelegate render, Vector3[] boundingBox)
        {


            ImposterStruct imp = new ImposterStruct( render, boundingBox, SizeOne);
            imp.Distance = ( Vector3.Distance( imp.BoundingBoxPosition, game.Camera.ViewInverse.Translation ) );

            SetImposterLOD( imp );
            Imposters.Add( imp );
            searchFreePlace( Imposters[ Imposters.Count - 1 ], Imposters.Count - 1 );

        }


        public void NewUpdateImposterLod()
        {
            int count = 0;
            for ( int i = 0; i < Imposters.Count; i++ )
            {
                //if ( Imposters[ i ].NeedsUpdate && ( Imposters[ i ].CurrentLODLevel == 1 || Imposters[ i ].CurrentLODLevel == 2 ) )
                //{

                if ( !Imposters[ i ].NeedsUpdate )
                    count++;
                if ( count > ImposterUpdatesPerFrame )
                    break;
                if ( Imposters[ i ].UrgeForUpdate < ImposterAngleThreshold && !Imposters[ i ].NeedsUpdate ) break;
                //Imposters[ i ].UrgeForUpdate = 0;


                //MHGameWork.TheWizards.ServerClient.ICamera beginCamera = game.Camera;
                Vector3[] impostorVerts = new Vector3[ 8 ];

                Vector3 cameraPosition = game.Camera.ViewInverse.Translation;
                ImposterCamera camera = new ImposterCamera();

                //----------------------------------------------------------------------
                // Step 1. Get bounding volume corners and set the camera to look at the
                //         diffuse mesh


                // Get the 8 corners of the bounding box and transform by the world matrix 
                Vector3[] corners = new Vector3[ 8 ];
                //boundingBox = CreateBoundingBox(model);
                //I     think this unnecesarry for now because I already moved them
                //Vector3.Transform(boundingBox, ref WorldMatrix, corners);
                corners = Imposters[ i ].BoundingBox;
                // Get the transformed center of the mesh 
                // alternatively, we could use mBoundingSphere.Center as sometimes it works better


                // Set the camera to be at the center of the reflector and to look at the diffuse mesh

                camera.LookAt( cameraPosition, Imposters[ i ].MeshCenter );
                Vector3 CameraDirection = cameraPosition - Imposters[ i ].BoundingBoxPosition;
                //CameraDirection = cameraPosition - meshCenter;
                Imposters[ i ].Normal = CameraDirection;
                // just for testing
                //normal = BoundingBoxPosition - game.Camera.ViewProjection.Translation;

                camera.BuildView();


                //----------------------------------------------------------------------
                // Step 2. Project the corners of the bounding volume and construct a 
                //         screen space quad that fits the boundaries of the mesh


                // Now we project the vertices to screen space, so we can find the AABB of the screen
                // space vertices
                Vector3[] screenVerts = new Vector3[ 8 ];
                for ( int j = 0; j < 8; j++ )
                {
                    screenVerts[ j ] = device.Viewport.Project( corners[ j ], camera.Projection, camera.View, Matrix.Identity );

                }

                // compute the screen space AABB
                Vector3 min, max;
                ComputeBoundingBoxFromPoints( screenVerts, out min, out max );

                // construct the quad that will represent our diffuse mesh
                Vector3[] screenQuadVerts = new Vector3[ 4 ];
                screenQuadVerts[ 0 ] = new Vector3( min.X, min.Y, min.Z );
                screenQuadVerts[ 1 ] = new Vector3( max.X, min.Y, min.Z );
                screenQuadVerts[ 2 ] = new Vector3( max.X, max.Y, min.Z );
                screenQuadVerts[ 3 ] = new Vector3( min.X, max.Y, min.Z );


                //----------------------------------------------------------------------
                // Step 3. Unproject the screen quad vertices to form a 3D quad that 
                //         represents our impostor. We will use this to both draw the 
                //         impostor quad and for intersecting the impostor for reflection.

                //now unproject the screen space quad and save the
                //vertices for when we render the impostor quad
                for ( int j = 0; j < 4; j++ )
                {
                    impostorVerts[ j ] = device.Viewport.Unproject( screenQuadVerts[ j ], camera.Projection, camera.View, Matrix.Identity );
                }

                //compute the center of the quad
                Vector3 impostorCenter = Vector3.Zero;
                impostorCenter = impostorVerts[ 0 ] + impostorVerts[ 1 ] + impostorVerts[ 2 ] + impostorVerts[ 3 ];
                impostorCenter *= .25f;

                // calculate the width and height of the imposter's vertices
                float width = ( impostorVerts[ 1 ] - impostorVerts[ 0 ] ).Length() * 1.2f;
                float height = ( impostorVerts[ 3 ] - impostorVerts[ 0 ] ).Length() * 1.2f;

                // We construct an Orthographic projection to get rid of the projection distortion
                // which we don't want for our impostor texture
                camera.Projection = Matrix.CreateOrthographic( width, height, .1f, 1000 );// I can't set the Projection so if this doesn't work i will try there camera
                camera.BuildView();
                Imposters[ i ].Cam = camera;
                //save the WorldViewProjection matrix so we can use it in the shader
                //Matrix worldViewProj = camera.ViewProj;


                //renderTarget = new RenderTarget2D(device, sizeX, sizeY, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents);

                //CreatePlain(Imposters[i], impostorVerts);
                CreatePlainLod( Imposters[ i ], impostorVerts, impostorCenter );
                Imposters[ i ].NeedsUpdate = false;
                Imposters[ i ].NeedsImposterRender = true;
                Imposters[ i ].CalculatedUrgeForUpdate( game.Camera.ViewInverse.Translation );
                // }

            }


            //NOTE: BART ik weet waarom het zo traag gaat (waarschijnlijk). 
            //      Ge kopieerd elke frame u volledige lijst vertices, dus mss wa minder voor de performance
            //      een oplossing hiervoor is mss gewoon direct setdata doen ipv ze eerst in de VertOne op te slagen
            //      of een double buffer maken: 2 vertexbuffer's, ene met de current en ene met de nieuwe, maar ik denk dat
            //      BufferUsage.Dynamic ervoor zorgt dat u videokaart al zo iets doet.
            SetupForRenderLod();


        }

        //this will go a bit slower but at the end there is an performance gain (I hope)

        public void RenderImpostersLod()
        {


            Viewport oldViewPort = device.Viewport;
            DepthStencilBuffer oldDepthBuffer = device.DepthStencilBuffer;

            // the first texture
            device.DepthStencilBuffer = depthBuffer;
            device.SetRenderTarget( 0, RenderTargetOne );
            device.Clear( ClearOptions.DepthBuffer | ClearOptions.Target, Color.Orange, 1f, 0 );
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
            batch.Begin( SpriteBlendMode.None ); //SpriteBlendMode.AlphaBlend);
            if ( textureOne != null )
            {
                if ( !game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.N ) )
                    batch.Draw( textureOne, new Vector2( 0, 0 ), new Color( 255, 255, 255, 255 ) );
            }
            batch.End();

            //DrawScreenQuad();



            for ( int i = 0; i < Imposters.Count; i++ )
            {

                if ( Imposters[ i ].NeedsImposterRender && Imposters[ i ].CurrentLODLevel == 1 )
                {
                    if ( VertOne[ Imposters[ i ].Index * 6 ].pos.LengthSquared() < 0.1f )
                        throw new InvalidOperationException();


                    device.Viewport = Imposters[ i ].VPort;
                    device.Clear( ClearOptions.DepthBuffer | ClearOptions.Target, new Color( 0, 0, 0, 0 ), 1f, 0 );// something needs to be fixed over here
                    //device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Red, 1f, 0);// something needs to be fixed over here
                    Imposters[ i ].RenderObject( Imposters[ i ].Cam.ViewProj);





                    Imposters[ i ].NeedsImposterRender = false;
                    //texture = Texture2D.FromFile(device, game.RootDirectory + "Grass//Textures//grass.tga"); the problem is at the texture it self
                }//textureOne.Save(game.EngineFiles.RootDirectory + "texturetest"+".tga", ImageFileFormat.Tga);
            }
            //device.Viewport = oldViewPort;
            device.SetRenderTarget( 0, null );
            textureOne = RenderTargetOne.GetTexture();
            //device.DepthStencilBuffer = oldDepthBuffer;
            device.Clear( ClearOptions.DepthBuffer | ClearOptions.Target, Color.CornflowerBlue, 1f, 0 );



            // the second texture
            //device.DepthStencilBuffer = depthBuffer;
            device.SetRenderTarget( 0, RenderTargetTwo );
            device.Clear( ClearOptions.DepthBuffer | ClearOptions.Target, Color.Orange, 1f, 0 );
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
            batch.Begin( SpriteBlendMode.None ); //SpriteBlendMode.AlphaBlend);
            if ( textureTwo != null )
            {
                if ( !game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.N ) )

                    batch.Draw( textureTwo, new Vector2( 0, 0 ), new Color( 255, 255, 255, 255 ) );
            }
            batch.End();

            //DrawScreenQuad();
            // NOTE: probeer dit maar is, dit geeft fouten. EDIT: niet meer
            /*
            for ( int i = 0; i < Imposters.Count; i++ )
            {
                for ( int j = 0; j < Imposters.Count; j++ )
                {
                    if ( i == j ) continue;
                    
                    if ( Imposters[ i ].Index == Imposters[ j ].Index
                        && Imposters[ i ].Index != 0
                        && Imposters[ i ].CurrentLODLevel == Imposters[ j ].CurrentLODLevel
                        && Imposters[ i ].CurrentLODLevel == 2 ) throw new Exception();
                }
            }*/


            for ( int i = 0; i < Imposters.Count; i++ )
            {


                if ( ( Imposters[ i ].NeedsImposterRender ) && ( Imposters[ i ].CurrentLODLevel == 2 ) )
                {
                    if ( VertTwo[ Imposters[ i ].Index * 6 ].pos.LengthSquared() < 0.1f )
                        throw new InvalidOperationException();

                    device.Viewport = Imposters[ i ].VPort;
                    device.Clear( ClearOptions.DepthBuffer | ClearOptions.Target, new Color( 0, 0, 0, 0 ), 1f, 0 );// something needs to be fixed over here
                    //device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Yellow, 1f, 0);// something needs to be fixed over here
                    Imposters[ i ].RenderObject( Imposters[ i ].Cam.ViewProj);





                    Imposters[ i ].NeedsImposterRender = false;
                    //texture = Texture2D.FromFile(device, game.RootDirectory + "Grass//Textures//grass.tga"); the problem is at the texture it self
                }//textureOne.Save(game.EngineFiles.RootDirectory + "texturetest"+".tga", ImageFileFormat.Tga);
            }
            device.Viewport = oldViewPort;
            device.SetRenderTarget( 0, null );
            textureTwo = RenderTargetTwo.GetTexture();
            device.DepthStencilBuffer = oldDepthBuffer;
            device.Clear( ClearOptions.DepthBuffer | ClearOptions.Target, Color.CornflowerBlue, 1f, 0 );


        }

        public void CreateLine( ImposterStruct imp )
        {
            VertThree[ imp.Index * 2 + 0 ] = new TangentVertex( imp.CalculateLineDown(), 0f, 0f, Vector3.UnitX, Vector3.UnitZ );
            VertThree[ imp.Index * 2 + 1 ] = new TangentVertex( imp.CalculateLineUp(), 0f, 0f, Vector3.UnitX, Vector3.UnitZ );

        }

        public void CreatePlainLod( ImposterStruct imp, Vector3[] impostorVerts, Vector3 center )
        {
            Vector3 tangent = Vector3.Up;
            Vector2 uvStart, uvEnd;

            float tempSize = 1;
            //32 this depends on how many pixels you give for each imposter
            //reminder if this doesn't work perhaps you still have to put an f after it for float
            //triangle 1
            //if ( imp.DrawImposterOne && imp.WantsToGoToOne == false )
            if ( imp.CurrentLODLevel == 1 )
            {
                tempSize = 1 / ( (float)ImpostersTextureSizeOne / SizeOne );//clean-up texturesizeone and tow will always be the same



                uvStart = new Vector2( imp.XCoord * tempSize, imp.YCoord * tempSize );
                uvEnd = new Vector2( ( imp.XCoord + 1 ) * tempSize, ( imp.YCoord + 1 ) * tempSize );



                VertOne[ imp.Index * 6 + 0 ] = new TangentVertex( impostorVerts[ 0 ], uvStart.X, uvStart.Y, imp.Normal, tangent );
                VertOne[ imp.Index * 6 + 1 ] = new TangentVertex( impostorVerts[ 3 ], uvStart.X, uvEnd.Y, imp.Normal, tangent );
                VertOne[ imp.Index * 6 + 2 ] = new TangentVertex( impostorVerts[ 2 ], uvEnd.X, uvEnd.Y, imp.Normal, tangent );
                //triangle 2
                VertOne[ imp.Index * 6 + 3 ] = new TangentVertex( impostorVerts[ 0 ], uvStart.X, uvStart.Y, imp.Normal, tangent );
                VertOne[ imp.Index * 6 + 4 ] = new TangentVertex( impostorVerts[ 1 ], uvEnd.X, uvStart.Y, imp.Normal, tangent );
                VertOne[ imp.Index * 6 + 5 ] = new TangentVertex( impostorVerts[ 2 ], uvEnd.X, uvEnd.Y, imp.Normal, tangent );



                //// for the billboardtesting

                //float width = ( impostorVerts[ 1 ] - impostorVerts[ 0 ] ).Length() * 1.2f;
                //float length = ( impostorVerts[ 3 ] - impostorVerts[ 0 ] ).Length() * 1.2f;

                ////float width=Vector3.Distance(impostorVerts[0],impostorVerts[2]);
                //// float length=Vector3.Distance(impostorVerts[2],impostorVerts[3]);
                //vertBill[ imp.Index * 6 + 0 ] = new VertexBillboard( center, new Vector2( width, length ), new Vector2( 0, 0 ), uvStart, imp.Normal );
                //vertBill[ imp.Index * 6 + 1 ] = new VertexBillboard( center, new Vector2( width, length ), new Vector2( 0, 1 ), new Vector2( uvStart.X, uvEnd.Y ), imp.Normal );
                //vertBill[ imp.Index * 6 + 2 ] = new VertexBillboard( center, new Vector2( width, length ), new Vector2( 1, 1 ), uvEnd, imp.Normal );
                ////triangle 2
                //vertBill[ imp.Index * 6 + 3 ] = new VertexBillboard( center, new Vector2( width, length ), new Vector2( 0, 0 ), uvStart, imp.Normal );
                //vertBill[ imp.Index * 6 + 4 ] = new VertexBillboard( center, new Vector2( width, length ), new Vector2( 1, 0 ), new Vector2( uvEnd.X, uvStart.Y ), imp.Normal );
                //vertBill[ imp.Index * 6 + 5 ] = new VertexBillboard( center, new Vector2( width, length ), new Vector2( 1, 1 ), uvEnd, imp.Normal );

                /*vertBill[ imp.Index * 6 + 0 ] = new VertexBillboard( center + new Vector3( -1, 1, 0 ), new Vector2( width, length ), new Vector2( 0, 0 ), uvStart, imp.Normal );
                //vertBill[ imp.Index * 6 + 1 ] = new VertexBillboard( center + new Vector3( -1, -1, 0 ), new Vector2( width, length ), new Vector2( 0, 1 ), new Vector2( uvStart.X, uvEnd.Y ), imp.Normal );
                //vertBill[ imp.Index * 6 + 2 ] = new VertexBillboard( center + new Vector3( 1, -1, 0 ), new Vector2( width, length ), new Vector2( 1, 1 ), uvEnd, imp.Normal );
                ////triangle 2
                //vertBill[ imp.Index * 6 + 3 ] = new VertexBillboard( center + new Vector3( -1, 1, 0 ), new Vector2( width, length ), new Vector2( 0, 0 ), uvStart, imp.Normal );
                //vertBill[ imp.Index * 6 + 4 ] = new VertexBillboard( center + new Vector3( 1, 1, 0 ), new Vector2( width, length ), new Vector2( 1, 0 ), new Vector2( uvEnd.X, uvStart.Y ), imp.Normal );
                //vertBill[ imp.Index * 6 + 5 ] = new VertexBillboard( center + new Vector3( 1, -1, 0 ), new Vector2( width, length ), new Vector2( 1, 1 ), uvEnd, imp.Normal );*/

            }
            if ( imp.CurrentLODLevel == 2 )
            {
                tempSize = 1 / ( (float)ImpostersTextureSizeOne / SizeTwo );//clean-up texturesizeone and tow will always be the same



                uvStart = new Vector2( imp.XCoord * tempSize, imp.YCoord * tempSize );
                uvEnd = new Vector2( ( imp.XCoord + 1 ) * tempSize, ( imp.YCoord + 1 ) * tempSize );


                VertTwo[ imp.Index * 6 + 0 ] = new TangentVertex( impostorVerts[ 0 ], uvStart.X, uvStart.Y, imp.Normal, tangent );
                VertTwo[ imp.Index * 6 + 1 ] = new TangentVertex( impostorVerts[ 3 ], uvStart.X, uvEnd.Y, imp.Normal, tangent );
                VertTwo[ imp.Index * 6 + 2 ] = new TangentVertex( impostorVerts[ 2 ], uvEnd.X, uvEnd.Y, imp.Normal, tangent );
                //triangle 2
                VertTwo[ imp.Index * 6 + 3 ] = new TangentVertex( impostorVerts[ 0 ], uvStart.X, uvStart.Y, imp.Normal, tangent );
                VertTwo[ imp.Index * 6 + 4 ] = new TangentVertex( impostorVerts[ 1 ], uvEnd.X, uvStart.Y, imp.Normal, tangent );
                VertTwo[ imp.Index * 6 + 5 ] = new TangentVertex( impostorVerts[ 2 ], uvEnd.X, uvEnd.Y, imp.Normal, tangent );
            }
        }


        public void SetupForRenderLod()
        {
            SetVerticesIndex();
            triangleCountOne = (int)( VerticesIndex1 * 0.3333 );
            triangleCountTwo = (int)( VerticesIndex2 * 0.3333 );
            lineCount = (int)( VerticesIndex3 * 0.5f );

            if ( textureOne != null )
                ShaderOne.DiffuseTexture = textureOne;

            vertexBufferOne.SetData( VertOne );



            if ( textureTwo != null )
                ShaderTwo.DiffuseTexture = textureTwo;

            vertexBufferTwo.SetData( VertTwo );



            vertexBufferThree.SetData( VertThree );

            ////for pointSprite
            //SetUpRenderBillBoard();

        }

        public void sort()
        {
           
            int i = 0;
            bool swap = false;
            do
            {
                swap = false;
                for ( int j = i + 1; j <= sortingIndex; j++ )
                {
                    if ( Imposters[ j ].NeedsUpdate && !Imposters[ i ].NeedsUpdate )
                    {
                        ImposterStruct tmp = Imposters[ i ];
                        Imposters[ i ] = Imposters[ j ];
                        Imposters[ j ] = tmp;

                        swap = true;
                    }
                    else
                    {
                        if ( ( Imposters[ j ].UrgeForUpdate > Imposters[ i ].UrgeForUpdate ) && ( !Imposters[ i ].NeedsUpdate ) )
                        {
                            ImposterStruct tmp = Imposters[ i ];
                            Imposters[ i ] = Imposters[ j ];
                            Imposters[ j ] = tmp;
                            swap = true;
                        }


                    }
                }
                i++;

            }
            while ( swap == true );


        }

        public void sortMichiel()
        {
            // Bart, ge had al een manier om de imposters te vergelijken gemaakt. 
            //   Ik vermoed dat er iets mis is met de methode die ge nu doet, vandaar ik het verander
            //   Als er toch geen fout in zit is het alleszins duidelijker



            int i = 0;
            bool swap;
            do
            {
                swap = false;
                for ( int j = i + 1; j <= sortingIndex; j++ )
                {
                    if ( Imposters[ j ].CompareTo( Imposters[ i ] ) < 0 )
                    {
                        ImposterStruct tmp = Imposters[ i ];
                        Imposters[ i ] = Imposters[ j ];
                        Imposters[ j ] = tmp;

                        swap = true;
                    }

                }
                i++;

            }
            while ( swap );


        }

        //to test the billboard
        //public struct VertexBillboard
        //{
        //    public Vector3 Position;
        //    public Vector2 Size;
        //    public Vector2 SquarePosition;
        //    public Vector2 TexUV;
        //    public Vector3 Normal;


        //    public VertexBillboard( Vector3 position, Vector2 size, Vector2 squarePosition, Vector2 texUV ,Vector3 normal )
        //    {
        //        Position = position;
        //        Size = size;
        //        SquarePosition = squarePosition;
        //        TexUV = texUV;
        //        Normal = normal;
        //    }

        //    public static readonly VertexElement[] VertexElements =
        //     {
        //         new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
        //         new VertexElement(0, 4*(3), VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
        //         new VertexElement(0, 4*(3+2), VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
        //         new VertexElement(0, 4*(3+2+2), VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2),
        //         new VertexElement(0, 4*(3+2+2+2), VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
                 
                 
        //     };
        //    public static int SizeInBytes = sizeof( float ) * ( 3 + 2 + 2 + 2 + 3 );
        //}
        //public BasicShader BillBoardShader;
        //public VertexBillboard[] vertBill;
        //public VertexBuffer vertexBufferBill;
        //public VertexDeclaration declBill;
        ////public int vertexCountBill;
        //public int triangleCountBill;
        //public int vertexStrideBill;


        //private void IntializeForBillBoard()
        //{
        //    vertBill = new VertexBillboard[ NumImpOne * NumImpOne * 6 ];
        //    InitializeBillBoardShader();
        //}

        //private void InitializeBillBoardShader()
        //{
        //    if ( BillBoardShader != null ) BillBoardShader.Dispose();
        //    BillBoardShader = BasicShader.LoadFromFXFile( game, new GameFile( game.EngineFiles.RootDirectory + @"TreeEngine\BillBoardShader.fx" ) );

        //    BillBoardShader.SetTechnique( "Billboard" );
        //    BillBoardShader.SetParameter( "world", Matrix.Identity );
        //    BillBoardShader.SetParameter( "viewProjection", Matrix.Identity );
        //    BillBoardShader.SetParameter( "viewInverse", Matrix.Identity );

        //    //shity i need to fixes this
        //    BillBoardShader.SetParameter( "height", 1.0f );
        //    BillBoardShader.SetParameter( "width", 1.0f );
        //}

        //private void SetUpRenderBillBoard()//needs to be made in a properway
        //{

        //    TWTexture tex = TWTexture.FromTexture2D( textureOne );
        //    BillBoardShader.SetParameter( "diffuseTexture", tex );

        //    vertexStrideBill = VertexBillboard.SizeInBytes;
        //    vertexBufferBill = new VertexBuffer( game.GraphicsDevice, typeof( VertexBillboard ), vertBill.Length, BufferUsage.None );
        //    vertexBufferBill.SetData( vertBill );
        //    declBill = new VertexDeclaration( device, VertexBillboard.VertexElements );

        //    triangleCountBill = NumImpOne * NumImpOne * 2;

        //}
        //public void RenderBillBoard()
        //{

        //    BillBoardShader.SetParameter( "view", game.Camera.View );
        //    BillBoardShader.SetParameter( "projection", game.Camera.Projection );
        //    BillBoardShader.SetParameter( "viewInverse", game.Camera.ViewInverse );
        //    BillBoardShader.SetParameter( "viewProjection", game.Camera.ViewProjection );

        //    BillBoardShader.RenderMultipass( renderPrimitiveBillBoard );
        //}

        //private void renderPrimitiveBillBoard()
        //{

        //    device.VertexDeclaration = declBill;
        //    device.Vertices[ 0 ].SetSource( vertexBufferBill, 0, vertexStrideBill );
        //    device.DrawPrimitives( PrimitiveType.TriangleList, 0, triangleCountBill );// don't know if this goes faster but just to make sure to not make mistake "TotalVertices"
        //}

        public static void TestImposters()
        {
            XNAGame game;
            game = new XNAGame();
            //game.DrawFps = true;

            TreeTypeData treeTypeData;
            TreeStructure treeStruct;
            TreeStructureGenerater genStruct = new TreeStructureGenerater();
            EngineTreeRenderData renderData = new EngineTreeRenderData(game);
            EngineTreeRenderDataGenerater genData = new EngineTreeRenderDataGenerater(10);
            Seeder seeder = new Seeder(47856);
            List<Matrix> Matrices = new List<Matrix>();



             TreeImposterEngine impEngine = new TreeImposterEngine(game);


            game.InitializeEvent +=
               delegate
               {
                   treeTypeData = TreeTypeData.GetTestTreeType();
                   treeStruct = genStruct.GenerateTree(treeTypeData, 123);
                   genData.GetRenderData(treeStruct, game, 0);

                   renderData = genData.TreeRenderData;
                   renderData.Initialize();
                   impEngine.Initialze();
                   for (int i = 0; i < 1000; i++)
                   {
                       Matrix mat = new Matrix();
                       mat = Matrix.CreateScale(seeder.NextFloat(1, 1.4f));
                       mat *= Matrix.CreateRotationY(seeder.NextFloat(0, MathHelper.TwoPi));
                       mat *= Matrix.CreateTranslation(seeder.NextVector3(new Vector3(300, 0, 300), new Vector3(0, 0, 0)));
                       Matrices.Add(mat);

                       //impEngine.AddRenderObjectLod(renderData.draw, renderData.BoundingBoxData);


                   }
               };

            game.DrawEvent +=
                delegate
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        renderData.Transform(Matrices[i]);
                        renderData.draw();
                    }


                };
            game.Run();
        }
    }

    public class ImposterStruct : IComparable<ImposterStruct> // don't know or I should make this an struct or an class
    {
        /**
         * For the LOD functionality
         * */

        private int currentLODLevel;
        /// <summary>
        /// This is the current rendering lod level.
        /// 0 is full detail and greater is lower detail
        /// </summary>
        public int CurrentLODLevel
        {
            get { return currentLODLevel; }
            set
            {
                // Pas lod level instellen als de vorige ge unset is!!!
                if ( currentLODLevel != -1 && value != -1 ) throw new Exception();
                currentLODLevel = value;
            }
        }
        /// <summary>
        /// This is the LOD level that this imposter needs to be, but its possible that it is still waiting to switch from the OldLODLevel
        /// 0 is full detail and greater is lower detail
        /// </summary>
        public int NewLODLevel;

        public int CompareTo( ImposterStruct other )
        {
            if ( needsUpdate && other.needsUpdate ) return 0;
            if ( needsUpdate ) return -1;
            if ( other.needsUpdate ) return 1;
            if ( UrgeForUpdate > other.UrgeForUpdate ) return -1;
            if ( UrgeForUpdate < other.UrgeForUpdate ) return 1;
            return 0;

        }



        public bool DrawMesh
        {
            get { return CurrentLODLevel == 0; }
        }

        //these values still have to be optimized

        public float MinDistanceOne = 20f;
        public float MinDistanceTwo = 250f;
        public float MinDistanceThree = 180f;
        public float MinDistanceFour = 1000f;



        public float Angle;




        /**
         * Non-LOD
         * */


        public TreeImposterEngine.TreeRenderDelegate RenderObject;

        private bool needsUpdate = true;

        public bool NeedsUpdate
        {
            get { return needsUpdate; }
            set
            {
                needsUpdate = value;
                /*if ( needsUpdate == true && value == false )
                {
                    time = 0;
                }*/
            }
        }
        public float UrgeForUpdate = 0;//100 is max and 0 is nothing, we can also call this the mistakemargine

        public void CalculatedUrgeForUpdate( Vector3 cameraPosition )
        {
            if ( normal.LengthSquared() < 0.00001f )
            {
                UrgeForUpdate = 1000;
                return;
            }

            UrgeForUpdate = Math.Abs( AngleBetweenTwoV3( cameraPosition - BoundingBoxPosition, normal ) );
            if ( float.IsNaN( UrgeForUpdate ) ) throw new InvalidOperationException();
        }
        public float AngleBetweenTwoV3( Vector3 v1, Vector3 v2 )
        {
            v1.Normalize();
            v2.Normalize();
            float angle = (float)Math.Acos( MathHelper.Clamp( Vector3.Dot( v1, v2 ), -1, 1 ) );
            angle = MathHelper.ToDegrees( angle );

            return angle;
        }// in radians


        // public bool DrawImposterOne = false;

        //public float MaxAngleDifference = 10f;
        //public float MaxAngleDiffernceAtMinDistance = 5f;// in degrees not in radians like in the other imposter disgn
        //public float MaxTime = 1f;
        //public float MaxtimeAtMinDist = 0.1f;
        private float time = 0;
        public float Time
        {
            get { return time; }
            set { time = value; }
        }

        public int XCoord, YCoord, Index;

        public Vector3[] BoundingBox = new Vector3[ 8 ];
        public Vector3 MeshCenter;
        public Vector3 BoundingBoxPosition;
        private Vector3 normal;

        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; normal.Normalize(); if ( float.IsNaN( normal.X ) ) throw new InvalidOperationException(); }
        }
        private float distance;


        public float Distance
        {
            get { return distance; }
            set
            {
                distance = value;

            }
        }



        // just for the new UpdateImposterClass
        public Imposter.ImposterCamera Cam = new Imposter.ImposterCamera();
        public Viewport VPort = new Viewport();
        public bool NeedsImposterRender = false;

        public bool DiscartRenderData = false;
        public bool LoadRenderData = false;
        public bool RenderDataGone = false;

       
        public int TextureIndex; // this is the same as index I think stupid mistake from me



        public ImposterStruct( int xCo, int yCo, int index, TreeImposterEngine.TreeRenderDelegate _renderObject, Vector3[] boundingBox, int size, int matrixIndex )
        {
           
            XCoord = xCo;
            YCoord = yCo;
            Index = index;
            RenderObject = _renderObject;
            BoundingBox = boundingBox;
            Cam = new ImposterCamera();
            VPort = new Viewport();
            VPort.X = XCoord * size;
            VPort.Y = YCoord * size;
            VPort.Width = size;
            VPort.Height = size;
            VPort.MaxDepth = 1.0f;
            VPort.MinDepth = 0.0f;
            BoundingBoxPosition = new Vector3( ( boundingBox[ 1 ].X + boundingBox[ 0 ].X ) * 0.5f, boundingBox[ 0 ].Y, ( boundingBox[ 2 ].Z + boundingBox[ 0 ].Z ) * 0.5f );
            CalculateCenter();
        }

        public ImposterStruct( TreeImposterEngine.TreeRenderDelegate _renderObject, Vector3[] boundingBox, int size)
        {
          
            RenderObject = _renderObject;
            BoundingBox = boundingBox;
            Cam = new ImposterCamera();
            BoundingBoxPosition = new Vector3( ( boundingBox[ 1 ].X + boundingBox[ 0 ].X ) * 0.5f, boundingBox[ 0 ].Y, ( boundingBox[ 2 ].Z + boundingBox[ 0 ].Z ) * 0.5f );
            CalculateCenter();
            CreateViewPort( size );
        }

        public void InitializeForLod( int index, int imposterCount, int WhichImposter, int resSize )
        {
            /*DrawImposterOne = false;
            DrawImposterTwo = false;
            if ( WhichImposter == 1 )
            {
                DrawImposterOne = true;
            }
            if ( WhichImposter == 2 )
            {
                DrawImposterTwo = true;
            }*/
            Index = index;
            CalculateXYCoord( imposterCount );
            CreateViewPort( resSize );
        }

        public void CreateViewPort( float XCoord, float YCoord, int size )
        {
            VPort = new Viewport();
            VPort.X = (int)XCoord * size;
            VPort.Y = (int)YCoord * size;
            VPort.Width = size;
            VPort.Height = size;
            VPort.MaxDepth = 1.0f;
            VPort.MinDepth = 0.0f;
        }
        public void CreateViewPort( int resSize )
        {
            VPort = new Viewport();
            VPort.X = XCoord * resSize;
            VPort.Y = YCoord * resSize;
            VPort.Width = resSize;
            VPort.Height = resSize;
            VPort.MaxDepth = 1.0f;
            VPort.MinDepth = 0.0f;
        }
        public void CalculateXYCoord( int imposterCount )
        {
            //changed textureindex to index
            //float step = 1f / imposterCount;
            XCoord = ( Index - (int)( Index / imposterCount ) * imposterCount ); //* step;
            YCoord = ( (int)( Index / imposterCount ) );// *step;

        }
        public void CalculateCenter()
        {

            Vector3 min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            Vector3 max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

            for ( int i = 0; i < BoundingBox.Length; i++ )
            {
                min = Vector3.Min( min, BoundingBox[ i ] );
                max = Vector3.Max( max, BoundingBox[ i ] );
            }

            MeshCenter = ( min + max ) * 0.5f;

        }
        public Vector3 CalculateLineDown()
        {
            Vector3 min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            Vector3 max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

            for ( int i = 0; i < BoundingBox.Length; i++ )
            {
                min = Vector3.Min( min, BoundingBox[ i ] );
                max = Vector3.Max( max, BoundingBox[ i ] );
            }
            return new Vector3( ( min.X + max.X ) * 0.5f, min.Y, ( min.Z + max.Z ) * 0.5f );
        }
        public Vector3 CalculateLineUp()
        {
            Vector3 min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            Vector3 max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

            for ( int i = 0; i < BoundingBox.Length; i++ )
            {
                min = Vector3.Min( min, BoundingBox[ i ] );
                max = Vector3.Max( max, BoundingBox[ i ] );
            }
            return new Vector3( ( min.X + max.X ) * 0.5f, max.Y, ( min.Z + max.Z ) * 0.5f );
        }


        public override string ToString()
        {
            string txt;
            txt = "Imposter";
            txt += currentLODLevel.ToString();
            txt += UrgeForUpdate.ToString( " 0.000" );
            if ( needsUpdate ) txt += " NeedsUpdate=true";
            return txt;
        }
    }




}

    

