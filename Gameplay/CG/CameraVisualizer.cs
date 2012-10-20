using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerGraphics;
using ComputerGraphics.Math;
using MHGameWork.TheWizards.DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.CG
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
                    game.LineManager3D.AddRay(cam.CalculateRay(new Point2(x, y)), new Color4(1, 0, 0));
                }    
        }

        
    }
}
