using MHGameWork.TheWizards.DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.CG.Visualization
{
    /// <summary>
    /// Responsible for visualizing a ICamera using DX11Game
    /// </summary>
    public class CameraVisualizer
    {
        private readonly DX11Game game;

        public CameraVisualizer(DX11Game game)
        {
            this.game = game;
        }

        public void RenderRays(ICamera cam, Point2 resolution)
        {
            for (int x = 0; x < resolution.X; x++)
                for (int y = 0; y < resolution.Y; y++)
                {
                    game.LineManager3D.AddRay(cam.CalculateRay(new Vector2((x + 0.5f) / resolution.X, (y + 0.5f) / resolution.Y)), new Color4(1, 0, 0));
                }
        }


    }
}
