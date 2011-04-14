using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ClientMain;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards.ServerClientMain
{
    public class TheWizardsServerClient
    {
        public TheWizardsServerClient()
        {
            
        }

        public void Run()
        {
            throw new NotImplementedException();
           /* // Need core disposal fix


            //TODO: implement this
            //server.ConnectClientNonNetworked();


            PhysicsEngine engine = new PhysicsEngine();
            engine.Initialize();
            XNAGame game = new XNAGame();

            TheWizardsServer server = new TheWizardsServer(engine.Core, game);
            TheWizardsClient client = new TheWizardsClient(engine.Core, game);

            server.Start();
            client.RunInExisting();


            game.Run();

            engine.Dispose();*/
        }
    }
}
