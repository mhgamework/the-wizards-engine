using DirectX11;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    /// <summary>
    /// This is the facade class for the Deferred Renderer. This hides the DirectX11 layer.
    /// </summary>
    public class DeferredRenderer
    {
        private readonly DX11Game game;

        public DeferredRenderer(DX11Game game)
        {
            this.game = game;

        }


        public void Draw()
        {
            
        }
    }
}
