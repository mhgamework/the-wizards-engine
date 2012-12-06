using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Raytracing;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.CG
{
    public class CGFactory
    {
        private GraphicalRayTracer ui;
        private ITraceableScene scene;
        private IRenderedImage image;
        private ICamera cam;

        public GraphicalRayTracer CreateUI()
        {
            ui = new GraphicalRayTracer();
            return ui;
        }

        public PhongShader CreatePhong()
        {
            return new PhongShader(GetScene(), GetCamera());
        }

        public PerspectiveCamera CreatePerspectiveCamera(Vector3 position, Vector3 lookAt)
        {
            var ret = new PerspectiveCamera();

            if (cam != null)
            {
                if (cam is PerspectiveCamera)
                {
                    ret = (PerspectiveCamera)cam;
                }
                else
                    throw new InvalidOperationException();
            }
            ret.Position = position;
            ret.Direction = Vector3.Normalize(lookAt - position);
            cam = ret;
            return ret;
        }


        public ICamera GetCamera()
        {
            if (cam == null) CreatePerspectiveCamera(new Vector3(0, 5, 20), new Vector3());
            return cam;
        }


        public void Run(int numThreads = 4)
        {
            if (ui == null) CreateUI();
            ui.Run(GetRenderedImage(),numThreads);
            
        }

        public IRenderedImage GetRenderedImage()
        {
            if (image == null)
                image = new TracedSceneImage(GetScene(), GetCamera());
            return image;
        }

        public ITraceableScene GetScene()
        {
            if (scene == null) CreateGenericTraceableScene();

            return scene;
        }

        public GenericTraceableScene CreateGenericTraceableScene()
        {
            var ret = new GenericTraceableScene();
            scene = ret;

            return ret;
        }
    }
}
