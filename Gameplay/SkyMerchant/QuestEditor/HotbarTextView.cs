using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor
{
    /// <summary>
    /// Visualizes the hotbar using text
    /// </summary>
    public class HotbarTextView
    {
        private readonly Hotbar bar;
        private Textarea textarea;

        public HotbarTextView(Hotbar bar, Rendering2DComponentsFactory rendering)
        {
            this.bar = bar;

            textarea = rendering.CreateTextArea();
            textarea.Position = new Vector2(600, 10);
            textarea.Size = new Vector2(190, 400);
        }

        public void Update()
        {
            var txt = "";
            for (int i = 0; i < bar.NumSlots; i++)
            {
                string selected = bar.SelectedSlot == i ? "> " : "";
                string itemText = bar.GetHotbarItem(i) == null ? "---" : bar.GetHotbarItem(i).Name;
                txt += selected + (i+1) + ": " + itemText + "\n";
            }
            textarea.Text = txt;
        }
    }
}