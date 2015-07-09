using System.Windows;

namespace MHGameWork.TheWizards.Forms
{
    public interface IFormElement
    {
        UIElement UIElement { get; }
        void WriteDataContext(object context);
        void ReadDataContext(object context);


    }
}