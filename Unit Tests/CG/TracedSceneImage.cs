using System;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Raytracing;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.CG
{
    /// <summary>
    /// Creates an image from a traceablescene and a camera
    /// </summary>
    internal class TracedSceneImage : IRenderedImage
    {
        private readonly ITraceableScene scene;
        private readonly ICamera camera;

        public TracedSceneImage(ITraceableScene scene, ICamera camera)
        {
            this.scene = scene;
            this.camera = camera;
        }

        public Color4 GetPixel(Vector2 pos)
        {
            var rayTrace = new RayTrace(camera.CalculateRay(pos), 0, Int32.MaxValue);
            IShadeCommand cmd;
            scene.Intersect(rayTrace, out cmd, true);
            if (cmd == null) return new Color4();
            return cmd.CalculateColor();

        }
    }
}