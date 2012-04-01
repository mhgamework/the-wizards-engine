using System;
using System.Windows;
using System.Windows.Controls;
using Dotway.WPF.Controls;
using Dotway.WPF.Controls.Utilities;
using MHGameWork.TheWizards.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Forms
{
    public class ColorElement : IFormElement
    {
        private readonly IAttribute attribute;
        private ColorPicker picker;
        private Expander expander;
        private bool Changed { get; set; }
        public UIElement UIElement { get { return expander; } }


        public ColorElement(IAttribute attribute)
        {
            this.attribute = attribute;

            picker = new ColorPicker();
            expander = new Expander();
            expander.Content = picker;
        }

        public void WriteDataContext(object context)
        {
            var c = picker.SelectedColor;

            attribute.SetData(context, toColor(picker.SelectedColor));
        }

        private Color toColor(NotifiedColor c)
        {
            return new Color(c.R, c.G, c.B, c.A);
        }

        public void ReadDataContext(object context)
        {
            var c = (Color)attribute.GetData(context);

            expander.Header = c;
            picker.InitialColor = toMediaColor(c);

            if (c.Equals(toColor(picker.SelectedColor))) return;


            var notifiedColor = picker.SelectedColor;
            setNotifiedColor(c, notifiedColor);
        }

        private void setNotifiedColor(Color c, NotifiedColor notifiedColor)
        {
            notifiedColor.R = c.R;
            notifiedColor.G = c.G;
            notifiedColor.B = c.B;
            notifiedColor.A = c.A;
        }

        private System.Windows.Media.Color toMediaColor(Color c)
        {
            var ret = new System.Windows.Media.Color();
            ret.R = c.R;
            ret.G = c.G;
            ret.B = c.B;
            ret.A = c.A;
            return ret;
        }
    }
}