using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NovodexWrapper;

namespace MHGameWork.TheWizards.ServerClient
{
    public static class ProgramOud
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            //TestServerClientMain.TestEmptyGame();
            //Wereld.QuadTree.TestQuadTreeStructure();
            //Engine.Model.TestRenderModel();
            //Engine.ModelManager.TestRenderMultipleSameModels();
            //Wereld.QuadTree.TestOrdenEntities();
            //Engine.LineManager3D.TestRenderLines();
            //Wereld.QuadTree.TestRender();
            //TestServerQuadtree();
            //XNAGeoMipMap.Terrain.TestSaveTerrain();
            //XNAGeoMipMap.Terrain.TestLoadTerrain();
            //TestServerEntities001();
            //TestTerrainCollision001();
            //TestOcean001();
            //XNAGeoMipMap.Water001.TestRenderWaterPlane();
            //XNAGeoMipMap.Water002.TestRefractionMap();
            //XNAGeoMipMap.Water002.TestReflectionMap();
            //XNAGeoMipMap.Water002.TestRenderWater();
            //XNAGeoMipMap.Skydome.TestRenderSkydome();
            //TestWaterSkyCollision001();
            //TestTerrainCollision002(); 
            //XNAGeoMipMap.Terrain.TestCreateTerrain();
            //XNAGeoMipMap.Terrain.TestMultipleTerrains();
            //XNAGeoMipMap.Terrain.TestCalculateErrors();

            //TestServerClientNetwork001();
            //TestWorld001();
            //TestServerQuadtreeCollision();

            //Engine.ColladaModel.TestColladaModel001();
            //Engine.ColladaModel.TestShowBones();
            //Engine.ColladaModel.TestColladaModelSkinned();
            //Engine.ColladaModel.TestColladaModelSkinnedSimple();

            //TestPlayerInput001();

            //Editor.WorldEditorOud.TestWorldEditor001();

            //ServerClientGame.TestServerClientGame001();

            //Engine.TextureManager.TestTextureManager001();

            //Engine.GameFileManager.TestSaveToDisk();

            //GameSettings.TestSaveSettings();

            //Common.GeoMipMap.TerrainInfo.TestSaveToDisk();

            //Gui.GuiControl.TestRender001();
            //Gui.GuiWindow.TestRenderWindow001();
            //XNAGeoMipMap.SkyBox001.TestRenderSkybox001();

            //Common.GeoMipMap.TerrainListFile.TestSaveTerrainListFile();

            //RunGame(); //Start the game in normal mode
        }

        public static void RunGame()
        {
            GameServerClientMain main = new GameServerClientMain();

            main.XNAGame.Graphics.PreferredBackBufferWidth = 1280;
            main.XNAGame.Graphics.PreferredBackBufferHeight = 1024;

            main.XNAGame.Graphics.ApplyChanges();

            main.Run();
        }


