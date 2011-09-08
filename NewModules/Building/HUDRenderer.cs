using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    public class HUDRenderer
    {
        private readonly DX11Game game;
        float rotationLoop = 0;
        private DeferredMeshRenderElement meshElement;
        DeferredRenderer renderer;

        public HUDRenderer(DX11Game game, DeferredRenderer renderer)
        {
            this.game = game;
            this.renderer = renderer;
        }


        public void Update()
        {
            if (meshElement == null)
                return;

            if (rotationLoop <= 360)
                rotationLoop += 0.01f;
            else rotationLoop = 0;

            Matrix projectionMatrix = Matrix.Scaling(new Vector3(0.3f, 0.3f, 0.3f))
                                        * Matrix.RotationZ(rotationLoop * 1.1f + 45) * Matrix.RotationX(2) * Matrix.RotationY(rotationLoop)
                                        * Matrix.Scaling(new Vector3(3 / 4f, 1, 1))
                                        * Matrix.Translation(new Vector3(0.6f, -0.6f, 0.6f));
            meshElement.WorldMatrix = projectionMatrix * Matrix.Invert(game.SpectaterCamera.ViewProjection);


        }

        public void SetMesh(IMesh mesh)
        {
            if (meshElement != null && meshElement.Mesh == mesh) return;
            if (meshElement != null)
                meshElement.Delete();
            meshElement = null;

            if (mesh != null)
                meshElement = renderer.CreateMeshElement(mesh);
        }
    }
}
