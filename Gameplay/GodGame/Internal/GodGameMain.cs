using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Runs the godgame
    /// </summary>
    public class GodGameMain
    {
        private readonly WorldPersister persister;
        public Model.World World { get; private set; }

        public GodGameMain(TWEngine engine, Model.World world, WorldPersister persister, IEnumerable<IPlayerTool> playerInputs)
        {
            this.persister = persister;
            World = world;


            // Game state
            var gameState = new GameState(world);
            var localPlayerState = new PlayerState();

            gameState.AddPlayer(localPlayerState);


            // Rendering
            var simpleWorldRenderer = new SimpleWorldRenderer(world, new ChunkedVoxelWorldRenderer(world));

            // Input

            var playerInputHandler = new PlayerInputHandler(playerInputs, world, persister, localPlayerState);

            // Simulators

            var playerInputSimulator = new PlayerInputSimulator(world, playerInputHandler, simpleWorldRenderer);
            var tickSimulator = new TickSimulator(world);
            var uiRenderer = new UIRenderer(world, localPlayerState, playerInputSimulator);
            var clearStateChangesSimulator = new ClearStateChangesSimulator(gameState);
            var worldRenderingSimulator = new WorldRenderingSimulator();


            engine.AddSimulator(playerInputSimulator);
            engine.AddSimulator(tickSimulator);
            engine.AddSimulator(uiRenderer);
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