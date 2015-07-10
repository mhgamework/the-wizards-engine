using System.Windows;
using System.Windows.Controls;
using MHGameWork.TheWizards.Reflection;

namespace MHGameWork.TheWizards.Graphics.Xna.Forms
{
    public class ReadonlyElement : IFormElement
    {
        private readonly Label label;
        private readonly IAttribute attribute;
        public UIElement UIElement { get { return label; } }


        public ReadonlyElement(IAttribute attribute)
        {
            this.attribute = attribute;

            label = new Label();

        }

        public void WriteDataContext(object context)
        {
            return;
        }

        public void ReadDataContext(object context)
        {
            label.Content = attribute.GetData(context);
        }
    }
}