using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class WorldEditorOud : IGameObjectOud
    {
        public ServerClientMainOud engine;

        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private Cursor cursor;

        private Viewport viewPort;
        private MHGameWork.Game3DPlay.Core.Camera camera;
        private Quaternion cameraOrientation = Quaternion.Identity;

        private RenderTarget2D viewRenderTarget;
        //private Texture2D viewTexture;

        //private Rectangle totalRect;
        //private Rectangle viewRect;
        //private Rectangle menuLeft;

        private GuiPanel guiRoot;
        private GuiPanel guiRootFloating;
        private GuiPanel topMenu;
        private GuiPanel rightMenu;
        private GuiImage viewPanel;

        private GuiPanel fileMenu;

        private Vector3 raiseTerrainPosition;

        private List<TerrainOud> OUDterrains = new List<TerrainOud>();


        private List<Engine.Texture> terrainTextures = new List<Engine.Texture>();

        //Top menu
        private GuiKnop knpFile;
        private GuiKnop knpEdit;
        private GuiKnop knpView;

        private GuiKnop knpFileSave;
        private GuiKnop knpFileExit;

        //Right Menu
        private int selectedPaintTexture = -1;
        private GuiKnop knpViewTerrain;
        private GuiKnop knpCreateTerrain;
        private GuiKnop knpPaintTextures;
        private List<GuiKnop> paintTextureKnoppen = new List<GuiKnop>();
        //private GuiKnop knpPaintTexture1;
        //private GuiKnop knpPaintTexture2;
        //private GuiKnop knpPaintTexture3;
        //private GuiKnop knpPaintTexture4;
        //private GuiKnop knpPaintTexture5;
        //private GuiKnop knpPaintTexture6;
        //private GuiKnop knpPaintTexture7;
        //private GuiKnop knpPaintTexture8;

        private float brushRange = 50;

        private GuiPanel pointedObject = null;

        public enum TerrainMode
        {
            ViewTerrain = 0,
            CreateTerrain,
            PaintTextures
        }
        public TerrainMode activeTerrainMode = TerrainMode.CreateTerrain;

        public void GenerateTerrainViewData( TerrainOud terr )
        {

            List<TerrainMaterial> mats = new List<TerrainMaterial>();
            List<TerrainTexture> texts = new List<TerrainTexture>();

            terr.ViewWeightmaps.Clear();

            for ( int z = 0; z < terr.NumBlocksY; z++ )
            {
                for ( int x = 0; x < terr.NumBlocksX; x++ )
                {

                    TerrainBlock block = terr.GetEditorBlock( x, z );

                    //
                    //Generate vertex buffers
                    //

                    Vector3 min;
                    Vector3 max;
                    XNAGeoMipMap.VertexMultitextured[] vertices;
                    vertices = block.BaseBlock.GenerateVerticesFromHeightmap_New( terr.HeightMap, out min, out max );

                    block.ViewVertexBuffer = new VertexBuffer( terr.Device, typeof( VertexPositionNormalTexture ),
                                    vertices.Length, BufferUsage.None );

                    block.ViewVertexBuffer.SetData<XNAGeoMipMap.VertexMultitextured>( vertices );

                    block.BaseBlock.SetMinMax( min, max );

                    //
                    //Generate materials
                    //

                    //Find all textures in block
                    texts.Clear();
                    for ( int iTex = 0; iTex < terr.Textures.Count; iTex++ )
                    {
                        if ( BlockContainsTexture( block, iTex ) )
                        {
                            //TODO: TODO
                            texts.Add( terr.GetTexture( iTex ) );
                        }
                    }

                    TerrainMaterial mat = new TerrainMaterial( terr );
                    mat.Textures.AddRange( texts );
                    mat.Blocks.Add( block );
                    mats.Add( mat );


                }
            }


            //
            //Optimize materials
            //

            //Remove identical materials.
            int i = 0;
            while ( i < mats.Count )
            {
                TerrainMaterial iMat;
                iMat = mats[ i ];

                //Loop through all next materials and check if they are the same as iMat
                int iTarget = i + 1;
                while ( iTarget < mats.Count )
                {
                    TerrainMaterial targetMat;
                    targetMat = mats[ iTarget ];

                    if ( CheckMaterialsEqual( iMat, targetMat ) )
                    {
                        iMat.Blocks.AddRange( targetMat.Blocks );
                        mats.RemoveAt( iTarget );
                    }
                    else
                    {
                        iTarget++;
                    }


                }


                i++;
            }


            int maxTexNum = 0;

            terr.ViewMaterials.Clear();
            terr.ViewMaterials.AddRange( mats );

            for ( i = 0; i < mats.Count; i++ )
            {
                TerrainMaterial iMat;
                iMat = mats[ i ];
                for ( int iBlock = 0; iBlock < mats[ i ].Blocks.Count; iBlock++ )
                {
                    TerrainBlock block = mats[ i ].Blocks[ iBlock ];
                    if ( mats[ i ].Textures.Count == 0 )
                    {
                        block.ViewMaterial = null;
                    }
                    else
                    {
                        block.ViewMaterial = mats[ i ];

                    }
                }


                if ( mats[ i ].Textures.Count > maxTexNum ) maxTexNum = mats[ i ].Textures.Count;

                //Remove if empty
                if ( mats[ i ].Textures.Count == 0 )
                {
                    mats.RemoveAt( i );
                    i--;
                }
            }



            //
            // Construct Weightmaps
            //

            int numWeightmaps = (int)Math.Floor( (double)maxTexNum / 4 + 1 );

            for ( i = 0; i < numWeightmaps; i++ )
            {
                Color[] weights = new Color[ ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksX * ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksY ];

                for ( int z = 0; z < terr.NumBlocksY; z++ )
                {
                    for ( int x = 0; x < terr.NumBlocksX; x++ )
                    {
                        TerrainBlock block = terr.GetEditorBlock( x, z );

                        int blockMapX = x * ( terr.BaseTerrain.BlockSize + 1 );
                        int blockMapZ = z * ( terr.BaseTerrain.BlockSize + 1 );

                        for ( int iz = 0; iz <= terr.BaseTerrain.BlockSize; iz++ )
                        {
                            for ( int ix = 0; ix <= terr.BaseTerrain.BlockSize; ix++ )
                            {
                                int mapX = blockMapX + ix;
                                int mapZ = blockMapZ + iz;

                                int absX = block.X + ix;
                                int absZ = block.Z + iz;
                                byte r = 0;
                                byte g = 0;
                                byte b = 0;
                                byte a = 0;

                                if ( block.ViewMaterial != null )
                                {
                                    for ( int iTex = i * 4; iTex < block.ViewMaterial.Textures.Count && iTex < ( i + 1 ) * 4; iTex++ )
                                    {


                                        byte val = block.ViewMaterial.Textures[ iTex ].AlphaMap[ absX, absZ ];
                                        //if ( val != 0 ) throw new Exception();
                                        switch ( iTex - i * 4 )
                                        {
                                            case 0:
                                                r = val;
                                                break;
                                            case 1:
                                                g = val;
                                                break;
                                            case 2:
                                                b = val;
                                                break;
                                            case 3:
                                                a = val;
                                                break;
                                            default:
                                                break;
                                        }

                                    }
                                }
                                weights[ mapZ * ( ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksY ) + mapX ] = new Color( r, g, b, a );


                            }
                        }


                    }
                }
                Texture2D weightmap = new Texture2D( terr.Device, ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksX
                    , ( terr.BaseTerrain.BlockSize + 1 ) * terr.NumBlocksY, 0, TextureUsage.None, SurfaceFormat.Color );

                weightmap.SetData<Color>( weights );

                weightmap.Save( terr.BaseTerrain.Content.RootDirectory + @"\Content\Terrain\ViewWeightMapTest002(" + i + ").dds", ImageFileFormat.Dds );

                terr.ViewWeightmaps.Add( weightmap );
            }


            for ( i = 0; i < mats.Count; i++ )
            {
                TerrainMaterial iMat;
                iMat = mats[ i ];
                iMat.Load();
            }

        }

        public bool CheckMaterialsEqual( TerrainMaterial mat1, TerrainMaterial mat2 )
        {

            if ( mat1.Textures.Count != mat2.Textures.Count ) return false;

            //Check if textures are equal
            for ( int iTex = 0; iTex < mat1.Textures.Count; iTex++ )
            {
                if ( mat1.Textures[ iTex ] != mat2.Textures[ iTex ] ) return false;
            }

            return true;

        }

        public bool BlockContainsTexture( TerrainBlock block, int texIndex )
        {
            TerrainOud terr = block.TerrainOud;
            TerrainTexture tex = terr.GetTexture( texIndex );

            for ( int iz = block.Z; iz < block.Z + block.BlockSize; iz++ )
            {
                for ( int ix = block.X; ix < block.X + block.BlockSize; ix++ )
                {
                    if ( tex.AlphaMap[ ix, iz ] != 0 ) return true;
                }
            }

            return false;

        }


        /// <summary>
        /// Load in RAM
        /// </summary>
        public void Initialize()
        {
            //Check if this is a reload

            if ( spriteBatch == null )
            {
                //No reload, so load all the static content


                spriteBatch = new SpriteBatch( engine.XNAGame.GraphicsDevice );

                //
                //Load Font
                //
                spriteFont = engine.XNAGame.Content.Load<SpriteFont>( @"Content\ComicSansMS" );





                //Load some test terrain textures.
                terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\CrackedEarth001.dds" ) );
                terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\Grind001Seamless.dds" ) );
                terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\Grass001.dds" ) );
                terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\LeavesFloor001.dds" ) );

                terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\EarthRough001.dds" ) );
                terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\Grass0044_L.jpg" ) );
                terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\Grass0093_L.jpg" ) );
                terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\GrassDead0008_L.jpg" ) );


                terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\RockGrassy0010_L.jpg" ) );
                terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\RockRed0004_L(1024).jpg" ) );



                //
                //Load GUI
                //
                LoadGuiPanels();



                //
                //Load a temporary test terrain
                //

                TerrainOud terr = new TerrainOud( engine.Wereld.TerrainManager.Terrains[ 0 ] );
                terrains.Add( terr );
                //terr.SetFilename( engine.XNAGame._content.RootDirectory + "\\Content\\TerrainTest\\TestWorldEditor001" );

                //engine.Wereld.Tree.Split(); //1024
                //engine.Wereld.Tree.UpperLeft.Split(); // 512 OK!
                //engine.Wereld.Tree.UpperLeft.LowerRight.SetStatic( true );


                //terr.SetQuadTreeNode( engine.Wereld.Tree.UpperLeft.LowerRight );
                //terr.CreateTerrain( 32, 16, 16 );

                //terr.SetQuadTreeNode( engine.Wereld.Tree );
                //terr.CreateTerrain( 32, 64, 64 );


                for ( int i = 0; i < terrainTextures.Count; i++ )
                {

                    terrains[ 0 ].AddTexture( terrainTextures[ i ] );
                }







                //engine.Wereld.AddTerrain( terr );
                //engine.ServerMain.Wereld.AddTerrain( terr );

                //Cursor

                cursor = new Cursor( engine );
                cursor.Bounding = guiRoot.Bounding;
                cursor.Speed = 10;
                cursor.PointerPosition = new Vector2( 11, 9 );
                cursor.Load( engine.XNAGame.Content.RootDirectory + @"\Content\Cursor001.dds" );


                //Camera

                camera = new MHGameWork.Game3DPlay.Core.Camera( engine );

                camera.CameraPosition = new Vector3( 0, 200, 0 );

                cameraOrientation = Quaternion.CreateFromYawPitchRoll( 0, -MathHelper.PiOver4, 0 );
                UpdateCameraOrientation();

                viewPort = new Viewport();
                viewPort.X = viewPanel.Bounding.X;
                viewPort.Y = viewPanel.Bounding.Y;
                viewPort.Width = viewPanel.Bounding.Width;
                viewPort.Height = viewPanel.Bounding.Height;
                viewPort.MinDepth = 0;
                viewPort.MaxDepth = 1;

            }

            terrains[ 0 ].Initialize();
        }

        public void LoadGraphicsContent()
        {
            //
            //Load a temporary test terrain
            //


            terrains[ 0 ].LoadGraphicsContent();




            //
            //View rendertarget
            //
            if ( viewRenderTarget != null ) viewRenderTarget.Dispose();
            viewRenderTarget = new RenderTarget2D( engine.XNAGame.GraphicsDevice, viewPanel.Bounding.Width, viewPanel.Bounding.Height, 1, SurfaceFormat.Color
                , GraphicsDevice.PresentationParameters.MultiSampleType
                , GraphicsDevice.PresentationParameters.MultiSampleQuality );




        }

        public void LoadGuiPanels()
        {
            //
            //GUI Root
            //
            guiRoot = new GuiPanel( 0, 0, engine.XNAGame.Window.ClientBounds.Width, engine.XNAGame.Window.ClientBounds.Height );

            topMenu = new GuiPanel();
            topMenu.SetSides( guiRoot.Left, guiRoot.Right, guiRoot.Top, 30 );

            rightMenu = new GuiPanel();
            rightMenu.SetSides( guiRoot.Right - 200, guiRoot.Right, topMenu.Bottom, guiRoot.Bottom );

            viewPanel = new GuiImage();
            viewPanel.SetSides( guiRoot.Left, rightMenu.Left, topMenu.Bottom, guiRoot.Bottom );

            guiRoot.AddChild( topMenu );
            guiRoot.AddChild( rightMenu );
            guiRoot.AddChild( viewPanel );


            //
            //GUI Root Floating (for floating menu's the should be rendered on top)
            //
            guiRootFloating = new GuiPanel( guiRoot.Left, guiRoot.Top, guiRoot.Width, guiRoot.Height );

            //Floating menu's
            fileMenu = new GuiPanel( 200, 200, 200, 300 );

            guiRootFloating.AddChild( fileMenu );



            //
            //Top Menu
            //
            knpFile = CreateKnop001( topMenu.Left, topMenu.Top, 60, topMenu.Bottom );
            knpEdit = CreateKnop001( knpFile.Right, topMenu.Top, 60, topMenu.Bottom );
            knpView = CreateKnop001( knpEdit.Right, topMenu.Top, 60, topMenu.Bottom );

            knpFile.Text = "File";
            knpEdit.Text = "Edit";
            knpView.Text = "View";

            topMenu.AddChild( knpFile );
            topMenu.AddChild( knpEdit );
            topMenu.AddChild( knpView );

            //
            //Right Menu
            //
            knpViewTerrain = CreateKnop001( rightMenu.Bounding.Left + 10, rightMenu.Bounding.Top + 10, 170, 50 );
            knpViewTerrain.Text = "View Terrain";
            knpCreateTerrain = CreateKnop001( knpViewTerrain.Bounding.Left + 10, knpViewTerrain.Bounding.Bottom + 10, 170, 50 );
            knpCreateTerrain.Text = "Create Terrain";
            knpPaintTextures = CreateKnop001( knpCreateTerrain.Bounding.Left, knpCreateTerrain.Bounding.Bottom + 10, 170, 50 );
            knpPaintTextures.Text = "Paint Textures";


            paintTextureKnoppen.Add( CreateKnop001( knpPaintTextures.Bounding.Left + 10, knpPaintTextures.Bounding.Bottom + 40, 160, 50 ) );
            paintTextureKnoppen[ 0 ].Text = "Texture 1";

            rightMenu.AddChild( paintTextureKnoppen[ 0 ] );

            for ( int i = 1; i < terrainTextures.Count; i++ )
            {
                paintTextureKnoppen.Add( CreateKnop001( paintTextureKnoppen[ i - 1 ].Bounding.Left, paintTextureKnoppen[ i - 1 ].Bounding.Bottom + 10, 160, 50 ) );
                paintTextureKnoppen[ i ].Text = "Texture " + ( i + 1 ).ToString();
                rightMenu.AddChild( paintTextureKnoppen[ i ] );
            }

            rightMenu.AddChild( knpViewTerrain );
            rightMenu.AddChild( knpCreateTerrain );
            rightMenu.AddChild( knpPaintTextures );



            //
            //File Menu
            //
            fileMenu.Enabled = false;
            fileMenu.SetPosition( knpFile.Left, knpFile.Bottom );
            fileMenu.Opaque = true;
            fileMenu.BackgroundColor = Color.Blue;


            knpFileSave = CreateKnop001( fileMenu.Left, fileMenu.Top, fileMenu.Width, 30 );
            knpFileSave.Text = "Save Terrain";
            knpFileExit = CreateKnop001( fileMenu.Left, knpFileSave.Bottom, fileMenu.Width, 30 );
            knpFileExit.Text = "Exit Editor";

            fileMenu.AddChild( knpFileSave );
            fileMenu.AddChild( knpFileExit );



            //menuLeft = new Rectangle( 0, 0, 200, totalRect.Height );

            //viewRect = new Rectangle( menuLeft.Right, 0, totalRect.Right - menuLeft.Right, totalRect.Bottom );

        }

        public GuiKnop CreateKnop001( int x, int y, int width, int height )
        {
            GuiKnop knp = new GuiKnop( x, y, width, height );

            knp.Texture = Texture2D.FromFile( engine.XNAGame.GraphicsDevice, engine.XNAGame.Content.RootDirectory + @"\Content\Knop001.dds" );
            knp.SourceRectangle = new Rectangle( 22, 16, 238 - 22, 81 - 16 );
            knp.Font = spriteFont;

            return knp;
        }

        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {

        }

        public void ProcessOld( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            engine.XNAGame.Window.Title = "The Wizards - Editor      FPS: " + engine.FPS.ToString();

            //Reload the terrain if L is pressed
            if ( e.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
            {
                for ( int i = 0; i < terrains.Count; i++ )
                {
                    terrains[ i ].Initialize();

                    terrains[ i ].LoadGraphicsContent();
                }
            }

            viewPanel.Enabled = true;

            //Update cursor
            cursor.Process( e );

            GuiPanel obj = guiRootFloating.FindPointedObject( cursor.GetClickPoint() );
            if ( obj == null || obj == guiRootFloating )
            {
                obj = guiRoot.FindPointedObject( cursor.GetClickPoint() );
            }

            pointedObject = obj;



            //Cursor checks

            if ( obj == viewPanel && e.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftAlt ) )
            {
                //
                // Mouse is over the viewPanel
                //

                // Do camera movement
                ProcessViewPanelCameraMovement( e );
            }
            else
            {

                //Process depending on the current mode

                switch ( activeTerrainMode )
                {
                    case TerrainMode.CreateTerrain:
                        RaiseTerrainMethod2( e );
                        break;
                    case TerrainMode.PaintTextures:
                        PaintTextures( e );
                        break;


                }
            }
            if ( obj != viewPanel )
            {
                //Cursor is not on the viewPanel, so process the GUI buttons etc


                if ( e.Mouse.LeftMouseJustPressed )
                {
                    //Check buttons
                    if ( obj == knpViewTerrain )
                    {
                        //Enable View Terrain Mode
                        ChangeTerrainMode( TerrainMode.ViewTerrain );
                    }
                    else if ( obj == knpCreateTerrain )
                    {
                        //Enable Create Terrain Mode
                        ChangeTerrainMode( TerrainMode.CreateTerrain );
                    }
                    else if ( obj == knpPaintTextures )
                    {
                        //Enable Paint Textures Terrain Mode
                        ChangeTerrainMode( TerrainMode.PaintTextures );
                    }
                }




            }
            //Check for the topMenu
            ProcessTopMenu( e );


            for ( int i = 0; i < terrains.Count; i++ )
            {
                terrains[ i ].BaseTerrain.Frustum = camera.CameraInfo.Frustum;
                terrains[ i ].BaseTerrain.CameraPostion = camera.CameraPosition;
                terrains[ i ].Update();
            }

            engine.Wereld.Process( e );
        }

        public void ProcessTopMenu( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            if ( pointedObject == knpFile )
            {
                fileMenu.Enabled = true;
            }
            else if ( !fileMenu.PointerInPanel( cursor.GetClickPoint() ) )
            {
                fileMenu.Enabled = false;
            }

            if ( pointedObject == knpEdit )
            {
                //fileMenu.Enabled = true;
            }
            else if ( !fileMenu.PointerInPanel( cursor.GetClickPoint() ) )
            {
                //fileMenu.Enabled = false;
            }

            if ( pointedObject == knpView )
            {
                //fileMenu.Enabled = true;
            }
            else if ( !fileMenu.PointerInPanel( cursor.GetClickPoint() ) )
            {
                //fileMenu.Enabled = false;
            }

            ProcessFileMenu( e );

        }

        public void ProcessFileMenu( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            if ( !fileMenu.Enabled ) return;

            if ( e.Mouse.LeftMouseJustPressed )
            {
                if ( pointedObject == knpFileSave )
                {
                    //Temporary
                    terrains[ 0 ].Save();

                }
                else if ( pointedObject == knpFileExit )
                {
                    engine.Exit();
                }
            }
        }

        public void PaintTextures( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            if ( e.Mouse.LeftMouseJustPressed )
            {
                for ( int i = 0; i < paintTextureKnoppen.Count; i++ )
                {
                    if ( pointedObject == paintTextureKnoppen[ i ] )
                    {
                        selectedPaintTexture = i;
                    }
                }
            }



            if ( pointedObject == viewPanel )
            {

                if ( e.Mouse.RelativeScrollWheel > 0 )
                {
                    brushRange += 2;//e.Mouse.RelativeScrollWheel;
                }
                if ( e.Mouse.RelativeScrollWheel < 0 )
                {
                    brushRange -= 2;// e.Mouse.RelativeScrollWheel;
                }
                if ( selectedPaintTexture != -1 )
                {

                    TerrainOud hitTerrainOud;
                    Vector3 hit;
                    if ( RaycastTerrains( CalculateViewPanelRay( cursor.Position ), out hitTerrainOud, out hit ) )
                    {
                        engine.LineManager3D.AddLine( new Vector3( hit.X, hit.Y, hit.Z ), new Vector3( hit.X, hit.Y + 50, hit.Z ), Color.Green );
                        for ( int i = 0; i < terrains.Count; i++ )
                        {
                            terrains[ i ].Brush = new Brush( hit, brushRange );
                            terrains[ i ].BrushTexture = terrainTextures[ selectedPaintTexture ];
                        }
                        if ( e.Mouse.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
                             )
                        {

                            for ( int i = 0; i < terrains.Count; i++ )
                            {
                                //Find the texture
                                int texIndex = terrains[ i ].FindCorrespondingTextureIndex( terrainTextures[ selectedPaintTexture ] );

                                terrains[ i ].PaintWeight( hit.X, hit.Z, brushRange, texIndex, e.Elapsed * 100 );
                                //terrains[ i ].RaiseTerrain( new Vector2( raiseTerrainPosition.X, raiseTerrainPosition.Z ), 50, e.Mouse.RelativeY * 10f );
                                //terrains[ i ].UpdateDirtyVertexbuffers();
                            }
                        }
                    }



                }
            }
        }

        public void ChangeTerrainMode( TerrainMode newMode )
        {
            TerrainMode oldMode = activeTerrainMode;
            //Do termination of the previous mode


            //Activate the new mode type
            activeTerrainMode = newMode;
            switch ( newMode )
            {
                case TerrainMode.ViewTerrain:
                    GenerateTerrainViewData( terrains[ 0 ] );
                    break;
                case TerrainMode.CreateTerrain:
                    break;
                case TerrainMode.PaintTextures:
                    break;
            }
        }

        public void RaiseTerrainMethod1( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            //Temp
            //if (e.Keyboard.IsKeyStateDown(Microsoft.Xna.Framework.Input.Keys.L))
            //{
            //    for ( int i = 0; i < terrains.Count; i++ )
            //    {
            //        terrains[ i ].effect.Load( engine.XNAGame.Content );
            //    }
            //}




            if ( pointedObject == viewPanel )
            {

                if ( e.Mouse.RelativeScrollWheel > 0 )
                {
                    brushRange += 2;//e.Mouse.RelativeScrollWheel;
                }
                if ( e.Mouse.RelativeScrollWheel < 0 )
                {
                    brushRange -= 2;// e.Mouse.RelativeScrollWheel;
                }

                TerrainOud hitTerrainOud;
                Vector3 hit;
                if ( RaycastTerrains( CalculateViewPanelRay( cursor.Position ), out hitTerrainOud, out hit ) )
                {
                    engine.LineManager3D.AddLine( new Vector3( hit.X, hit.Y, hit.Z ), new Vector3( hit.X, hit.Y + 50, hit.Z ), Color.Green );

                }


                if ( e.Mouse.LeftMouseJustPressed )
                {
                    raiseTerrainPosition = hit;
                    for ( int i = 0; i < terrains.Count; i++ )
                    {
                        terrains[ i ].Brush = new Brush( hit, brushRange );
                    }
                }



                if ( e.Mouse.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
                {

                    for ( int i = 0; i < terrains.Count; i++ )
                    {
                        terrains[ i ].RaiseTerrain( new Vector2( raiseTerrainPosition.X, raiseTerrainPosition.Z ), brushRange, e.Mouse.OppositeRelativeY * 10f );
                        terrains[ i ].UpdateDirtyVertexbuffers();
                    }
                }
                else
                {

                    for ( int i = 0; i < terrains.Count; i++ )
                    {
                        terrains[ i ].Brush = new Brush( hit, brushRange );
                    }
                }
            }
        }
        public void RaiseTerrainMethod2( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            //Temp
            //if (e.Keyboard.IsKeyStateDown(Microsoft.Xna.Framework.Input.Keys.L))
            //{
            //    for ( int i = 0; i < terrains.Count; i++ )
            //    {
            //        terrains[ i ].effect.Load( engine.XNAGame.Content );
            //    }
            //}




            if ( pointedObject == viewPanel )
            {

                if ( e.Mouse.RelativeScrollWheel > 0 )
                {
                    brushRange += 2;//e.Mouse.RelativeScrollWheel;
                }
                if ( e.Mouse.RelativeScrollWheel < 0 )
                {
                    brushRange -= 2;// e.Mouse.RelativeScrollWheel;
                }

                TerrainOud hitTerrainOud;
                Vector3 hit;
                if ( RaycastTerrains( CalculateViewPanelRay( cursor.Position ), out hitTerrainOud, out hit ) )
                {
                    engine.LineManager3D.AddLine( new Vector3( hit.X, hit.Y, hit.Z ), new Vector3( hit.X, hit.Y + 50, hit.Z ), Color.Green );

                }



                raiseTerrainPosition = hit;
                for ( int i = 0; i < terrains.Count; i++ )
                {
                    terrains[ i ].Brush = new Brush( hit, brushRange );
                }


                if ( e.Mouse.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
                {


                    for ( int i = 0; i < terrains.Count; i++ )
                    {
                        terrains[ i ].RaiseTerrain( new Vector2( raiseTerrainPosition.X, raiseTerrainPosition.Z ), brushRange, e.Elapsed * 10 );
                        terrains[ i ].UpdateDirtyVertexbuffers();
                        terrains[ i ].UpdateDirtyServerBlocks();
                        terrains[ i ].UpdateDirtyHeightMapBlocks();
                    }
                }

                if ( e.Mouse.MouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
                {


                    for ( int i = 0; i < terrains.Count; i++ )
                    {
                        terrains[ i ].RaiseTerrain( new Vector2( raiseTerrainPosition.X, raiseTerrainPosition.Z ), brushRange, -e.Elapsed * 200 );
                        terrains[ i ].UpdateDirtyVertexbuffers();
                        terrains[ i ].UpdateDirtyServerBlocks();
                        terrains[ i ].UpdateDirtyHeightMapBlocks();
                    }
                }

            }
        }
        /// <summary>
        /// Terreinen moeten nog in de juiste volgorde geraycast worden.
        /// </summary>
        /// <param name="terrainOud"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        private bool RaycastTerrains( Ray ray, out TerrainOud terrainOud, out Vector3 hit )
        {

            bool ret;
            terrainOud = null;
            hit = Vector3.Zero;

            for ( int i = 0; i < terrains.Count; i++ )
            {
                ret = terrains[ i ].Raycast( ray, out hit );
                if ( ret == true )
                {
                    terrainOud = terrains[ i ];
                    return true;
                }
            }

            return false;

        }

        public Ray CalculateViewPanelRay( Vector2 mousecoords )
        {
            Viewport viewport = viewPort;

            Vector3 nearSource = viewport.Unproject( new Vector3( mousecoords.X, mousecoords.Y, viewport.MinDepth ), camera.CameraInfo.ProjectionMatrix, camera.CameraInfo.ViewMatrix, Matrix.Identity );
            Vector3 farSource = viewport.Unproject( new Vector3( mousecoords.X, mousecoords.Y, viewport.MaxDepth ), camera.CameraInfo.ProjectionMatrix, camera.CameraInfo.ViewMatrix, Matrix.Identity );
            Vector3 direction = farSource - nearSource;
            direction.Normalize();

            Ray ray = new Ray( nearSource, direction );

            return ray;

        }

        private void ProcessViewPanelCameraMovement( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {

            if ( e.Mouse.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
               && e.Mouse.MouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
            {
                camera.CameraPosition += Vector3.Up * ( -e.Mouse.OppositeRelativeY * 10 );
            }
            else
            {
                if ( e.Mouse.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
                {
                    //Matrix rot = Matrix.Identity;
                    //Quaternion rot = Quaternion.Identity;
                    if ( e.Mouse.OppositeRelativeX != 0.0f )
                    {
                        //rot *= Matrix.CreateFromAxisAngle( Vector3.Up, e.Mouse.RelativeX * 0.01f );
                        cameraOrientation = Quaternion.CreateFromAxisAngle( Vector3.Up, -e.Mouse.OppositeRelativeX * 0.01f ) * cameraOrientation;
                    }
                    if ( e.Mouse.OppositeRelativeY != 0.0f )
                    {
                        //rot *= Matrix.CreateFromAxisAngle( Vector3.Cross( camera.CameraUp, camera.CameraDirection ), -e.Mouse.RelativeY * 0.01f ) * rot;
                        //rot = Quaternion.CreateFromAxisAngle( Vector3.Cross( camera.CameraUp, camera.CameraDirection ), -e.Mouse.RelativeY * 0.01f ) * rot;
                        cameraOrientation = Quaternion.CreateFromAxisAngle( Vector3.Transform( Vector3.Right, cameraOrientation ), -e.Mouse.OppositeRelativeY * 0.01f ) * cameraOrientation;

                    }
                    //rot.Normalize();
                    //camera.CameraDirection = Vector3.Transform( camera.CameraDirection, rot );
                    //camera.CameraUp = Vector3.Transform( camera.CameraUp, rot );
                    UpdateCameraOrientation();

                }


                if ( e.Mouse.MouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
                {
                    //camera.CameraPosition +=  new Vector3( -e.Mouse.RelativeX * 10, 0, -e.Mouse.RelativeY * 10 );
                    Vector3 forward = Vector3.Transform( Vector3.Forward, cameraOrientation );
                    Vector3 right = Vector3.Transform( Vector3.Right, cameraOrientation );

                    forward.Y = 0;
                    right.Y = 0;


                    camera.CameraPosition += forward * -e.Mouse.OppositeRelativeY * 10;
                    camera.CameraPosition += right * e.Mouse.OppositeRelativeX * 10;
                }
            }

            //****
            //* Different camera styles. can be implemented later
            //**
            //*
            //if ( e.Mouse.MouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
            //{
            //    GuiPanel obj = guiRoot.FindClickedObject( cursor.GetClickPoint() );

            //    if ( obj == viewPanel )
            //    {
            //        camera.CameraPosition += camera.CameraUp * e.Mouse.RelativeY * 10;
            //        camera.CameraPosition += Vector3.Cross( camera.CameraUp, camera.CameraDirection ) * e.Mouse.RelativeX * 10;
            //    }
            //}
            //if ( e.Mouse.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
            //{
            //    Quaternion rot = Quaternion.Identity;
            //    if ( e.Mouse.RelativeX != 0.0f )
            //    {
            //        rot = Quaternion.CreateFromAxisAngle( camera.CameraUp, e.Mouse.RelativeX * 0.01f ) * rot;
            //    }
            //    if ( e.Mouse.RelativeY != 0.0f )
            //    {
            //        rot = Quaternion.CreateFromAxisAngle( Vector3.Cross( camera.CameraUp, camera.CameraDirection ), -e.Mouse.RelativeY * 0.01f ) * rot;
            //    }

            //    camera.CameraDirection = Vector3.Transform( camera.CameraDirection, rot );
            //    camera.CameraUp = Vector3.Transform( camera.CameraUp, rot );
            //}



            //    }

        }

        public void UpdateCameraOrientation()
        {
            camera.CameraDirection = Vector3.Transform( Vector3.Forward, cameraOrientation );
            camera.CameraUp = Vector3.Transform( Vector3.Up, cameraOrientation );
        }

        public void RenderOld()
        {
            if ( !Initialized ) return;

            // Render view panel

            camera.UpdateCameraInfo();
            engine.SetCamera( camera );

            //for ( int i = 0; i < terrains.Count; i++ )
            //{
            //    terrains[ i ].Frustum = camera.CameraInfo.Frustum;
            //    terrains[ i ].CameraPostion = camera.CameraPosition;
            //    terrains[ i ].RenderWeightPaintMode();



            //}
            //GraphicsDevice.Indices = null;
            //GraphicsDevice.Vertices[ 0 ].SetSource( null, 0, 0 );
            //return;



            GraphicsDevice.Viewport = viewPort;

            GraphicsDevice.SetRenderTarget( 0, viewRenderTarget );


            //
            //START OF RENDERING VIEWPANEL
            //
            if ( engine.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.J ) )
            {
                engine.XNAGame.GraphicsDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0 );
            }
            else
            {

                engine.XNAGame.GraphicsDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1, 0 );
            }
            engine.Wereld.UpdateNodeVisible( camera.CameraInfo.Frustum, engine.Wereld.Tree, false );
            engine.Wereld.RenderEntities();

            for ( int i = 0; i < terrains.Count; i++ )
            {
                //terrains[ i ].Frustum = camera.CameraInfo.Frustum;
                //terrains[ i ].CameraPostion = camera.CameraPosition;
                if ( activeTerrainMode == TerrainMode.ViewTerrain )
                {
                    terrains[ i ].RenderViewMode();
                }
                else if ( activeTerrainMode == TerrainMode.CreateTerrain )
                {

                    terrains[ i ].RenderEditHeightsMode();
                }
                else if ( activeTerrainMode == TerrainMode.PaintTextures )
                {
                    terrains[ i ].RenderWeightPaintMode();
                }
                GraphicsDevice.Indices = null;
                GraphicsDevice.Vertices[ 0 ].SetSource( null, 0, 0 );
            }

            GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            //Render X and Z Axis
            engine.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 2000, 0, 0 ), Color.Red );
            engine.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 2000 ), Color.Blue );




            //Render Quadtree
            engine.Wereld.Tree.RenderNodeBoundingBox();
            //engine.Wereld.Tree.RenderNodeEntityBoundingBox();

            //Render Lines
            engine.LineManager3D.Render();

            //Render Physics Debug Data
            engine.serverDebugRenderer.OnRender( this, engine._renderEventArgs );


            //
            //END OF RENDERING VIEWPANEL
            //
            GraphicsDevice.SetRenderTarget( 0, null );

            viewPanel.Texture = viewRenderTarget.GetTexture();




            //
            //
            //
            // Render Editor
            //
            //
            //

            //Dont render viewPanel
            viewPanel.Enabled = false;

            engine.XNAGame.GraphicsDevice.Clear( ClearOptions.DepthBuffer | ClearOptions.Target, Color.Yellow, 1, 0 );
            //spriteBatch.Begin( SpriteBlendMode.None );





            spriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState );
            {
                guiRoot.Draw( spriteBatch );

                spriteBatch.End();
            }

            //Draw viewPanel without alpha blending!!!
            spriteBatch.Begin( SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState );
            {
                viewPanel.Draw( spriteBatch );
            }
            spriteBatch.End();
            spriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState );
            {
                guiRootFloating.Draw( spriteBatch );
                cursor.Render( spriteBatch );
            }
            spriteBatch.End();





        }

        public GraphicsDevice GraphicsDevice
        { get { return engine.XNAGame.GraphicsDevice; } }








        ///////////// NEW ////////////////

        private Engine.GameFileOud editorIniFile;

        private EditorIniFile iniFile;

        public WorldEditorOud( ServerClientMainOud nEngine )
        {
            engine = nEngine;

        }

        Engine.LoadTaskRef initializeTaskRef;

        private List<TerrainOud> terrains = new List<TerrainOud>();

        public void LaadEditorIniFile()
        {
            //Should always be up to date since this is a client file.

            iniFile = EditorIniFile.Load( editorIniFile );

            for ( int i = 0; i < iniFile.Terrains.Count; i++ )
            {
                EditorIniFile.Terrain terrData = iniFile.Terrains[ i ];

                TerrainOud terr = new TerrainOud( engine, engine.GameFileManager.FindGameFile( terrData.TerrainInfoFileID ) );
            }
        }

        public Engine.LoadingTaskState InitializeTask( Engine.LoadingTaskType nType )
        {
            // load all the static content


            spriteBatch = new SpriteBatch( engine.XNAGame.GraphicsDevice );

            //
            //Load Font
            //
            spriteFont = engine.XNAGame.Content.Load<SpriteFont>( @"Content\ComicSansMS" );





            //Load some test terrain textures.
            //terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\CrackedEarth001.dds" ) );
            //terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\Grind001Seamless.dds" ) );
            //terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\Grass001.dds" ) );
            //terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\LeavesFloor001.dds" ) );

            //terrainTextures.Add( Engine.Texture.FromFile( engine, engine.XNAGame._content.RootDirectory + @"\Content\EarthRough001.dds" ) );
            //terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\Grass0044_L.jpg" ) );
            //terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\Grass0093_L.jpg" ) );
            //terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\GrassDead0008_L.jpg" ) );


            //terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\RockGrassy0010_L.jpg" ) );
            //terrainTextures.Add( Engine.Texture.FromFile( engine, @"E:\Textures\RockRed0004_L(1024).jpg" ) );



            //
            //Load GUI
            //
            LoadGuiPanels();



            //
            //Load a temporary test terrain
            //

            //Terrain terr = new Terrain( engine.Wereld.TerrainManager.Terrains[ 0 ] );
            //terrains.Add( terr );



            ////terr.SetFilename( engine.XNAGame._content.RootDirectory + "\\Content\\TerrainTest\\TestWorldEditor001" );

            ////engine.Wereld.Tree.Split(); //1024
            ////engine.Wereld.Tree.UpperLeft.Split(); // 512 OK!
            ////engine.Wereld.Tree.UpperLeft.LowerRight.SetStatic( true );


            ////terr.SetQuadTreeNode( engine.Wereld.Tree.UpperLeft.LowerRight );
            ////terr.CreateTerrain( 32, 16, 16 );

            ////terr.SetQuadTreeNode( engine.Wereld.Tree );
            ////terr.CreateTerrain( 32, 64, 64 );


            //for ( int i = 0; i < terrainTextures.Count; i++ )
            //{

            //    terrains[ 0 ].AddTexture( terrainTextures[ i ] );
            //}







            //engine.Wereld.AddTerrain( terr );
            //engine.ServerMain.Wereld.AddTerrain( terr );

            //Cursor

            //cursor = new Cursor( engine );
            //cursor.Bounding = guiRoot.Bounding;
            //cursor.Speed = 10;
            //cursor.PointerPosition = new Vector2( 11, 9 );
            //cursor.Load( engine.XNAGame.Content.RootDirectory + @"\Content\Cursor001.dds" );


            //Camera

            camera = new MHGameWork.Game3DPlay.Core.Camera( engine );

            camera.CameraPosition = new Vector3( 0, 200, 0 );

            cameraOrientation = Quaternion.CreateFromYawPitchRoll( 0, -MathHelper.PiOver4, 0 );
            UpdateCameraOrientation();

            viewPort = new Viewport();
            viewPort.X = viewPanel.Bounding.X;
            viewPort.Y = viewPanel.Bounding.Y;
            viewPort.Width = viewPanel.Bounding.Width;
            viewPort.Height = viewPanel.Bounding.Height;
            viewPort.MinDepth = 0;
            viewPort.MaxDepth = 1;


            //
            //View rendertarget
            //
            if ( viewRenderTarget != null ) viewRenderTarget.Dispose();
            viewRenderTarget = new RenderTarget2D( engine.XNAGame.GraphicsDevice, viewPanel.Bounding.Width, viewPanel.Bounding.Height, 1, SurfaceFormat.Color
                , GraphicsDevice.PresentationParameters.MultiSampleType
                , GraphicsDevice.PresentationParameters.MultiSampleQuality );



            //terrains[ 0 ].Initialize();


            LaadEditorIniFile();


            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;
        }


        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            if ( !Initialized )
            {
                //Not yet initialized.

                if ( initializeTaskRef.IsEmpty )
                {
                    initializeTaskRef = engine.LoadingManager.AddLoadTaskAdvanced( InitializeTask, Engine.LoadingTaskType.Normal );
                }
                else if ( initializeTaskRef.State == Engine.LoadingTaskState.Completed )
                {
                    //shouldnt happen, because this if wouldn't be entered otherwise
                    throw new Exception();
                }
            }
        }




        public void Render()
        {
            if ( !Initialized ) return;



            // Render view panel

            camera.UpdateCameraInfo();
            engine.SetCamera( camera );

            //for ( int i = 0; i < terrains.Count; i++ )
            //{
            //    terrains[ i ].Frustum = camera.CameraInfo.Frustum;
            //    terrains[ i ].CameraPostion = camera.CameraPosition;
            //    terrains[ i ].RenderWeightPaintMode();



            //}
            //GraphicsDevice.Indices = null;
            //GraphicsDevice.Vertices[ 0 ].SetSource( null, 0, 0 );
            //return;



            GraphicsDevice.Viewport = viewPort;

            GraphicsDevice.SetRenderTarget( 0, viewRenderTarget );


            //
            //START OF RENDERING VIEWPANEL
            //
            if ( engine.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.J ) )
            {
                engine.XNAGame.GraphicsDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0 );
            }
            else
            {

                engine.XNAGame.GraphicsDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1, 0 );
            }
            engine.Wereld.UpdateNodeVisible( camera.CameraInfo.Frustum, engine.Wereld.Tree, false );
            engine.Wereld.RenderEntities();

            for ( int i = 0; i < terrains.Count; i++ )
            {
                //terrains[ i ].Frustum = camera.CameraInfo.Frustum;
                //terrains[ i ].CameraPostion = camera.CameraPosition;
                if ( activeTerrainMode == TerrainMode.ViewTerrain )
                {
                    terrains[ i ].RenderViewMode();
                }
                else if ( activeTerrainMode == TerrainMode.CreateTerrain )
                {

                    terrains[ i ].RenderEditHeightsMode();
                }
                else if ( activeTerrainMode == TerrainMode.PaintTextures )
                {
                    terrains[ i ].RenderWeightPaintMode();
                }
                GraphicsDevice.Indices = null;
                GraphicsDevice.Vertices[ 0 ].SetSource( null, 0, 0 );
            }

            GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            //Render X and Z Axis
            engine.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 2000, 0, 0 ), Color.Red );
            engine.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 2000 ), Color.Blue );




            //Render Quadtree
            engine.Wereld.Tree.RenderNodeBoundingBox();
            //engine.Wereld.Tree.RenderNodeEntityBoundingBox();

            //Render Lines
            engine.LineManager3D.Render();

            //Render Physics Debug Data
            engine.serverDebugRenderer.OnRender( this, engine._renderEventArgs );


            //
            //END OF RENDERING VIEWPANEL
            //
            GraphicsDevice.SetRenderTarget( 0, null );

            viewPanel.Texture = viewRenderTarget.GetTexture();




            //
            //
            //
            // Render Editor
            //
            //
            //

            //Dont render viewPanel
            viewPanel.Enabled = false;

            engine.XNAGame.GraphicsDevice.Clear( ClearOptions.DepthBuffer | ClearOptions.Target, Color.Yellow, 1, 0 );
            //spriteBatch.Begin( SpriteBlendMode.None );





            spriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState );
            {
                guiRoot.Draw( spriteBatch );

                spriteBatch.End();
            }

            //Draw viewPanel without alpha blending!!!
            spriteBatch.Begin( SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState );
            {
                viewPanel.Draw( spriteBatch );
            }
            spriteBatch.End();
            spriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState );
            {
                guiRootFloating.Draw( spriteBatch );
                //cursor.Render( spriteBatch );
            }
            spriteBatch.End();





        }



        private bool Initialized { get { return spriteBatch != null; } }



        public static void TestWorldEditor001()
        {
            TestServerClientMain main = null;

            Editor.WorldEditorOud worldEditor = null;

            //bool started = false;


            TestServerClientMain.Start( "TestWorldEditor001",
                delegate
                {
                    main = TestServerClientMain.Instance;

                    main.XNAGame.Graphics.PreferredBackBufferWidth = 1280;
                    main.XNAGame.Graphics.PreferredBackBufferHeight = 1024;
                    main.XNAGame.Graphics.ApplyChanges();

                    main.CursorEnabled = true;

                    worldEditor = new MHGameWork.TheWizards.ServerClient.Editor.WorldEditorOud( main );

                    worldEditor.editorIniFile = new Engine.GameFileOud( main.RootDirectory + @"\Content\EditorIniTest.xml" );



                },
                delegate
                {
                    main.LoadingManager.ProcessNextTask( MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
                    //if ( started == false )
                    //{
                    //    worldEditor.LoadGraphicsContent();
                    //    started = true;
                    //}

                    worldEditor.Process( main.ProcessEventArgs );

                    //if ( main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.B ) )
                    //{
                    //    main.serverDebugRenderer.Enabled = true;
                    //}
                    //else
                    //{
                    //    main.serverDebugRenderer.Enabled = false;
                    //}



                    worldEditor.Render();


                } );

        }




    }
}
