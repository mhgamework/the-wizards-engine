using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Runs the godgame
    /// </summary>
    public class GodGameMain
    {
        public World World { get; private set; }



        public GodGameMain(TWEngine engine, World world, PlayerInputSimulator playerInputSimulator)
        {
            World = world;
            engine.AddSimulator(playerInputSimulator);

            engine.AddSimulator(new TickSimulator(world));
            engine.AddSimulator(new UIRenderer(world, playerInputSimulator));
            engine.AddSimulator(new SimpleWorldRenderer(world));
            engine.AddSimulator(new ClearWorldChangesSimulator(world));
            engine.AddSimulator(new WorldRenderingSimulator());
        }
    }
}