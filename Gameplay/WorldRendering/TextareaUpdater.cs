using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering.Text;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Responsible for processing changes to a textarea
    /// </summary>
    public class TextareaUpdater
    {
        private List<Textarea> areas = new List<Textarea>();

        public void Update()
        {
            foreach (var change in TW.Model.GetChangesOfType<Textarea>())
            {
                var area = change.ModelObject as Textarea;
                if (change.Change == ModelContainer.ModelChange.Removed)
                {
                    areas.Remove(area);
                    area.get<TextTexture>().Dispose();

                    continue;
                }


                if (change.Change == ModelContainer.ModelChange.Added)
                {
                    areas.Add(area);
                    area.set(new Data());

                }

                var data = area.get<Data>();
                var tex = area.get<TextTexture>();

                if ((data.OldSize - area.Size).LengthSquared() > 0.001)
                {
                    // Size changed
                    if (tex != null)
                        tex.Dispose();

                    tex = new TextTexture(TW.Game, (int)area.Size.X, (int)area.Size.Y);
                    area.set(tex);
                }


                tex.SetFont(area.FontFamily, area.FontSize);
                tex.Clear();
                tex.DrawText(area.Text, new Vector2(), area.Color);
                tex.UpdateTexture();

            }
        }

        public void Render()
        {
            TW.Game.Device.ImmediateContext.OutputMerger.BlendState = TW.Game.HelperStates.AlphaBlend;
            foreach (var a in areas)
            {
                if (!a.Visible) continue;
                var data = a.get<TextTexture>();
                TW.Game.TextureRenderer.Draw(data.GPUTexture.View, a.Position, a.Size);
            }

        }

        private class Data
        {
            public Vector2 OldSize;
        }

    }
}
