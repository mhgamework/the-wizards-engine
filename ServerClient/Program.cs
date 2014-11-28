using System;
using System.IO;
using System.Reflection;
using System.Threading;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.ServerClientMain;
using MHGameWork.TheWizards.TestRunner;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.ServerClient
{
    static class Program
    {


        /// <summary>
        /// The main entry point for the application 
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            (new TWBootstrapper()).Run();
            //runOldTestRunner(args);
        }

        private static void runOldTestRunner(string[] args)
        {
            var fi = new FileInfo("Console.log");
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            OudMain();

            var oldstrm = Console.Out;



            using (var redir = new ConsoleRedir(oldstrm, fi.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
            {
                redir.WriteLine("--------------------Start Logging!");
                Console.SetOut(redir);

                TestRunnerGUI runnerGui = new TestRunnerGUI(Assembly.LoadFrom("Unit Tests.dll"));
                //runner.RunTestNewProcessPath = "\"" + Assembly.GetExecutingAssembly().Location + "\"" + " -test {0}";


                if (args.Length == 2 && args[0] == "-test")
                {
                    runnerGui.RunTestByName(args[1]);
                }
                else
                {
                    runnerGui.Run();

                }





                redir.WriteLine("--------------------End Logging!");
            }
            Console.SetOut(oldstrm);
        }


        private static void runNormal(string[] args)
        {
            //args = new string[] {"-editor"};
            //args = new string[] { "-server" };
            args = new string[] { "-serverclient" };

            if (args.Length > 0 && args[0].Equals("-server"))
            {
                //TheWizardsServer server = new TheWizardsServer();
                //server.Start();

            }
            else if (args.Length > 0 && args[0].Equals("-serverclient"))
            {
                TheWizardsServerClient serverClient = new TheWizardsServerClient();
                serverClient.Run();


            }
            else if (args.Length > 0 && args[0].Equals("-editor"))
            {
                //WizardsEditorFormDevcomponents.RunWorldEditor();
                //WizardsEditor.TestRunEditorDevcomponents();
                //WizardsEditor.TestRunEditor();
                //EditorMain.TheWizardsEditor editor = new MHGameWork.TheWizards.EditorMain.TheWizardsEditor();
                //editor.Run();

            }
            else
            {
                //TheWizardsClient client = new TheWizardsClient();
                //client.Run();
                //RunGame();
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Console.WriteLine(e.ExceptionObject);
        }

        private class ConsoleRedir : StreamWriter
        {
            private readonly TextWriter writer;

            public ConsoleRedir(TextWriter writer, Stream strm)
                : base(strm)
            {
                this.writer = writer;
            }

            public override void WriteLine(string value)
            {
                base.WriteLine(String.Format("{0:d/M/yyyy HH:mm:ss} - {1}", DateTime.Now, value));
                writer.WriteLine(value);
            }
        }


        public static void OudMain()
        {

            //TestServerClientMain.TestEmptyGame();
            //Wereld.QuadTree.TestQuadTreeStructure();
            //Engine.Model.TestRenderModel();
            //Engine.ModelManager.TestRenderMultipleSameModels();
            //Wereld.QuadTree.TestOrdenEntities();
            //Engine.LineManager3D.TestRenderLines();
            //Wereld.QuadTree.TestRender();
            //TestServerQuadtree();

            //XNAGeoMipMap.TerrainChunk.TestSaveTerrain();
            //XNAGeoMipMap.TerrainChunk.TestLoadTerrain();
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
            //XNAGeoMipMap.TerrainChunk.TestCreateTerrain();
            //XNAGeoMipMap.TerrainChunk.TestMultipleTerrains();
            //XNAGeoMipMap.TerrainChunk.TestCalculateErrors();

            //TestServerClientNetwork001();
            //TestWorld001();
            //TestServerQuadtreeCollision();

            //ColladaModel.TestColladaModel001();
            //ColladaModel.TestShowBones();
            //ColladaModel.TestColladaModelSkinned();
            //ColladaModel.TestColladaModelSkinnedSimple();

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
            //XNAGame.TestRunXNAGame();
            //TestXNAGame.TestEmptyGame();
            //ColladaModel.TestLoadColladaModel();
            //TWModel.TestRenderModelFromCollada();
            //LineManager3D.TestRenderLines();
            //TODO: DatabaseOud.TestSerialize();
            //TODO: TWWereld.TestWereld001();
            //TODO: Entity.TestReadWriteXML();
            //TODO: TWModel.TestSaveLoadXML();
            //TODO: WereldMesh.TestSerialize();
            //TODO: ShaderEffectCode.TestSerialize();
            //TODO: WereldClientRenderer.TestRenderModel();
            //Gui.GuiWindow.TestRenderWindow001();
            //Gui.GuiButton.TestRenderButton();
            //Gui.GuiImage.TestRenderImage();
            //Gui.GuiListBox<object>.TestRenderListBox();
            //EditorGrid.TestRender();
            //TWEditor.TestEditor();
            //EditorPlaceEntityTool.TestTool();
            //EditorMoveEntityTool.TestTool();

            //System.Windows.Forms.Application.Run(new TestForm());


            //Editor.EditorXMLSerializer.TestSerializeWorld();

            //Torus.TestCalculateDistanceLineTorus();

            //MathQuartic.TestQuartic();
            //MathExtra.Solve.TestSolvePolynomial();

            //Editor.GameControlPanel.TestCreateDispose();
            //Editor.XNAGameControl.TestCreateDispose();


            //CascadedShadowMaps.CSMTest.RunAutomated();
            //Sky.SkyTest.RunAutomated();
            //Water.WaterTest.RunAutomated();
            //UnitTest.Run<TestShadowsScattering>();
            //UnitTest.Run<TestWaterShadowsScattering>();
            //TheWizards.TerrainChunk.Editor.TerainEditorPlugin.TestTerrainEditor();

            //ColladaShader.TestLoadShader();
            //ColladaModel.TestLoadColladaModel();

            //SkinnedModel.TestRenderBones();
            //SkinnedModel.TestRenderAnimated();
            //TheWizards.Entities.Editor.EntityEditorPlugin.TestEntityEditor();
            //TheWizards.Entities.Editor.PutObjectsTool.TestPutObjectsTool();
            //TheWizards.Character.Editor.CharacterEditor.TestCharacterEditor();
            //TheWizards.Graphics.BoxMesh.TestRenderBoxMesh();




        }

        public static void ReproducePhysXDOTNetCharacterErrorTests()
        {
            /*System.Threading.Thread t = new Thread(
                delegate()
                {
                    test.TestPlayerInputOffline();
                } );
            t.Start();
            t.Join();
            t = new Thread( delegate()
               {
                   test.TestPlayerInputOffline();
               } );
            t.Start();*/
            /*System.Threading.Thread t = new Thread(
                delegate()
                {
                    TestPhysicsError();
                } );
            t.Start();
            t.Join();
            t = new Thread( delegate()
               {
                   TestPhysicsError();
               } );
            t.Start();*/

            //ReproduceCharacterError();
            //ReproduceCharacterErrorThreads();
        }
        public static void ReproduceCharacterError()
        {
            Game game1 = new Game();
            //game1.Run();
            RunCharacterTest();
            RunCharacterTest();

        }
        public static void ReproduceCharacterErrorThreads()
        {

            Thread t = new Thread(
                delegate()
                {
                    Game game1 = new Game();
                    //game1.Run();
                    RunCharacterTest();
                });
            t.Start();

            t.Join();

            t = new Thread(
                delegate()
                {
                    RunCharacterTest();
                });
            t.Start();

        }

        public static void RunCharacterTest()
        {

            Core core = new Core();
            StillDesign.PhysX.Scene scene = core.CreateScene();

            ControllerManager manager = scene.CreateControllerManager();

            CapsuleControllerDescription desc = new CapsuleControllerDescription(2, 10);
            CapsuleController capsuleController = manager.CreateController<CapsuleController>(desc);


            BoxShapeDescription boxShapeDesc = new BoxShapeDescription(1, 1, 1);
            ActorDescription actorDesc = new ActorDescription(boxShapeDesc);
            actorDesc.BodyDescription = new BodyDescription(1f);

            Actor actor = scene.CreateActor(actorDesc);

            //capsuleController.Move( Vector3.Up );

            // Update Physics
            scene.Simulate(1.0f / 60.0f);
            scene.FlushStream();
            scene.FetchResults(SimulationStatus.RigidBodyFinished, true);

            capsuleController.Move(Vector3.Up);

            core.Dispose();
        }

        public static void TestPhysicsError()
        {
            Game game1 = new Game();
            game1.Run();
            /*XNAGame game = new XNAGame();
            bool flag = false;
            game.UpdateEvent += delegate
            {
                if ( flag ) game.Exit();
                flag = true;
            };

            game.Run();*/

            using (PhysicsEngine engine = new PhysicsEngine())
            {
                StillDesign.PhysX.Scene scene;
                CapsuleController controller = null;

                ControllerManager manager;

                /*
                game.InitializeEvent +=
                    delegate
                    {*/

                engine.Initialize(null);
                scene = engine.Scene;


                manager = scene.CreateControllerManager();


                CapsuleControllerDescription capsuleControllerDesc = new CapsuleControllerDescription(0.5f, 1);

                CapsuleController capsuleController = manager.CreateController<CapsuleController>(capsuleControllerDesc);

                controller = capsuleController;

                ActorDescription actorDesc;
                Actor actor;

                BoxShapeDescription boxShapeDesc = new BoxShapeDescription(1, 1, 1);

                actorDesc = new ActorDescription(boxShapeDesc);
                actorDesc.BodyDescription = new BodyDescription(1f);

                actor = engine.Scene.CreateActor(actorDesc);
                actor.GlobalPosition = new Vector3(1, 4, 0);

                controller.Move(Vector3.Up);
                engine.Update(null);
                controller.Move(Vector3.Up);


                /*    };

                game.UpdateEvent += delegate { if (flag) game.Exit();
                                                 flag = true; };

                game.Run();*/



            }




        }

        public static void RunGame()
        {
            throw new NotImplementedException();
            //ServerClientMain main = new ServerClientMain();
            //main.Run();
        }
    }
}

