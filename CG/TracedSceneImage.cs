using System;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// Creates an image from a traceablescene and a camera
    /// </summary>
    public class TracedSceneImage : IRenderedImage
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
            TraceResult result;
            scene.Intersect(rayTrace, out result);//TODO: shadecommand true);
            return result.Shade(ref rayTrace);

        }
    }
}