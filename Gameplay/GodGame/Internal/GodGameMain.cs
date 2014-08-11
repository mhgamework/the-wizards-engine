using System.IO;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Runs the godgame
    /// </summary>
    public class GodGameMain
    {
        private readonly WorldPersister persister;
        public World World { get; private set; }

        public GodGameMain(TWEngine engine, GameState state, PlayerInputSimulator playerInputSimulator, WorldPersister persister)
        {
            this.persister = persister;
            var world = state.World;
            engine.AddSimulator(playerInputSimulator);

            engine.AddSimulator(new TickSimulator(world));
            engine.AddSimulator(new UIRenderer(world, new PlayerState(), playerInputSimulator));
            engine.AddSimulator(new SimpleWorldRenderer(world));
            engine.AddSimulator(new ClearStateChangesSimulator(state));
            engine.AddSimulator(new WorldRenderingSimulator());


        }

        public void LoadSave()
        {
            if (!persister.GetDefaultSaveFile().Exists) return;
            persister.Load(World, persister.GetDefaultSaveFile());
        }


    }
}