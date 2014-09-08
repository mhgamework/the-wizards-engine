﻿using System.Collections.Generic;
using MHGameWork.TheWizards.Rendering.Text;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    public interface ITextareaUpdater : ISimulator
    {
    }

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

    /// <summary>
    /// Responsible for processing changes to a textarea
    /// Uses the TextTexture: GDI->disk->D3D == slow
    /// </summary>
    public class SlowTextareaUpdater : ITextareaUpdater
    {
        private List<Textarea> areas = new List<Textarea>();


        public void Simulate()
        {
            foreach (var change in TW.Data.GetChangesOfType<Textarea>())
            {
                var area = change.ModelObject as Textarea;
                if (change.Change == TheWizards.Data.ModelChange.Removed)
                {
                    areas.Remove(area);
                    area.get<TextTexture>().Dispose();

                    continue;
                }


                if (change.Change == TheWizards.Data.ModelChange.Added)
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

                    tex = new TextTexture(TW.Graphics, (int)area.Size.X, (int)area.Size.Y);
                    area.set(tex);
                }

                tex.SetFont(area.FontFamily, area.FontSize);
                tex.Clear(area.BackgroundColor);
                tex.DrawText(area.Text, new Vector2(), area.Color);
                tex.UpdateTexture();


            }

            render();
        }

        private void render()
        {
            TW.Graphics.Device.ImmediateContext.OutputMerger.BlendState = TW.Graphics.HelperStates.AlphaBlend;
            foreach (var a in areas)
            {
                if (!a.Visible) continue;
                var data = a.get<TextTexture>();
                TW.Graphics.TextureRenderer.Draw(data.GPUTexture.View, a.Position, a.Size);
            }

        }

        private class Data
        {
            public Vector2 OldSize;
        }

    }
}
