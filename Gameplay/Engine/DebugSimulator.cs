using MHGameWork.TheWizards.Engine.WorldRendering;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for showing debug information
    /// </summary>
    public class DebugSimulator : ISimulator
    {
        private Textarea drawcallArea;

        public DebugSimulator()
        {
            drawcallArea = new Textarea();
            int left = 650;
            drawcallArea.Position = new SlimDX.Vector2(left, 10);
            drawcallArea.Size = new SlimDX.Vector2(800 - left - 10, 20);

        }

        public void Simulate()
        {
            drawcallArea.Text = "Drawcalls: " + TW.Graphics.AcquireRenderer().DrawCalls;
        }
    }
}
