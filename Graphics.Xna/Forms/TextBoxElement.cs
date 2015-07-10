using System;
using System.Windows;
using System.Windows.Controls;
using MHGameWork.TheWizards.Reflection;

namespace MHGameWork.TheWizards.Graphics.Xna.Forms
{
    public class TextBoxElement<TU> : IFormElement
    {
        private readonly TextBox box;
        private readonly IAttribute attribute;
        private readonly Func<TU, string> toString;
        private readonly Func<string, TU> fromString;
        private bool Changed { get; set; }
        public UIElement UIElement { get { return box; } }


        public TextBoxElement(TextBox box, IAttribute attribute, Func<TU, string> toString, Func<string, TU> fromString)
        {
            this.box = box;
            this.attribute = attribute;
            this.toString = toString;
            this.fromString = fromString;
            box.TextChanged += delegate { Changed = true; };

        }

        public void WriteDataContext(object context)
        {
            if (!Changed) return;
            Changed = false;

            attribute.SetData(context, fromString(box.Text));
        }

        public void ReadDataContext(object context)
        {
            if (box.IsFocused) return;
            box.Text = toString((TU)attribute.GetData(context));
        }
    }
}