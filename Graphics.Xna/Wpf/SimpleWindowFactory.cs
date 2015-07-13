using System.Windows;

namespace MHGameWork.TheWizards.Graphics.Xna.Wpf
{
    /// <summary>
    /// Simply creates a new window in this thread, using application.Current
    /// </summary>
    public class SimpleWindowFactory : IWindowFactory
    {
        public Window CreateWindow()
        {
            return new Window();
        }
    }
}
