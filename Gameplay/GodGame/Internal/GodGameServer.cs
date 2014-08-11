using System.IO;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class GodGameServer
    {
        private readonly GameState state;
        private readonly WorldPersister persister;
        //public World World { get; private set; }

        public GodGameServer(TWEngine engine, GameState state, PlayerInputSimulator playerInputSimulator, WorldPersister persister, ISimulator tickSimulator, PlayerState localPlayer)
        {
            this.state = state;
            this.persister = persister;
            //World = world;
            engine.AddSimulator(playerInputSimulator);
            engine.AddSimulator(tickSimulator);
            engine.AddSimulator(new GodGameRenderingSimulator(state.World,playerInputSimulator,localPlayer));
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new ClearStateChangesSimulator(state));
            
            loadSave();

        }

        private void loadSave()
        {
            if (!persister.GetDefaultSaveFile().Exists) return;
            persister.Load(state.World, persister.GetDefaultSaveFile());
        }

       
    }
}