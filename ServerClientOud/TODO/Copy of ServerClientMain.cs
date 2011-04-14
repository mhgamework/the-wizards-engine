using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay.Core;
using MHGameWork.TheWizards.ServerClient.Engine;

namespace MHGameWork.TheWizards.ServerClient
{
    public class ServerClientMain : AdminClient.AdminClientMain, Game3DPlay.Core.Elements.IRenderable
    {
        private ModelManager modelManager;
        private ShaderManager shaderManager;
        private TextureManager textureManager;

        private LineManager3D lineManager3D;

        private Server.ServerMainNew serverMain;
        public Client.PhysicsDebugRenderer serverDebugRenderer;

        private Wereld.Wereld wereld;
        private GameClient gameClient;

        public static ServerClientMain instance;

        protected Network.ProxyServer server;


        public ServerClientMain()
            : base()
        {
            instance = this;
            serverMain = new MHGameWork.TheWizards.Server.ServerMainNew();


            modelManager = new ModelManager(this);
            shaderManager = new ShaderManager(this);
            textureManager = new TextureManager(this);

            lineManager3D = new LineManager3D(this);


            debugRenderer.Enabled = false;

            serverDebugRenderer = new Client.PhysicsDebugRenderer(this,
                                            new NovodexWrapper.NovodexDebugRenderer(serverMain.XNAGame.Graphics.GraphicsDevice),
                                            serverMain.PhysicsScene);
            serverDebugRenderer.Enabled = true;

            wereld = new MHGameWork.TheWizards.ServerClient.Wereld.Wereld(this);

            server = new MHGameWork.TheWizards.ServerClient.Network.ProxyServer(this);

            gameClient = new GameClient(this);
        }


        public override void Dispose()
        {
            base.Dispose();
            ServerMain.Dispose();
        }

        public void CreateProxyServer()
        {
            server = new Network.ProxyServer(this);
        }




        public override void DoProcess(object sender, float nElapsed)
        {
            base.DoProcess(sender, nElapsed);
            serverMain.DoProcess(sender, nElapsed);
        }

        /*public override void DoTick(object sender, float nElapsed)
        {
            base.DoTick(sender, nElapsed);
            serverMain.DoTick(sender, nElapsed);
        }*/






        public virtual void OnBeforeRender(object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e)
        {

        }

        public virtual void OnRender(object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e)
        {
            lineManager3D.Render();
            wereld.Render();
        }

        public virtual void OnAfterRender(object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e)
        {

        }

        public override void OnLoad(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
        {
            base.OnLoad(sender, e);
            ModelManager.Load(sender, e);
            shaderManager.Load(sender, e);
            textureManager.Load(sender, e);
            lineManager3D.Load();

            serverDebugRenderer.DebugRenderer.setRenderDevice(XNAGame.Graphics.GraphicsDevice);
        }

        public override void OnUnload(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
        {

            base.OnUnload(sender, e);
            ModelManager.Unload(sender, e);
            shaderManager.Unload(sender, e);
            textureManager.Unload(sender, e);
            //lineManager3D.Unload(sender,e);
        }

        public override void OnProcess(object sender, MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
        {
            base.OnProcess(sender, e);
            if (e.Keyboard.IsKeyStateDown(Microsoft.Xna.Framework.Input.Keys.B))
            {
                debugRenderer.Enabled = true;
            }
            if (e.Keyboard.IsKeyStateUp(Microsoft.Xna.Framework.Input.Keys.B))
            {
                debugRenderer.Enabled = false;
            }

            wereld.Process(e);

            gameClient.Process( e );
        }

        public override void OnTick(object sender, MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e)
        {
            base.OnTick(sender, e);
            XNAGame.Window.Title = _processEventArgs.Elapsed.ToString();
            wereld.Tick(e);
        }


        public ModelManager ModelManager { get { return modelManager; } }
        public ShaderManager ShaderManager { get { return shaderManager; } }
        public TextureManager TextureManager { get { return textureManager; } }

        public LineManager3D LineManager3D { get { return lineManager3D; } }

        public Game3DPlay.Core.Elements.ProcessEventArgs ProcessEventArgs { get { return _processEventArgs; } }

        public Server.ServerMainNew ServerMain
        { get { return serverMain; } }

        public Wereld.Wereld Wereld
        { get { return wereld; } }

        public Network.ProxyServer Server { get { return server; } }

        public int Time { get { return _processEventArgs.Time; } }

        public GameClient GameClient
        { get { return gameClient; } }
    }
}
