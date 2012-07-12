using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DirectX11;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Synchronization;
using MHGameWork.TheWizards.Tiling;
using MHGameWork.TheWizards.WorldRendering;

namespace MHGameWork.TheWizards.Main
{
    /// <summary>
    /// This forms the starting point for the client for the wizards. Start this to start the full client.
    /// </summary>
    public class TheWizardsClient
    {

        private DX11Game xnaGame;
        public DX11Game XNAGame { get { return xnaGame; } }


        private PhysicsEngine physicsEngine;
        //private PhysicsDebugRenderer physicsDebugRenderer;

        private ClientPacketManagerNetworked packetManager;
        private ModelContainer.ModelContainer container;
        private List<ISimulator> simulators = new List<ISimulator>();

        private ManualResetEvent runningEvent = new ManualResetEvent(false);

        private bool holdLoop = false;

        /// <summary>
        /// If this is true, the game will just run without holding each step
        /// </summary>
        private bool runContinuously = false;

        public TheWizardsClient()
        {

        }


        /// <summary>
        /// In this mode, the client will wait until StepSingle is called, and then process one frame
        /// </summary>
        public void EnableSingleStepMode()
        {
            runContinuously = false;
        }

        private void setScriptLayerScope()
        {
            TW.Game = xnaGame;
            TW.PhysX = physicsEngine;
            TW.Scene = physicsEngine.Scene;
            TW.Model = container;
        }

        private void setUp()
        {
            
            physicsEngine = new PhysicsEngine();
            xnaGame = new DX11Game();
            xnaGame.GameLoopEvent += xnaGame_GameLoopEvent;
            /*xnaGame.Graphics1.PreferredBackBufferWidth = 1440;
        xnaGame.Graphics1.PreferredBackBufferHeight = 900;
        xnaGame.Graphics1.ToggleFullScreen();*/
            //xnaGame.Window.Title = "The Wizards Client - MHGameWork All Rights Reserved";

            //((SpectaterCamera)xnaGame.Camera).FarClip = 1000;


            //XNAGame.SpectaterCamera.NearClip = 0.1f;
            //XNAGame.SpectaterCamera.FarClip = 1000f;




            xnaGame.InitDirectX();
            //xnaGame.GetWindowForm().Location = new System.Drawing.Point(800, 0);
            physicsEngine = new PhysicsEngine();

            physicsEngine.Initialize();
            //physicsDebugRenderer = new PhysicsDebugRenderer(xnaGame, physicsEngine.Scene);
            //physicsDebugRenderer.Initialize(xnaGame);


            var conn = ConnectTCP(10045, "127.0.0.1");
            //var conn = ConnectTCP(10045, "5.23.165.201");
            conn.Receiving = true;
            packetManager = new ClientPacketManagerNetworked(conn);



            container = new ModelContainer.ModelContainer();
            setScriptLayerScope();

            var player = new PlayerData(); // create a player

            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.GenerateRandomCacheFile("", "dll"));
            var transporter = new ServerPacketTransporterNetworked<ChangePacket>();
            transporter.AddClientTransporter(new DummyClient("Server"),
                                             packetManager.CreatePacketTransporter("NetworkSyncer",
                                                                                   gen.GetFactory<ChangePacket>(),
                                                                                   PacketFlags.TCP));
            gen.BuildFactoriesAssembly();


            this
                .AddSimulator(new LocalPlayerSimulator(player)) 
                .AddSimulator(new TileEditorSimulator())
                .AddSimulator(new NetworkSyncerSimulator(transporter))
                .AddSimulator(new ThirdPersonCameraSimulator())
                .AddSimulator(new RenderingSimulator());


            TW.Model.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.FirstPerson;
            TW.Model.GetSingleton<CameraInfo>().FirstPersonCameraTarget = player.Entity;



            packetManager.WaitForUDPConnected();
            packetManager.SyncronizeRemotePacketIDs();




            

        }

        private TheWizardsClient AddSimulator(ISimulator sim)
        {
            simulators.Add(sim);
            return this;
        }

        void xnaGame_GameLoopEvent(DX11Game obj)
        {
            runningEvent.Set();
            lock (this)
            {
                if (!runContinuously)
                {
                    // Wait while the loop is on hold
                    while (holdLoop)
                        Monitor.Wait(this);
                }


                var game = xnaGame;
                setScriptLayerScope();


                foreach (var sim in simulators)
                {
                    sim.Simulate();
                }

                container.ClearDirty();

                physicsEngine.Update(xnaGame.Elapsed);
                //physicsDebugRenderer.Update();


                Monitor.Pulse(this); // Give control back to the singlestep method, if its listening
            }



        }



        private static TCPConnection ConnectTCP(int port, string ip)
        {
            AutoResetEvent ev = new AutoResetEvent(false);

            var conn = new TCPConnection();
            conn.ConnectedToServer += delegate { ev.Set(); };

            conn.Connect(ip, port);
            if (!ev.WaitOne(5000)) throw new Exception("Connection timed out!");

            return conn;
        }

        public void Run()
        {
            setUp();

            xnaGame.Run();
        }


        public void Exit()
        {
            xnaGame.Exit();
            xnaGame = null;
        }


        /// <summary>
        /// This will make the game run a single frame.
        /// If the client is not yet initialized, this method will block
        /// SingleStepMode must be enabled!
        /// </summary>
        public void StepSingle()
        {
            WaitUntilRunning();

            lock (this)
            {
                holdLoop = false; // Allow the loop to continue, but still blocks the loop
                Monitor.Pulse(this); // Puts the game loop in the ready queue
                Monitor.Wait(this); // Releases the lock, the game loop can now run!
                holdLoop = true; // Hold the loop again
            }
        }

        public void WaitUntilRunning()
        {
            runningEvent.WaitOne();
        }
    }
}