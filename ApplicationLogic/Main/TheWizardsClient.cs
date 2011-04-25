using System;
using System.Collections.Generic;
using System.Threading;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Packets;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scripting;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.World;
using MHGameWork.TheWizards.World.Static;
using Microsoft.Xna.Framework.Graphics;
using PlayerController = MHGameWork.TheWizards.GamePlay.PlayerController;

namespace MHGameWork.TheWizards.Main
{
    /// <summary>
    /// This forms the starting point for the client for the wizards. Start this to start the full client.
    /// </summary>
    public class TheWizardsClient
    {

        private XNAGame xnaGame;
        public XNAGame XNAGame { get { return xnaGame; } }


        private PhysicsEngine physicsEngine;
        private PhysicsDebugRenderer physicsDebugRenderer;

        private bool needLogin;

        float time = 0;

        private volatile bool loginComplete;
        private ClientPacketManagerNetworked packetManager;
        private ClientSyncer clientSyncer;
        private IClientPacketTransporter<DataPacket> playerControllerTransporter;
        private PlayerInputClient inputClient;
        private PlayerThirdPersonCamera cam;
        private List<PlayerController> controllerClients;
        private TreeClient treeClient;
        private ClientStaticWorldObjectSyncer clientStaticWorldObjectSyncer;
        private ClientStaticWorldObjectSyncer staticWorldObjectSyncer;
        private SimpleMeshRenderer renderer;

        private object updateLock = new object();

        public TheWizardsClient()
        {
            physicsEngine = new PhysicsEngine();
            xnaGame = new XNAGame();
            xnaGame.DrawFps = true;

            /*xnaGame.Graphics1.PreferredBackBufferWidth = 1440;
            xnaGame.Graphics1.PreferredBackBufferHeight = 900;
            xnaGame.Graphics1.ToggleFullScreen();*/

            xnaGame.Window.Title = "The Wizards Client - MHGameWork All Rights Reserved";
            treeClient = new TreeClient();


        }


        void xnaGame_Exiting(object sender, EventArgs e)
        {
        }

        void xnaGame_DrawEvent()
        {
            physicsDebugRenderer.Render(xnaGame);

            xnaGame.GraphicsDevice.RenderState.CullMode = CullMode.None;
            treeClient.Render();
        }

        void xnaGame_UpdateEvent()
        {
            // Test for doing synchronized calls to the main thread:
            lock (this)
            {
                // Signal waiting threads that the main thread is locked
                //Monitor.Pulse(this);

                //Monitor.Wait(this); // Wait until other thread pulses ready
            }



            var game = xnaGame;


            updatePlayers(game);


            physicsEngine.Update(xnaGame);
            physicsDebugRenderer.Update(xnaGame);

            treeClient.Update();

            if (clientStaticWorldObjectSyncer != null)
                clientStaticWorldObjectSyncer.Update(game.Elapsed);
            if (worldObjectFactory != null)
                worldObjectFactory.Update();

            lock (updateLock)
            {
                updateWaiting = true;

                while (blockUpdateWaitingCount > 0)
                {

                    Monitor.Pulse(updateLock);
                    Monitor.Wait(updateLock);

                }

                updateWaiting = false;
            }

        }


        private AutoResetEvent test = new AutoResetEvent(false);
        private ManualResetEvent blockEvent = new ManualResetEvent(false);

        private int blockUpdateWaitingCount;
        private bool updateWaiting;
        private SimpleAssetStaticWorldObjectFactory worldObjectFactory;

        private void blockUpdate()
        {
            lock (updateLock)
            {
                blockUpdateWaitingCount++;
                while (!updateWaiting)
                    Monitor.Wait(updateLock);
            }
        }
        private void unblockUpdate()
        {
            lock (updateLock)
            {
                if (blockUpdateWaitingCount < 0) throw new InvalidOperationException();
                blockUpdateWaitingCount--;
                Monitor.Pulse(updateLock);
            }

        }

        private void updatePlayers(XNAGame game)
        {
            if (playerControllerTransporter == null) return;
            if (clientSyncer == null) return;

            while (playerControllerTransporter.PacketAvailable)
            {
                var p = playerControllerTransporter.Receive();
                var c = new GamePlay.PlayerController(new PlayerData());
                c.Initialize(game);
                game.AddXNAObject(c);

                var clientA = clientSyncer.CreateActor(c);
                clientA.ID = BitConverter.ToUInt16(p.Data, 0);


                if (cam == null)
                {
                    cam = new PlayerThirdPersonCamera(game, c.Player);
                    cam.Enabled = true;
                    game.AddXNAObject(cam);
                    game.SetCamera(cam);
                }

            }

            for (int i = 0; i < controllerClients.Count; i++)
            {
                controllerClients[i].Update(game);
            }
            if (clientSyncer != null)
                clientSyncer.Update(game.Elapsed);
            if (staticWorldObjectSyncer != null)
                staticWorldObjectSyncer.Update(xnaGame.Elapsed);



            if (inputClient != null)
            {
                inputClient.Update(game);
                if (cam != null)
                    inputClient.HorizontalLookAngle = cam.LookAngleHorizontal;
            }
        }

