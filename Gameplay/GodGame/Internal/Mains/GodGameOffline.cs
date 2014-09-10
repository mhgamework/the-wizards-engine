using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Persistence;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Runs the godgame in offline mode, that is, without using the networking layer. 
    /// The offline game uses a single 'gamestate' instead of a server and client gamestate
    /// </summary>
    public class GodGameOffline
    {
        private readonly WorldPersisterService persister;
        public Model.World World { get; private set; }

        public GodGameOffline(TWEngine engine, Model.World world)
        {
            World = world;


            // Game state
            var gameState = new GameState(world);
            var localPlayerState = new PlayerState();

            gameState.AddPlayer(localPlayerState);


            // Rendering
            var simpleWorldRenderer = new WorldRenderingService(world, new ChunkedVoxelWorldRenderer(world),localPlayerState);

            // Input
            var toolsFactory = new PlayerToolsFactory(world, localPlayerState);
            var playerInputHandler = new PlayerInputHandler(toolsFactory, world, persister, localPlayerState);

            // Persistance
            persister = new WorldPersisterService(new GameplayObjectsSerializer(toolsFactory));
            // Simulators

            var playerInputSimulator = new UserInputProcessingService(world, playerInputHandler, simpleWorldRenderer);
            var tickSimulator = new WorldSimulationService(world);
            var uiRenderer = new UIRenderingService(world, localPlayerState, playerInputSimulator);
            var developerConsoleSimulator = new DeveloperConsoleService(playerInputSimulator, new AllCommandProvider(persister,world));
            var clearStateChangesSimulator = new ClearGameStateChangesService(gameState);
            var worldRenderingSimulator = new WorldRenderingSimulator();

            //TODO: these simulators form some kind of configuration, so maybe try splitting them into configuration and features
            //   Place the configuration code into methods in this class.
            engine.AddSimulator(playerInputSimulator); 
            engine.AddSimulator(tickSimulator);
            engine.AddSimulator(uiRenderer);
            engine.AddSimulator(developerConsoleSimulator);
            engine.AddSimulator(simpleWorldRenderer);
            engine.AddSimulator(clearStateChangesSimulator);
            engine.AddSimulator(worldRenderingSimulator);


        }

        public void LoadSave()
        {
            if (!persister.GetDefaultSaveFile().Exists) return;
            persister.Load(World, persister.GetDefaultSaveFile());
        }


    }
}