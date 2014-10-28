using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Networking;
using MHGameWork.TheWizards.GodGame.Internal.Networking.Packets;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Persistence;
using MHGameWork.TheWizards.GodGame._Engine;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Runs the godgame in offline mode, that is, without using the networking layer. 
    /// The offline game uses a single 'gamestate' instead of a server and client gamestate
    /// </summary>
    public class GodGameOffline
    {
        public Model.World World { get; private set; }
        public WorldSimulationService WorldSimulationService;

        private UserInputProcessingService userInputProcessingService;
        private UIRenderingService uiRenderingService;
        private DeveloperConsoleService developerConsoleService;
        private ClearGameStateChangesService clearStateChangesSimulator;
        private WorldRenderingSimulator worldRenderingSimulator;
        private WorldRenderingService simpleWorldRenderer;


        public GodGameOffline(WorldSimulationService worldSimulationService,
            Model.World world,
            WorldPersisterService persisterService,
            UserInputProcessingService userInputProcessingService,
            UIRenderingService uiRenderingService,
            DeveloperConsoleService developerConsoleService,
            ClearGameStateChangesService clearStateChangesSimulator,
            WorldRenderingSimulator worldRenderingSimulator,
            WorldRenderingService simpleWorldRenderer
            )
        {
            World = world;
            this.WorldSimulationService = worldSimulationService;
            this.clearStateChangesSimulator = clearStateChangesSimulator;



             this.userInputProcessingService = userInputProcessingService;
            this.uiRenderingService = uiRenderingService;
            this.developerConsoleService = developerConsoleService;
            this.clearStateChangesSimulator = clearStateChangesSimulator;
            this.worldRenderingSimulator = worldRenderingSimulator;
            this.simpleWorldRenderer = simpleWorldRenderer;



            persisterService.Load(world, TWDir.GameData.GetChild("Saves/GodGame").CreateFile("auto.xml"));

        }



        public void AddSimulatorsToEngine(TWEngine engine)
        {
            //engine.AddSimulator(updateConnectedClients, "Server-UpdateConnectedClientsSim");
            //engine.AddSimulator(processClientInputs, "Server-PlayerInputProcessingSim");
            
            //engine.AddSimulator(sendGameStateUpdates, "Server-SendStateSim");
            //engine.AddSimulator(clearStateChangesSimulator, "Server-ClearChangesSim");


            engine.AddSimulator(userInputProcessingService, "Client-UserInputProcessingSim");
            engine.AddSimulator(WorldSimulationService, "Server-WorldSimulationSim");
            //engine.AddSimulator(sendPlayerInputs, "Client-PlayerInputsSendingSim");
            //engine.AddSimulator(applyServerStateChanges, "Client-applyServerStateChangesSim");
            engine.AddSimulator(uiRenderingService, "Client-UIRenderingSim");
            engine.AddSimulator(developerConsoleService, "Client-DevConsoleSim");
            engine.AddSimulator(simpleWorldRenderer, "Client-WorldRenderingSim");
            engine.AddSimulator(clearStateChangesSimulator, "Client-ClearChangesSim");
            engine.AddSimulator(worldRenderingSimulator, "Client-EngineWorldRenderingSimulator");

        }

    }
}