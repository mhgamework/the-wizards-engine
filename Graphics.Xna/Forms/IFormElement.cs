using System.Windows;

namespace MHGameWork.TheWizards.Graphics.Xna.Forms
{
    public interface IFormElement
    {
        UIElement UIElement { get; }
        void WriteDataContext(object context);
        void ReadDataContext(object context);


    }
}