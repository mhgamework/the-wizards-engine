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
            fontWrapper = new DX11FontWrapper(TW.Graphics.Device, "Verdana");
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

                var pxSize = a.FontSize / 12f * 16f;

                var txt = a.Text ?? ""; // Causes memory exception when string is null, could also be fixed in the C++ wrapepr

                TW.Graphics.TextureRenderer.DrawColor(a.BackgroundColor, a.Position, a.Size);
                if (a.AlignRight)
                    // When drawing right, the text writes from right to left, so use the right
                    fontWrapper.DrawRight(txt, pxSize, (int)(a.Position.X+a.Size.X), (int)a.Position.Y, a.Color);
                else
                    fontWrapper.Draw(txt, pxSize, (int)a.Position.X, (int)a.Position.Y, a.Color);
            }
        }

    }
}