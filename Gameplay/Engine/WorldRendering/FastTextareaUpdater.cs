using System.Linq;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    /// <summary>
    /// Responsible for processing changes to a textarea
    /// Uses the DX11FontWrapper for rendering: fast!
    /// </summary>
    public class FastTextareaUpdater : ITextareaUpdater
    {
        private DX11FontWrapper fontWrapper;

        public FastTextareaUpdater()
        {
            fontWrapper = new DX11FontWrapper(TW.Graphics.Device,"Verdana");
        }

        public void Simulate()
        {
            render();
        }

        private void render()
        {
            TW.Graphics.Device.ImmediateContext.OutputMerger.BlendState = TW.Graphics.HelperStates.AlphaBlend;
            foreach (var a in TW.Data.Objects.OfType<Textarea>())
            {
                if (!a.Visible) continue;

                var pxSize = a.FontSize/12f*16f;

                TW.Graphics.TextureRenderer.DrawColor(a.BackgroundColor, a.Position, a.Size);
                fontWrapper.Draw(a.Text, pxSize , (int)a.Position.X, (int)a.Position.Y, a.Color);
            }
        }

    }
}