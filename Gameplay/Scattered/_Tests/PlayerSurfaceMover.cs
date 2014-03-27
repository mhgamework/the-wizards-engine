using System;
using DirectX11;
using ProceduralBuilder.Shapes;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    /// <summary>
    /// Allows walking on any surface which is raycastable
    /// </summary>
    public class PlayerSurfaceMover
    {
        private readonly Func<Ray, float?> raycastSurface;
        private Ray downRay;
        public float WalkHeight { get; private set; }

        public PlayerSurfaceMover(Func<Ray, float?> raycastSurface)
        {
            this.raycastSurface = raycastSurface;
            WalkHeight = 1.8f;
        }

        public Vector3 ProcessUserMovement(Vector3 pos)
        {
            var trans = getUserMovementInputAsDir();
            trans.Normalize();
            trans = Vector3.TransformNormal(trans, TW.Graphics.Camera.ViewInverse);
            trans.Y = 0;
            trans.Normalize();
            //TW.Graphics.LineManager3D.AddLine(TW.Graphics.Camera.ViewInverse.GetTranslation(), TW.Graphics.Camera.ViewInverse.GetTranslation() + trans * 10, new Color4(1, 0, 0));
            var newPos = pos + trans * TW.Graphics.Elapsed * 8;

            if (!isAboveIsland(newPos))
            {
                if (isAboveIsland(pos)) newPos = pos;
            }
            else
            {
                downRay = new Ray(newPos, -Vector3.UnitY);
                var dist = raycastSurface(downRay).Value; //Note: should have a value
                newPos.Y = downRay.GetPoint(dist).Y + WalkHeight;
            }
            return newPos;
        }

        private Vector3 getUserMovementInputAsDir()
        {
            var keyboard = TW.Graphics.Keyboard;
            Vector3 ret = new Vector3();

            if (keyboard.IsKeyDown(Key.S)) { ret += MathHelper.Backward; }
            if (keyboard.IsKeyDown(Key.W)) { ret += MathHelper.Forward; }
            if (keyboard.IsKeyDown(Key.A)) { ret += MathHelper.Left; }
            if (keyboard.IsKeyDown(Key.D)) { ret += MathHelper.Right; }
            //if (keyboard.IsKeyDown(Key.Space)) { ret += MathHelper.Up; }
            //if (keyboard.IsKeyDown(Key.LeftControl)) { ret += MathHelper.Down; }
            return ret;
        }

        private bool isAboveIsland(Vector3 pos)
        {
            downRay = new Ray(pos, -Vector3.UnitY);
            var dist = raycastSurface(downRay);
            return dist.HasValue;
        }



    }
}