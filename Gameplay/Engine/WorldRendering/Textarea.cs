using MHGameWork.TheWizards.Data;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    [ModelObjectChanged]
    public class Textarea : EngineModelObject
    {

        public string FontFamily { get; set; }
        /// <summary>
        /// Font size in em
        /// </summary>
        public float FontSize { get; set; }
        public string Text { get; set; }
        public Color4 Color { get; set; }
        public Vector2 Position { get; set; }
        /// <summary>
        /// Only Integer support for now
        /// </summary>
        public Vector2 Size { get; set; }
        public bool Visible { get; set; }
        public Color4 BackgroundColor { get; set; }


        public Textarea()
        {
            Visible = true;
            Size = new Vector2(100, 50);
            Position = new Vector2();
            FontFamily = "Verdana";
            FontSize = 10;
            Color = new Color4(0, 0, 0);
            Text = "The Wizards";
            BackgroundColor = new Color4(0,0,0,0);

        }


            

    }
}
