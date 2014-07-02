using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame._Tests;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Runs the godgame
    /// </summary>
    public class GodGameMain
    {
        public World World { get; private set; }

        public GodGameMain(TWEngine engine, World world)
        {
            World = world;
            var playerInputHandler = new PlayerInputHandler(world);
            var playerInputSimulator = new PlayerInputSimulator(playerInputHandler, world);
            engine.AddSimulator(playerInputSimulator);

            engine.AddSimulator(new TickSimulator(world));
            engine.AddSimulator(new UIRenderer(world, playerInputSimulator));
            engine.AddSimulator(new SimpleWorldRenderer(world));
            engine.AddSimulator(new WorldRenderingSimulator());
        }
    }
}