using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class GodGameRenderingSimulator : ISimulator
    {
        private UIRenderer uiRenderer;
        private SimpleWorldRenderer simpleWorldRenderer;

        public GodGameRenderingSimulator(World world, PlayerInputSimulator playerInputSimulator, PlayerState localPlayer)
        {
            uiRenderer = new UIRenderer(world, localPlayer, playerInputSimulator);
            simpleWorldRenderer = new SimpleWorldRenderer(world);
        }

        public void Simulate()
        {
            uiRenderer.Simulate();
            simpleWorldRenderer.Simulate();
        }
    }
}