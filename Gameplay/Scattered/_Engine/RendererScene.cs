namespace MHGameWork.TheWizards.Scattered._Engine
{
    /// <summary>
    /// Responsbile for implementing the renderer facade. A scene consists of a set of objects that are rendered.
    /// This is a concept/stub implementation for now and is to be used together with the old functionality.
    /// TODO: use only low level features (no engine features), or at least redesign dependencies
    /// </summary>
    public class RendererScene
    {
         public TextRectangle CreateTextRectangle()
         {
             return new TextRectangle();
         }
    }
}