#if DEBUG
        public static void TestServerQuadtree()
        {
            Server.Wereld.QuadTree tree =
                new MHGameWork.TheWizards.Server.Wereld.QuadTree( new Microsoft.Xna.Framework.BoundingBox(
                                    new Microsoft.Xna.Framework.Vector3( 0, -1000, 0 ),
                                    new Microsoft.Xna.Framework.Vector3( 100, 1000, 100 ) ) );

            List<Server.Wereld.ServerEntityHolder> ents = new List<Server.Wereld.ServerEntityHolder>();
            Server.Entities.Shuriken001 shur;




            TestServerClientMain.Start( "TestServerQuadtree",
                delegate
                {
                    shur = new Server.Entities.Shuriken001();
                    shur.Positie = new Vector3( 250, 100, 250 );
                    ents.Add( new Server.Wereld.ServerEntityHolder( shur ) );
                    tree.OrdenEntity( ents[ ents.Count - 1 ] );

                    shur = new Server.Entities.Shuriken001();
                    shur.Positie = new Vector3( 250, 100, 750 );
                    ents.Add( new Server.Wereld.ServerEntityHolder( shur ) );
                    tree.OrdenEntity( ents[ ents.Count - 1 ] );

                    shur = new Server.Entities.Shuriken001();
                    shur.Positie = new Vector3( 750, 100, 250 );
                    ents.Add( new Server.Wereld.ServerEntityHolder( shur ) );
                    tree.OrdenEntity( ents[ ents.Count - 1 ] );

                    shur = new Server.Entities.Shuriken001();
                    shur.Positie = new Vector3( 750, 100, 750 );
                    ents.Add( new Server.Wereld.ServerEntityHolder( shur ) );
                    tree.OrdenEntity( ents[ ents.Count - 1 ] );
                },
                delegate
                {

                }
            );
        }

        public static void TestServerEntities001()
        {

            Server.Wereld.ServerEntityHolder entH = null;


            TestServerClientMain.Start( "TestServerQuadtree",
                delegate
                {

                },
                delegate
                {
                    if ( ServerClientMainOud.instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.B ) )
                    {
                        ServerClientMainOud.instance.serverDebugRenderer.Enabled = !ServerClientMainOud.instance.serverDebugRenderer.Enabled;
                    }

                    if ( entH == null )
                    {

                        Entities.TestPlayer pl = new Entities.TestPlayer();

                        Wereld.ClientEntityHolder nClientEntH;
                        Server.Wereld.ServerEntityHolder nServerEntH;
                        Entities.TestPlayer.CreateTestPlayerEntity( pl, out nClientEntH, out nServerEntH );

                        ServerClientMainOud.instance.Wereld.AddEntity( nClientEntH );
                        ServerClientMainOud.instance.ServerMain.Wereld.AddEntity( nServerEntH );

                        entH = nServerEntH;

                    }

                    RenderServerNodeBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
                    RenderServerNodeEntityBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
                }
            );
        }

        public static void RenderServerNodeEntityBoundingBox( Server.Wereld.QuadTreeNode node )
        {
            /*if ( node == null ) return;


            Color[] levelColors;

            levelColors = new Color[ 5 ];
            levelColors[ 0 ] = Color.Red;
            levelColors[ 1 ] = Color.Orange;
            levelColors[ 2 ] = Color.Yellow;
            levelColors[ 3 ] = Color.Purple;
            levelColors[ 4 ] = Color.Green;


            RenderServerNodeEntityBoundingBox( node.UpperLeft );
            RenderServerNodeEntityBoundingBox( node.UpperRight );
            RenderServerNodeEntityBoundingBox( node.LowerLeft );
            RenderServerNodeEntityBoundingBox( node.LowerRight );

            if ( node.Entities.Count == 0 ) return;
            //FloorLowerLeft = min
            //TopUpperRight = max

            int level = node.CalculateLevel();
            Color col;
            if ( level < levelColors.Length )
                col = levelColors[ level ];
            else
                col = levelColors[ levelColors.Length - 1 ];

            Vector3 radius = node.EntityBoundingBox.Max - node.EntityBoundingBox.Min;
            Vector3 radX = new Vector3( radius.X, 0, 0 );
            Vector3 radY = new Vector3( 0, radius.Y, 0 );
            Vector3 radZ = new Vector3( 0, 0, radius.Z );
            Vector3 min = node.EntityBoundingBox.Min;


            Vector3 fll = min;
            Vector3 flr = min + radX;
            Vector3 ful = min + radZ;
            Vector3 fur = min + radX + radZ;
            Vector3 tll = min + radY;
            Vector3 tlr = min + radY + radX;
            Vector3 tul = min + radY + radZ;
            Vector3 tur = min + radY + radX + radZ; //= max



            //grondvlak
            ServerClientMain.instance.LineManager3D.AddLine( fll, flr, col );
            ServerClientMain.instance.LineManager3D.AddLine( flr, fur, col );
            ServerClientMain.instance.LineManager3D.AddLine( fur, ful, col );
            ServerClientMain.instance.LineManager3D.AddLine( ful, fll, col );

            //opstaande ribben
            ServerClientMain.instance.LineManager3D.AddLine( fll, tll, col );
            ServerClientMain.instance.LineManager3D.AddLine( flr, tlr, col );
            ServerClientMain.instance.LineManager3D.AddLine( fur, tur, col );
            ServerClientMain.instance.LineManager3D.AddLine( ful, tul, col );

            //bovenvlak
            ServerClientMain.instance.LineManager3D.AddLine( tll, tlr, col );
            ServerClientMain.instance.LineManager3D.AddLine( tlr, tur, col );
            ServerClientMain.instance.LineManager3D.AddLine( tur, tul, col );
            ServerClientMain.instance.LineManager3D.AddLine( tul, tll, col );*/

        }


        public static void RenderServerNodeBoundingBox( Server.Wereld.QuadTreeNode node )
        {
            if ( node == null ) return;


            Color[] levelColors;

            levelColors = new Color[ 8 ];
            levelColors[ 0 ] = Color.Red;
            levelColors[ 1 ] = Color.Orange;
            levelColors[ 2 ] = Color.Yellow;
            levelColors[ 3 ] = Color.Purple;
            levelColors[ 7 ] = Color.DarkGoldenrod;
            levelColors[ 6 ] = Color.Brown;
            levelColors[ 5 ] = Color.Green;
            levelColors[ 4 ] = Color.LightGreen;





            RenderServerNodeBoundingBox( node.UpperLeft );
            RenderServerNodeBoundingBox( node.UpperRight );
            RenderServerNodeBoundingBox( node.LowerLeft );
            RenderServerNodeBoundingBox( node.LowerRight );

            //if ( node.IsLeaf == false ) return;
            //FloorLowerLeft = min
            //TopUpperRight = max
            int level = node.CalculateLevel();
            Color col;
            if ( level < levelColors.Length )
                col = levelColors[ level ];
            else
                col = levelColors[ levelColors.Length - 1 ];

            Vector3 radius = node.BoundingBox.Max - node.BoundingBox.Min;
            Vector3 radX = new Vector3( radius.X, 0, 0 );
            Vector3 radY = new Vector3( 0, radius.Y, 0 );
            Vector3 radZ = new Vector3( 0, 0, radius.Z );
            Vector3 min = node.BoundingBox.Min;
            min.Y = -1 + level;


            Vector3 fll = min;
            Vector3 flr = min + radX;
            Vector3 ful = min + radZ;
            Vector3 fur = min + radX + radZ;



            //grondvlak
            ServerClientMainOud.instance.LineManager3D.AddLine( fll, flr, col );
            ServerClientMainOud.instance.LineManager3D.AddLine( flr, fur, col );
            ServerClientMainOud.instance.LineManager3D.AddLine( fur, ful, col );
            ServerClientMainOud.instance.LineManager3D.AddLine( ful, fll, col );


        }

        //public static void TestTerrainCollision001()
        //{

        //    TestServerClientMain main = null;


        //    XNAGeoMipMap.Terrain terr = null;
        //    Entities.TestPlayer pl = null;

        //    Game3DPlay.Core.Camera cam = null;


        //    TestServerClientMain.Start( "TestTerrainCollision001",
        //        delegate
        //        {
        //            main = TestServerClientMain.Instance;

        //            pl = new Entities.TestPlayer();

        //            Wereld.ClientEntityHolder nClientEntH;
        //            Server.Wereld.ServerEntityHolder nServerEntH;
        //            Entities.TestPlayer.CreateTestPlayerEntity( pl, out nClientEntH, out nServerEntH );

        //            ServerClientMain.instance.Wereld.AddEntity( nClientEntH );
        //            ServerClientMain.instance.ServerMain.Wereld.AddEntity( nServerEntH );



        //            terr = new XNAGeoMipMap.Terrain( TestServerClientMain.instance,
        //                TestServerClientMain.instance.XNAGame._content.RootDirectory
        //                + @"\Content\Terrain\Data.txt" );

        //            terr.HeightMap = new Common.GeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


        //            terr.Enabled = true;
        //            terr.Visible = true;
        //            TestServerClientMain.instance.XNAGame.Components.Add( terr );

        //            terr.LoadFromDisk();


        //            pl.activeTerrain = terr;

        //            cam = new Game3DPlay.Core.Camera( ServerClientMain.instance );


        //        },
        //        delegate
        //        {
        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.B ) )
        //            {
        //                ServerClientMain.instance.serverDebugRenderer.Enabled = !ServerClientMain.instance.serverDebugRenderer.Enabled;
        //            }

        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.C ) )
        //            {

        //                ServerClientMain.instance.spec.Enabled = false;
        //                cam.Enabled = true;
        //                ServerClientMain.instance.SetCamera( cam );

        //                cam.CameraInfo.ProjectionMatrix = ServerClientMain.instance.spec.CameraInfo.ProjectionMatrix;
        //                cam.CameraPosition = pl.Body.Positie - cam.CameraDirection * 30 + new Vector3( 0, 10, 0 );
        //                cam.CameraDirection = Vector3.Transform( Vector3.Backward, pl.Body.Rotatie );
        //                cam.CameraUp = Vector3.Up;
        //                cam.UpdateCameraInfo();

        //            }
        //            else
        //            {
        //                ServerClientMain.instance.spec.Enabled = true;
        //                cam.Enabled = false;
        //                ServerClientMain.instance.SetCamera( ServerClientMain.instance.spec );
        //            }

        //            //pl.Body.Positie = new Vector3( 0, 250, 0 );

        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.A ) )
        //            {
        //                terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //                terr.CameraPostion = main.ActiveCamera.CameraPosition;
        //            }

        //            else
        //            {
        //                terr.Frustum = new BoundingFrustum(
        //                    Matrix.CreateLookAt( pl.Body.Positie, Vector3.Transform( Vector3.Backward, pl.Body.Rotatie ) + pl.Body.Positie,
        //                        Vector3.Up )
        //                        * ServerClientMain.instance.ActiveCamera.CameraInfo.ProjectionMatrix );

        //                terr.CameraPostion = pl.Body.Positie;

        //            }




        //            RenderServerNodeBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
        //            RenderServerNodeEntityBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );

        //        }
        //    );
        //}


        //public static void TestOcean001()
        //{

        //    XNAGeoMipMap.Water001 water = null;
        //    TestServerClientMain main = null;


        //    XNAGeoMipMap.Terrain terr = null;
        //    Entities.TestPlayer pl = null;

        //    Game3DPlay.Core.Camera cam = null;


        //    TestServerClientMain.Start( "TestTerrainCollision001",
        //        delegate
        //        {
        //            main = TestServerClientMain.Instance;

        //            pl = new Entities.TestPlayer();

        //            Wereld.ClientEntityHolder nClientEntH;
        //            Server.Wereld.ServerEntityHolder nServerEntH;
        //            Entities.TestPlayer.CreateTestPlayerEntity( pl, out nClientEntH, out nServerEntH );

        //            ServerClientMain.instance.Wereld.AddEntity( nClientEntH );
        //            ServerClientMain.instance.ServerMain.Wereld.AddEntity( nServerEntH );



        //            terr = new XNAGeoMipMap.Terrain( TestServerClientMain.instance,
        //                TestServerClientMain.instance.XNAGame._content.RootDirectory
        //                + @"\Content\Terrain\Data.txt" );

        //            terr.HeightMap = new Common.GeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


        //            terr.Enabled = true;
        //            terr.Visible = true;
        //            TestServerClientMain.instance.XNAGame.Components.Add( terr );

        //            terr.LoadFromDisk();


        //            pl.activeTerrain = terr;

        //            cam = new Game3DPlay.Core.Camera( ServerClientMain.instance );

        //            water = new MHGameWork.TheWizards.ServerClient.XNAGeoMipMap.Water001();
        //            water.Load( main.XNAGame.Graphics.GraphicsDevice );

        //            water.World = Matrix.CreateTranslation( -100, 0, -100 ) * Matrix.CreateScale( 20 ) * Matrix.CreateTranslation( 0, 50, 0 );

        //        },
        //        delegate
        //        {
        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.B ) )
        //            {
        //                ServerClientMain.instance.serverDebugRenderer.Enabled = !ServerClientMain.instance.serverDebugRenderer.Enabled;
        //            }

        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.C ) )
        //            {

        //                ServerClientMain.instance.spec.Enabled = false;
        //                cam.Enabled = true;
        //                ServerClientMain.instance.SetCamera( cam );

        //                cam.CameraInfo.ProjectionMatrix = ServerClientMain.instance.spec.CameraInfo.ProjectionMatrix;
        //                cam.CameraPosition = pl.Body.Positie - cam.CameraDirection * 30 + new Vector3( 0, 10, 0 );
        //                cam.CameraDirection = Vector3.Transform( Vector3.Backward, pl.Body.Rotatie );
        //                cam.CameraUp = Vector3.Up;
        //                cam.UpdateCameraInfo();

        //            }
        //            else
        //            {
        //                ServerClientMain.instance.spec.Enabled = true;
        //                cam.Enabled = false;
        //                ServerClientMain.instance.SetCamera( ServerClientMain.instance.spec );
        //            }

        //            if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
        //            { water.effect.Load( main.XNAGame._content ); }

        //            //pl.Body.Positie = new Vector3( 0, 250, 0 );

        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.A ) )
        //            {
        //                terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //                terr.CameraPostion = main.ActiveCamera.CameraPosition;
        //            }

        //            else
        //            {
        //                terr.Frustum = new BoundingFrustum(
        //                    Matrix.CreateLookAt( pl.Body.Positie, Vector3.Transform( Vector3.Backward, pl.Body.Rotatie ) + pl.Body.Positie,
        //                        Vector3.Up )
        //                        * ServerClientMain.instance.ActiveCamera.CameraInfo.ProjectionMatrix );

        //                terr.CameraPostion = pl.Body.Positie;

        //            }




        //            RenderServerNodeBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
        //            RenderServerNodeEntityBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );

        //            water.Time += main.ProcessEventArgs.Elapsed;

        //            water.Render();
        //        }
        //    );
        //}

        //public static void TestWaterSkyCollision001()
        //{

        //    XNAGeoMipMap.Skydome skydome = null;
        //    XNAGeoMipMap.Water002 water = null;
        //    TestServerClientMain main = null;


        //    XNAGeoMipMap.Terrain terr = null;
        //    Entities.TestPlayer pl = null;

        //    Entities.PrimitiveHouse001 house = null;

        //    Game3DPlay.Core.Camera cam = null;
        //    GraphicsDevice device = null;
        //    //truukje
        //    float backwidth = 0;

        //    TestServerClientMain.Start( "TestWaterSkyCollision001",
        //        delegate
        //        {
        //            main = TestServerClientMain.Instance;
        //            device = main.XNAGame.Graphics.GraphicsDevice;

        //            pl = new Entities.TestPlayer();

        //            Wereld.ClientEntityHolder nClientEntH;
        //            Server.Wereld.ServerEntityHolder nServerEntH;
        //            Entities.TestPlayer.CreateTestPlayerEntity( pl, out nClientEntH, out nServerEntH );

        //            ServerClientMain.instance.Wereld.AddEntity( nClientEntH );
        //            ServerClientMain.instance.ServerMain.Wereld.AddEntity( nServerEntH );


        //            house = new Entities.PrimitiveHouse001();

        //            Entities.PrimitiveHouse001.CreatePrimitiveHouse001Entity( house, out nClientEntH, out nServerEntH );

        //            ServerClientMain.instance.Wereld.AddEntity( nClientEntH );
        //            ServerClientMain.instance.ServerMain.Wereld.AddEntity( nServerEntH );

        //            house.Body.Rotatie = Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, MathHelper.PiOver2, 0 );
        //            house.Body.Scale = new Vector3( 200 );
        //            house.Body.Positie = new Vector3( -300, 60, 600 );



        //            terr = new XNAGeoMipMap.Terrain( TestServerClientMain.instance,
        //                TestServerClientMain.instance.XNAGame._content.RootDirectory
        //                + @"\Content\Terrain\Data.txt" );

        //            terr.HeightMap = new Common.GeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


        //            terr.Enabled = true;
        //            terr.Visible = true;
        //            TestServerClientMain.instance.XNAGame.Components.Add( terr );

        //            terr.LoadFromDisk();


        //            pl.activeTerrain = terr;

        //            cam = new Game3DPlay.Core.Camera( ServerClientMain.instance );

        //            water = new MHGameWork.TheWizards.ServerClient.XNAGeoMipMap.Water002( main );


        //            skydome = new XNAGeoMipMap.Skydome( main );

        //        },
        //        delegate
        //        {


        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.B ) )
        //            {
        //                ServerClientMain.instance.serverDebugRenderer.Enabled = !ServerClientMain.instance.serverDebugRenderer.Enabled;
        //            }

        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.C ) )
        //            {

        //                ServerClientMain.instance.spec.Enabled = false;
        //                cam.Enabled = true;
        //                ServerClientMain.instance.SetCamera( cam );

        //                cam.CameraInfo.ProjectionMatrix = ServerClientMain.instance.spec.CameraInfo.ProjectionMatrix;
        //                cam.CameraPosition = pl.Body.Positie - cam.CameraDirection * 60 + new Vector3( 0, 10, 0 );
        //                cam.CameraDirection = Vector3.Transform( Vector3.Backward, Matrix.CreateFromYawPitchRoll( pl.cameraAngles.Y, -pl.cameraAngles.X, pl.cameraAngles.Z ) );
        //                cam.CameraUp = Vector3.Up;
        //                cam.UpdateCameraInfo();

        //            }
        //            else
        //            {
        //                ServerClientMain.instance.spec.Enabled = true;
        //                cam.Enabled = false;
        //                ServerClientMain.instance.SetCamera( ServerClientMain.instance.spec );
        //            }

        //            if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
        //            { water.effect.Load( main.XNAGame._content ); }




        //            if ( skydome.device == null ) skydome.Load( main.XNAGame.Graphics.GraphicsDevice );
        //            if ( device.ScissorRectangle.Width != backwidth )
        //            {
        //                water.Load( device );

        //                backwidth = device.ScissorRectangle.Width;
        //            }
        //            water.elapsedTime += main.ProcessEventArgs.Elapsed / 100.0f;
        //            //skydome.elapsedTime = water.elapsedTime;

        //            device.RenderState.CullMode = CullMode.None;


        //            water.DrawReflectionMap( delegate
        //            {
        //                skydome.RenderSkyDome();
        //                if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.A ) )
        //                {
        //                    terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //                    terr.CameraPostion = main.ActiveCamera.CameraPosition;
        //                }

        //                else
        //                {
        //                    terr.Frustum = new BoundingFrustum(
        //                        Matrix.CreateLookAt( pl.Body.Positie, Vector3.Transform( Vector3.Backward, pl.Body.Rotatie ) + pl.Body.Positie,
        //                            Vector3.Up )
        //                            * ServerClientMain.instance.ActiveCamera.CameraInfo.ProjectionMatrix );

        //                    terr.CameraPostion = pl.Body.Positie;

        //                }

        //                terr.Update();

        //                terr.Draw();
        //                main.Wereld.Render();

        //            } );


        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.A )
        //                || ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.C ) )
        //            {
        //                terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //                terr.CameraPostion = main.ActiveCamera.CameraPosition;
        //            }

        //            else
        //            {
        //                terr.Frustum = new BoundingFrustum(
        //                    Matrix.CreateLookAt( pl.Body.Positie, Vector3.Transform( Vector3.Backward, pl.Body.Rotatie ) + pl.Body.Positie,
        //                        Vector3.Up )
        //                        * ServerClientMain.instance.ActiveCamera.CameraInfo.ProjectionMatrix );

        //                terr.CameraPostion = pl.Body.Positie;

        //            }


        //            terr.Update();

        //            water.DrawRefractionMap( delegate
        //            {
        //                terr.Draw();
        //                main.Wereld.Render();

        //            } );


        //            main.XNAGame.Graphics.GraphicsDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.SkyBlue, 1.0f, 0 );

        //            if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
        //            { water.effect.Load( main.XNAGame._content ); skydome.effect.Load( main.XNAGame._content ); }

        //            skydome.RenderSkyDome();
        //            terr.Draw();
        //            water.RenderWater();
        //            main.Wereld.Render();
        //            if ( main.serverDebugRenderer.Enabled ) main.serverDebugRenderer.OnRender( null, main._renderEventArgs );

        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.F4 ) )
        //            {
        //                main.XNAGame.Graphics.ToggleFullScreen();
        //                main.ProcessEventArgs.Keyboard.UpdateKeyboardState( Microsoft.Xna.Framework.Input.Keyboard.GetState() );
        //                backwidth = 0;

        //            }

        //        }
        //    );
        //}

        //public static void TestTerrainCollision002()
        //{

        //    TestServerClientMain main = null;


        //    XNAGeoMipMap.Terrain terr = null;
        //    Entities.TestPlayer pl = null;

        //    Game3DPlay.Core.Camera cam = null;


        //    TestServerClientMain.Start( "TestTerrainCollision001",
        //        delegate
        //        {
        //            main = TestServerClientMain.Instance;

        //            pl = new Entities.TestPlayer();

        //            Wereld.ClientEntityHolder nClientEntH;
        //            Server.Wereld.ServerEntityHolder nServerEntH;
        //            Entities.TestPlayer.CreateTestPlayerEntity( pl, out nClientEntH, out nServerEntH );

        //            ServerClientMain.instance.Wereld.AddEntity( nClientEntH );
        //            ServerClientMain.instance.ServerMain.Wereld.AddEntity( nServerEntH );



        //            terr = new XNAGeoMipMap.Terrain( TestServerClientMain.instance,
        //                TestServerClientMain.instance.XNAGame._content.RootDirectory
        //                + @"\Content\Terrain\Data.txt" );

        //            terr.HeightMap = new Common.GeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


        //            terr.Enabled = true;
        //            terr.Visible = true;
        //            TestServerClientMain.instance.XNAGame.Components.Add( terr );

        //            terr.LoadFromDisk();


        //            pl.activeTerrain = terr;

        //            cam = new Game3DPlay.Core.Camera( ServerClientMain.instance );


        //        },
        //        delegate
        //        {
        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.B ) )
        //            {
        //                ServerClientMain.instance.serverDebugRenderer.Enabled = !ServerClientMain.instance.serverDebugRenderer.Enabled;
        //            }

        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.C ) )
        //            {

        //                ServerClientMain.instance.spec.Enabled = false;
        //                cam.Enabled = true;
        //                ServerClientMain.instance.SetCamera( cam );

        //                cam.CameraInfo.ProjectionMatrix = ServerClientMain.instance.spec.CameraInfo.ProjectionMatrix;
        //                cam.CameraPosition = pl.Body.Positie - cam.CameraDirection * 30 + new Vector3( 0, 10, 0 );
        //                cam.CameraDirection = Vector3.Transform( Vector3.Backward, pl.Body.Rotatie );
        //                cam.CameraUp = Vector3.Up;
        //                cam.UpdateCameraInfo();

        //            }
        //            else
        //            {
        //                ServerClientMain.instance.spec.Enabled = true;
        //                cam.Enabled = false;
        //                ServerClientMain.instance.SetCamera( ServerClientMain.instance.spec );
        //            }

        //            //pl.Body.Positie = new Vector3( 0, 250, 0 );

        //            if ( ServerClientMain.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.A ) )
        //            {
        //                terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //                terr.CameraPostion = main.ActiveCamera.CameraPosition;
        //            }

        //            else
        //            {
        //                terr.Frustum = new BoundingFrustum(
        //                    Matrix.CreateLookAt( pl.Body.Positie, Vector3.Transform( Vector3.Backward, pl.Body.Rotatie ) + pl.Body.Positie,
        //                        Vector3.Up )
        //                        * ServerClientMain.instance.ActiveCamera.CameraInfo.ProjectionMatrix );

        //                terr.CameraPostion = pl.Body.Positie;

        //            }




        //            RenderServerNodeBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
        //            RenderServerNodeEntityBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );

        //        }
        //    );
        //}

        public static void TestServerClientNetwork001()
        {
            ServerClientMainOud main = null;

            bool loggingIn = false;

            Server.Entities.Shuriken003 shurServer = null;
            Entities.Shuriken003 clientEnt;
            Wereld.ClientEntityHolder clientEntH;
            Server.Entities.Shuriken003 serverEnt;
            Server.Wereld.ServerEntityHolder serverEntH;
            //int nextUpdate = 0;


            TestServerClientMain.Start( "TestTerrainCollision001",
                delegate
                {
                    main = TestServerClientMain.instance;

                    main.ServerMain.Communication.StartListening();
                    main.Server.ConnectAsync();






                    shurServer = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );

                    main.ServerMain.Wereld.AddEntity( Server.Entities.Shuriken003.CreateShuriken003Entity( shurServer ) );

                    shurServer.FlyUp();

                    /*Entities.Shuriken003 shurClient = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                    clientEntH = Entities.Shuriken003.CreateShuriken003Entity( shurClient );
                    main.Wereld.AddEntity( clientEntH );*/


                    for ( int i = 200; i < 210; i++ )
                    {
                        serverEnt = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );
                        serverEntH = Server.Entities.Shuriken003.CreateShuriken003Entity( serverEnt );

                        main.ServerMain.Wereld.AddEntity( serverEntH );
                        serverEntH.SetID( i );
                        serverEnt.Body.Positie = new Vector3( 0, 100 * ( i - 199 ), 0 );
                        serverEnt.FlyUp();

                        clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                        clientEntH = Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                        main.Wereld.AddEntity( clientEntH );
                        clientEntH.SetID( i );
                    }





                },
                delegate
                {
                    if ( main.Server.ConnectedToServer && !loggingIn )
                    {
                        loggingIn = true;
                        main.Server.Wereld.LoginAsync( "MHGW", "pass" );
                    }

                    if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.P ) )
                    {
                        main.Server.Wereld.Ping();
                    }




                    if ( main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.B ) )
                    {
                        main.serverDebugRenderer.Enabled = true;
                    }
                    else
                    {
                        main.serverDebugRenderer.Enabled = false;
                    }
                    if ( main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.F ) )
                    {
                        main.Server.Communication.SendPacketUDP( MHGameWork.TheWizards.Common.Communication.ServerCommands.TempShootShuriken );
                        //System.Windows.Forms.MessageBox.Show("beep");
                    }
                    if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.G ) )
                    {
                        serverEnt = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );
                        serverEntH = Server.Entities.Shuriken003.CreateShuriken003Entity( serverEnt );

                        main.ServerMain.Wereld.AddEntity( serverEntH );
                        //serverEntH.SetID( i );
                        serverEnt.Body.Positie = new Vector3( 0, 100, 0 );
                        serverEnt.FlyUp();

                        /*clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                        clientEntH = Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                        main.Wereld.AddEntity( clientEntH );
                        clientEntH.SetID( serverEntH.ID );*/
                        // shurServer.Body.serverBody.Rotatie = Matrix.CreateRotationX(MathHelper.PiOver2);
                    }


                } );



        }

        public static void TestWorld001()
        {

            TestServerClientMain main = null;

            XNAGeoMipMap.Terrain terr = null;
            bool started = false;


            bool loggingIn = false;

            Server.Entities.Shuriken003 shurServer = null;
            Entities.Shuriken003 clientEnt;
            Wereld.ClientEntityHolder clientEntH;
            Server.Entities.Shuriken003 serverEnt;
            Server.Wereld.ServerEntityHolder serverEntH;
            //int nextUpdate = 0;


            TestServerClientMain.Start( "TestWorld001",
                delegate
                {
                    main = TestServerClientMain.Instance;

                    main.ServerMain.Communication.StartListening();
                    main.Server.ConnectAsync();







                    terr = new XNAGeoMipMap.Terrain( TestServerClientMain.instance );

                    //terr.HeightMap = new XNAGeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


                    /*terr.Enabled = false;
                    terr.Visible = false;
                    TestServerClientMain.instance.XNAGame.Components.Add( terr );*/

                    terr.Initialize();

                    terr.SetFilename( main.XNAGame._content.RootDirectory + "\\Content\\TerrainTest\\TerrainWorld001" );


                    main.Wereld.Tree.Split(); //1024
                    main.Wereld.Tree.UpperLeft.Split(); // 512 OK!
                    main.Wereld.Tree.UpperLeft.LowerRight.SetStatic( true );


                    terr.SetQuadTreeNode( main.Wereld.Tree.UpperLeft.LowerRight );




















                    shurServer = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );

                    main.ServerMain.Wereld.AddEntity( Server.Entities.Shuriken003.CreateShuriken003Entity( shurServer ) );

                    shurServer.FlyUp();

                    /*Entities.Shuriken003 shurClient = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                    clientEntH = Entities.Shuriken003.CreateShuriken003Entity( shurClient );
                    main.Wereld.AddEntity( clientEntH );*/


                    for ( int i = 200; i < 210; i++ )
                    {
                        serverEnt = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );
                        serverEntH = Server.Entities.Shuriken003.CreateShuriken003Entity( serverEnt );

                        main.ServerMain.Wereld.AddEntity( serverEntH );
                        serverEntH.SetID( i );
                        serverEnt.Body.Positie = new Vector3( 0, 100 * ( i - 199 ), 0 );
                        serverEnt.FlyUp();

                        clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                        clientEntH = Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                        main.Wereld.AddEntity( clientEntH );
                        clientEntH.SetID( i );
                    }













                },
                delegate
                {
                    if ( main.Server.ConnectedToServer && !loggingIn )
                    {
                        loggingIn = true;
                        main.Server.Wereld.LoginAsync( "MHGW", "pass" );
                    }

                    if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.P ) )
                    {
                        main.Server.Wereld.Ping();
                    }




                    if ( main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.B ) )
                    {
                        main.serverDebugRenderer.Enabled = true;
                    }
                    else
                    {
                        main.serverDebugRenderer.Enabled = false;
                    }
                    if ( main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.F ) )
                    {
                        main.Server.Communication.SendPacketUDP( MHGameWork.TheWizards.Common.Communication.ServerCommands.TempShootShuriken );
                        //System.Windows.Forms.MessageBox.Show("beep");
                    }
                    if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.G ) )
                    {
                        serverEnt = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );
                        serverEntH = Server.Entities.Shuriken003.CreateShuriken003Entity( serverEnt );

                        main.ServerMain.Wereld.AddEntity( serverEntH );
                        //serverEntH.SetID( i );
                        serverEnt.Body.Positie = main.ActiveCamera.CameraPosition + new Vector3( 0, 100, 0 );
                        serverEnt.FlyUp();

                        /*clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                        clientEntH = Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                        main.Wereld.AddEntity( clientEntH );
                        clientEntH.SetID( serverEntH.ID );*/
                        // shurServer.Body.serverBody.Rotatie = Matrix.CreateRotationX(MathHelper.PiOver2);
                    }











                    if ( !started || main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
                    {
                        terr.CreateTerrain( 32, 16, 16 );


                        //terr.BuildVerticesFromHeightmap();

                        terr.effect.Load( terr.Content );

                        terr.LoadGraphicsContent();
                        started = true;
                    }

                    if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.R ) )
                    {
                        terr.RaiseTerrain( new Vector2( main.ActiveCamera.CameraPosition.X, main.ActiveCamera.CameraPosition.Z )
                             , 20, 5 );
                        terr.UpdateDirtyVertexbuffers();
                        //terr.RebuildBounding();
                    }


                    terr.Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
                    terr.CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

                    //if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.E ) ) 
                    terr.Update();
                    terr.Draw();



                    main.XNAGame.GraphicsDevice.RenderState.DepthBufferEnable = true;





                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 2000, 0, 0 ), Color.Red );
                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 2000 ), Color.Blue );

                    main.Wereld.Tree.RenderNodeBoundingBox();
                    main.Wereld.Tree.RenderNodeEntityBoundingBox();

                    main.XNAGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;

                    //RenderServerNodeBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
                    //RenderServerNodeEntityBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
















                } );



        }

        public static void TestServerQuadtreeCollision()
        {

            TestServerClientMain main = null;

            Server.GeoMipMap.Terrain terr = null;
            bool started = false;


            bool loggingIn = false;

            Server.Entities.Shuriken003 shurServer = null;
            //Entities.Shuriken003 clientEnt;
            //Wereld.ClientEntityHolder clientEntH;
            Server.Entities.Shuriken003 serverEnt;
            Server.Wereld.ServerEntityHolder serverEntH;
            //int nextUpdate = 0;


            TestServerClientMain.Start( "TestWorld001",
                delegate
                {
                    main = TestServerClientMain.Instance;

                    //main.ServerMain.Communication.StartListening();
                    //main.Server.ConnectAsync();







                    terr = new Server.GeoMipMap.Terrain( main.ServerMain );

                    //terr.HeightMap = new XNAGeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


                    /*terr.Enabled = false;
                    terr.Visible = false;
                    TestServerClientMain.instance.XNAGame.Components.Add( terr );*/

                    //terr.Initialize();

                    terr.SetFilename( main.XNAGame._content.RootDirectory + "\\Content\\TerrainTest\\TestServerQuadtreeCollision" );


                    main.ServerMain.Wereld.Tree.Split(); //1024
                    main.ServerMain.Wereld.Tree.UpperLeft.Split(); // 512 OK!
                    main.ServerMain.Wereld.Tree.UpperLeft.LowerRight.SetStatic( true );


                    terr.SetQuadTreeNode( main.ServerMain.Wereld.Tree.UpperLeft.LowerRight );




















                    shurServer = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );

                    main.ServerMain.Wereld.AddEntity( Server.Entities.Shuriken003.CreateShuriken003Entity( shurServer ) );

                    shurServer.FlyUp();

                    /*Entities.Shuriken003 shurClient = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                    clientEntH = Entities.Shuriken003.CreateShuriken003Entity( shurClient );
                    main.Wereld.AddEntity( clientEntH );*/


                    for ( int i = 200; i < 210; i++ )
                    {
                        serverEnt = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );
                        serverEntH = Server.Entities.Shuriken003.CreateShuriken003Entity( serverEnt );

                        main.ServerMain.Wereld.AddEntity( serverEntH );
                        serverEntH.SetID( i );
                        serverEnt.Body.Positie = new Vector3( 0, 100 * ( i - 199 ), 0 );
                        serverEnt.FlyUp();

                        //clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                        //clientEntH = Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                        //main.Wereld.AddEntity( clientEntH );
                        //clientEntH.SetID( i );
                    }













                },
                delegate
                {
                    if ( main.Server.ConnectedToServer && !loggingIn )
                    {
                        loggingIn = true;
                        main.Server.Wereld.LoginAsync( "MHGW", "pass" );
                    }

                    if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.P ) )
                    {
                        main.Server.Wereld.Ping();
                    }




                    if ( !main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.B ) )
                    {
                        main.serverDebugRenderer.Enabled = true;
                    }
                    else
                    {
                        main.serverDebugRenderer.Enabled = false;
                    }
                    if ( main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.F ) )
                    {
                        main.Server.Communication.SendPacketUDP( MHGameWork.TheWizards.Common.Communication.ServerCommands.TempShootShuriken );
                        //System.Windows.Forms.MessageBox.Show("beep");
                    }
                    if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.G ) )
                    {
                        serverEnt = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );
                        serverEntH = Server.Entities.Shuriken003.CreateShuriken003Entity( serverEnt );

                        main.ServerMain.Wereld.AddEntity( serverEntH );
                        //serverEntH.SetID( i );
                        serverEnt.Body.Positie = main.ActiveCamera.CameraPosition + new Vector3( 0, 100, 0 );
                        serverEnt.FlyUp();

                        /*clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                        clientEntH = Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                        main.Wereld.AddEntity( clientEntH );
                        clientEntH.SetID( serverEntH.ID );*/
                        // shurServer.Body.serverBody.Rotatie = Matrix.CreateRotationX(MathHelper.PiOver2);
                    }











                    if ( !started || main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
                    {
                        terr.CreateTerrain( 32, 16, 16 );


                        //terr.BuildVerticesFromHeightmap();

                        //terr.effect.Load( terr.Content );

                        //terr.LoadGraphicsContent();
                        started = true;
                    }

                    //if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.R ) )
                    //{
                    //    terr.RaiseTerrain( new Vector2( main.ActiveCamera.CameraPosition.X, main.ActiveCamera.CameraPosition.Z )
                    //         , 20, 5 );
                    //    terr.UpdateDirtyVertexbuffers();
                    //    //terr.RebuildBounding();
                    //}


                    //terr.Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
                    //terr.CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

                    ////if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.E ) ) 
                    //terr.Update();
                    //terr.Draw();



                    main.XNAGame.GraphicsDevice.RenderState.DepthBufferEnable = true;





                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 2000, 0, 0 ), Color.Red );
                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 2000 ), Color.Blue );

                    //main.Wereld.Tree.RenderNodeBoundingBox();
                    //main.Wereld.Tree.RenderNodeEntityBoundingBox();
                    RenderServerNodeBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );

                    main.XNAGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;

                    //RenderServerNodeBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
                    //RenderServerNodeEntityBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
















                } );



        }


        public static void TestPlayerInput001()
        {

            TestServerClientMain main = null;



            //XNAGeoMipMap.Terrain terr = null;
            bool started = false;


            bool loggingIn = false;

            //Server.Entities.Shuriken003 shurServer = null;
            //Entities.Shuriken003 clientEnt;
            //Wereld.ClientEntityHolder clientEntH;
            //Server.Entities.Shuriken003 serverEnt;
            //Server.Wereld.ServerEntityHolder serverEntH;
            //int nextUpdate = 0;



            TestServerClientMain.Start( "TestPlayerInput001",
                delegate
                {
                    main = TestServerClientMain.Instance;

                    main.ServerMain.Communication.StartListening();
                    main.Server.ConnectAsync();





                    main.SetCamera( main.GameClient.PlayerCamera );


                    //terr = new XNAGeoMipMap.Terrain( TestServerClientMain.instance );

                    ////terr.HeightMap = new XNAGeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


                    ///*terr.Enabled = false;
                    //terr.Visible = false;
                    //TestServerClientMain.instance.XNAGame.Components.Add( terr );*/

                    //terr.Initialize();

                    //terr.SetFilename( main.XNAGame._content.RootDirectory + "\\Content\\TerrainTest\\TerrainWorld001" );


                    //main.Wereld.Tree.Split(); //1024
                    //main.Wereld.Tree.UpperLeft.Split(); // 512 OK!
                    //main.Wereld.Tree.UpperLeft.LowerRight.SetStatic( true );


                    //terr.SetQuadTreeNode( main.Wereld.Tree.UpperLeft.LowerRight );


                    //shurServer = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );

                    //main.ServerMain.Wereld.AddEntity( Server.Entities.Shuriken003.CreateShuriken003Entity( shurServer ) );

                    //shurServer.FlyUp();

                    ///*Entities.Shuriken003 shurClient = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                    //clientEntH = Entities.Shuriken003.CreateShuriken003Entity( shurClient );
                    //main.Wereld.AddEntity( clientEntH );*/


                    //for ( int i = 200; i < 210; i++ )
                    //{
                    //    serverEnt = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );
                    //    serverEntH = Server.Entities.Shuriken003.CreateShuriken003Entity( serverEnt );

                    //    main.ServerMain.Wereld.AddEntity( serverEntH );
                    //    serverEntH.SetID( i );
                    //    serverEnt.Body.Positie = new Vector3( 0, 100 * ( i - 199 ), 0 );
                    //    serverEnt.FlyUp();

                    //    clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                    //    clientEntH = Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                    //    main.Wereld.AddEntity( clientEntH );
                    //    clientEntH.SetID( i );
                    //}













                },
                delegate
                {
                    if ( main.Server.ConnectedToServer && !loggingIn )
                    {
                        loggingIn = true;
                        main.Server.Wereld.LoginAsync( "MHGW", "pass" );
                    }

                    if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.P ) )
                    {
                        main.Server.Wereld.Ping();
                    }





                    main.GameClient.PlayerCamera.Process( main.ProcessEventArgs );


                    if ( main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.B ) )
                    {
                        main.serverDebugRenderer.Enabled = true;
                    }
                    else
                    {
                        main.serverDebugRenderer.Enabled = false;
                    }
                    if ( main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.F ) )
                    {
                        main.Server.Communication.SendPacketUDP( MHGameWork.TheWizards.Common.Communication.ServerCommands.TempShootShuriken );
                        //System.Windows.Forms.MessageBox.Show("beep");
                    }                    //if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.G ) )
                    //{
                    //    serverEnt = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main.ServerMain );
                    //    serverEntH = Server.Entities.Shuriken003.CreateShuriken003Entity( serverEnt );

                    //    main.ServerMain.Wereld.AddEntity( serverEntH );
                    //    //serverEntH.SetID( i );
                    //    serverEnt.Body.Positie = main.ActiveCamera.CameraPosition + new Vector3( 0, 100, 0 );
                    //    serverEnt.FlyUp();

                    //    /*clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                    //    clientEntH = Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                    //    main.Wereld.AddEntity( clientEntH );
                    //    clientEntH.SetID( serverEntH.ID );*/
                    //    // shurServer.Body.serverBody.Rotatie = Matrix.CreateRotationX(MathHelper.PiOver2);
                    //}











                    if ( !started || main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
                    {
                        //terr.CreateTerrain( 32, 16, 16 );


                        //terr.BuildVerticesFromHeightmap();

                        //terr.effect.Load( terr.Content );

                        //terr.LoadGraphicsContent();
                        started = true;
                    }

                    //if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.R ) )
                    //{
                    //    terr.RaiseTerrain( new Vector2( main.ActiveCamera.CameraPosition.X, main.ActiveCamera.CameraPosition.Z )
                    //         , 20, 5 );
                    //    terr.UpdateDirtyVertexbuffers();
                    //    //terr.RebuildBounding();
                    //}


                    //terr.Frustum = TestServerClientMain.instance.ActiveCamera.CameraInfo.Frustum;
                    //terr.CameraPostion = TestServerClientMain.instance.ActiveCamera.CameraPosition;

                    ////if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.E ) ) 
                    //terr.Update();
                    //terr.Draw();



                    main.XNAGame.GraphicsDevice.RenderState.DepthBufferEnable = true;





                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 2000, 0, 0 ), Color.Red );
                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 2000 ), Color.Blue );

                    main.Wereld.Tree.RenderNodeBoundingBox();
                    main.Wereld.Tree.RenderNodeEntityBoundingBox();

                    main.XNAGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;

                    //RenderServerNodeBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
                    //RenderServerNodeEntityBoundingBox( TestServerClientMain.Instance.ServerMain.Wereld.Tree );
















                } );



        }



#endif
        public static ServerClientMainOud SvClMain { get { return ServerClientMainOud.instance; } }

    }
}