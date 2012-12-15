using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
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

        //private List<Textarea> areasAwaitingUpdate = new List<Textarea>();

        private float nextUpdate;

        public void Update()
        {
            nextUpdate += TW.Graphics.Elapsed;
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

                //if (!areasAwaitingUpdate.Contains(area))
                //    areasAwaitingUpdate.Add(area);

                tex.SetFont(area.FontFamily, area.FontSize);
                tex.Clear();
                tex.DrawText(area.Text, new Vector2(), area.Color);
                tex.UpdateTexture();


            }

            // This should work no clue why it doesn't
            //if (nextUpdate > 3f)
            //{
            //    nextUpdate = 0;
            //    foreach (var area in areasAwaitingUpdate)
            //    {
            //        var tex = area.get<TextTexture>();

            //        tex.SetFont(area.FontFamily, area.FontSize);
            //        tex.Clear();
            //        tex.DrawText(area.Text, new Vector2(), area.Color);
            //        tex.UpdateTexture();
            //    }

            //    areasAwaitingUpdate.Clear();


            //}
        }

        public void Render()
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
