using System.IO;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class GodGameProxy
    {
        private readonly WorldPersister persister;
        public World World { get; private set; }

        public GodGameProxy(TWEngine engine, World world, PlayerInputSimulator playerInputSimulator, WorldPersister persister)
        {
            this.persister = persister;
            World = world;
            engine.AddSimulator(playerInputSimulator);

            engine.AddSimulator(new TickSimulator(world));
            engine.AddSimulator(new UIRenderer(world, playerInputSimulator));
            engine.AddSimulator(new SimpleWorldRenderer(world));
            engine.AddSimulator(new ClearWorldChangesSimulator(world));
            engine.AddSimulator(new WorldRenderingSimulator());


            loadSave();

        }

        private void loadSave()
        {
            if (!persister.GetDefaultSaveFile().Exists) return;
            persister.Load(World, persister.GetDefaultSaveFile());
        }

       
    }
}