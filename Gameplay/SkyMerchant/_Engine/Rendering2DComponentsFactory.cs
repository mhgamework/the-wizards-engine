using MHGameWork.TheWizards.Engine.WorldRendering;

namespace MHGameWork.TheWizards.SkyMerchant._Engine
{
    /// <summary>
    /// Responsible for creating engine components for 2D rendering
    /// </summary>
    public class Rendering2DComponentsFactory
    {
         public Textarea CreateTextArea()
         {
             return new Textarea();
         }
    }
}