        void xnaGame_InitializeEvent(object sender, EventArgs e)
        {
            xnaGame.GetWindowForm().Location = new System.Drawing.Point(800, 0);
            physicsEngine = new PhysicsEngine();

            physicsEngine.Initialize(xnaGame);
            physicsDebugRenderer = new PhysicsDebugRenderer(xnaGame, physicsEngine.Scene);
            physicsDebugRenderer.Initialize(xnaGame);



            treeClient.Initialize(xnaGame);


            var vertexDeclarationPool = new VertexDeclarationPool();
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);

            var texturePool = new TexturePool();
            var meshPartPool = new MeshPartPool();

            renderer = new SimpleMeshRenderer(texturePool, meshPartPool, vertexDeclarationPool);



            xnaGame.AddXNAObject(renderer);
            xnaGame.AddXNAObject(vertexDeclarationPool);
            xnaGame.AddXNAObject(texturePool);
            xnaGame.AddXNAObject(meshPartPool);




            setScriptLayerScope();
        }

        private void setScriptLayerScope()
        {
            ScriptLayer.Game = xnaGame;
            ScriptLayer.ScriptRunner = new ScriptRunner(xnaGame);
            ScriptLayer.Physics = physicsEngine;
            ScriptLayer.Scene = physicsEngine.Scene;
        }

        private void setUp()
        {
            xnaGame.InitializeEvent += xnaGame_InitializeEvent;
            xnaGame.UpdateEvent += xnaGame_UpdateEvent;
            xnaGame.DrawEvent += xnaGame_DrawEvent;
            xnaGame.Exiting += xnaGame_Exiting;


            ((SpectaterCamera)xnaGame.Camera).FarClip = 1000;


            XNAGame.SpectaterCamera.NearClip = 0.1f;
            XNAGame.SpectaterCamera.FarClip = 1000f;





            controllerClients = new List<PlayerController>();


            var t = new Thread(startJob);
            t.Name = "ClientStartJob";
            t.Start();



        }


        private void startJob()
        {
            //var conn = ConnectTCP(10045, "127.0.0.1");
            var conn = ConnectTCP(10045, "5.23.165.201");
            conn.Receiving = true;
            packetManager = new ClientPacketManagerNetworked(conn);





            treeClient.StartJob(packetManager);


            while (renderer == null)
                Thread.Sleep(100);


            var syncer = new ClientAssetSyncer(packetManager, TWDir.GameData.CreateSubdirectory("ClientAssets"));
            syncer.Start();
            worldObjectFactory = new SimpleAssetStaticWorldObjectFactory(renderer, syncer);
            staticWorldObjectSyncer = new ClientStaticWorldObjectSyncer(packetManager, worldObjectFactory);


            packetManager.WaitForUDPConnected();
            packetManager.SyncronizeRemotePacketIDs();



            treeClient.RequestInitialState();
            startJobPlayerPhysics();



        }

        private void startJobPlayerPhysics()
        {
            playerControllerTransporter = packetManager.CreatePacketTransporter("PlayerControllerID",
                                                                                new Networking.Packets.DataPacket.Factory(), PacketFlags.TCP);
            clientSyncer = new ClientSyncer(packetManager);
            inputClient = new PlayerInputClient(packetManager);
        }


        private void establishTCP(out TCPConnection conn1, out  TCPConnection conn2)
        {
            TCPConnectionListener listener = new TCPConnectionListener(10010);
            conn2 = null;

            TCPConnection connected = null;

            AutoResetEvent ev = new AutoResetEvent(false);

            listener.ClientConnected += delegate(object sender, TCPConnectionListener.ClientConnectedEventArgs e)
            {
                connected = new TCPConnection(e.CL);
                ev.Set();
            };


            listener.Listening = true;

            Thread.Sleep(500);

            conn1 = ConnectTCP(10010, "127.0.0.1");


            ev.WaitOne();

            conn2 = connected;



            listener.Listening = false;
            listener.Dispose();
        }

        public static TCPConnection ConnectTCP(int port, string ip)
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

    }
}