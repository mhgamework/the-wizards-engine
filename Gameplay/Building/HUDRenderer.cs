using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;

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

            Matrix projectionMatrix = Matrix.Translation(MathHelper.Up * -0.5f)* Matrix.Scaling(new Vector3(0.3f, 0.3f, 0.3f))
                                        * Matrix.RotationZ(rotationLoop * 1.1f + 45) * Matrix.RotationX(2) * Matrix.RotationY(rotationLoop)
                                        * Matrix.Scaling(new Vector3(3 / 4f, 1, 1))
                                        * Matrix.Translation(new Vector3(0.6f, -0.6f, 0.6f));
            meshElement.WorldMatrix = projectionMatrix * Matrix.Invert(game.SpectaterCamera.ViewProjection);


        }

        public void SetBlockType(BlockType type)
        {
            if (type != null && meshElement != null)
            {
                if (meshElement.Mesh == type.Mesh) return;
            }
            if(meshElement != null) meshElement.Delete();

            meshElement = null;
            if (type != null)
                meshElement = renderer.CreateMeshElement(type.Mesh);
        }
    }
}
