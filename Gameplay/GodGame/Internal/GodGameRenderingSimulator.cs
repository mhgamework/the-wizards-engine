using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class GodGameRenderingSimulator : ISimulator
    {
        private UIRenderer uiRenderer;
        private SimpleWorldRenderer simpleWorldRenderer;

        public GodGameRenderingSimulator(World world, PlayerInputSimulator playerInputSimulator, PlayerState localPlayer, SimpleWorldRenderer worldRenderer)
        {
            uiRenderer = new UIRenderer(world, localPlayer, playerInputSimulator);
            simpleWorldRenderer = worldRenderer;
        }

        public void Simulate()
        {
            uiRenderer.Simulate();
            simpleWorldRenderer.Simulate();
        }
    }
}