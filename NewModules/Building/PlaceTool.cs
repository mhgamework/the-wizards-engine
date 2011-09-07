using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    public class PlaceTool
    {
        private readonly DX11Game game;
        DeferredRenderer renderer;
        
        private DeferredMeshRenderElement ghost;
        private Ray ray;
        private Plane p = new Plane(new Vector3(0, 1, 0), 0);

        public PlaceTool(DX11Game game, DeferredRenderer renderer)
        {
            this.game = game;
            this.renderer = renderer;
        }


        public void Update()
        {
            game.GameLoopEvent += delegate
            {
                if (ghost == null)
                    return;


                ghost.WorldMatrix = Matrix.Translation(CalculatePlacePos());


                if (game.Mouse.LeftMousePressed)
                    PlaceBlock();

            };
        }

        private Vector3 CalculatePlacePos()
        {
            var placePos = new Vector3();

            ray.Position = game.SpectaterCamera.CameraPosition;
            ray.Direction = game.SpectaterCamera.CameraDirection;

            var intersects = ray.xna().Intersects(p.xna());
            if (intersects.HasValue)
            {
                placePos = ray.Position + intersects.Value * ray.Direction;
            }

            placePos.Y = 0;

            return placePos;
        }

        private void PlaceBlock()
        {
            throw new NotImplementedException();
        }

        public void SetMesh(IMesh mesh)
        {
            if (ghost != null)
                ghost.Delete();
            ghost = null;

            if (mesh != null)
                ghost = renderer.CreateMeshElement(mesh);
        }


    }
}
