using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.GodGame.Internal.Rendering
{
    public class GodGameRenderingSimulator : ISimulator
    {
        private UIRenderer uiRenderer;
        private SimpleWorldRenderer simpleWorldRenderer;

        public GodGameRenderingSimulator(Model.World world, PlayerInputSimulator playerInputSimulator, PlayerState localPlayer, SimpleWorldRenderer worldRenderer)
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