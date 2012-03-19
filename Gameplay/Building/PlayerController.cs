using DirectX11;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Building
{
    public class PlayerController
    {
        private readonly DX11Game game;
        private readonly BlockFactory blockFactory;
        private Vector3 previousPos;
        private float jumpTimeLeft;

        public PlayerController(DX11Game game, BlockFactory blockFactory)
        {
            this.game = game;
            this.blockFactory = blockFactory;
        }

        public void Update()
        {

            var gravity = 3;
            var offset = game.SpectaterCamera.CameraPosition - previousPos;
            offset.Y = 0;
            offset *= 0.33f;
            offset += Vector3.UnitY * game.Elapsed * -gravity;

            if (game.Keyboard.IsKeyDown(Key.Space) && jumpTimeLeft <= 0 && checkCollision(previousPos + Vector3.UnitY * -0.1f))
                jumpTimeLeft = 0.5f;

            if (jumpTimeLeft > 0)
            {
                offset += Vector3.UnitY * game.Elapsed * (gravity + 4 * 1.8f) * (jumpTimeLeft / 0.5f);
                jumpTimeLeft -= game.Elapsed;
            }


#pragma warning disable 642
            if (tryMove(previousPos + offset)) ;
            else if (tryMove(previousPos + Vector3.UnitX * offset.X + Vector3.UnitZ * offset.Z)) ;
            else if (tryMove(previousPos + Vector3.UnitX * offset.X)) ;
            else if (tryMove(previousPos + Vector3.UnitX * offset.X + Vector3.UnitY * offset.Y)) ;
            else if (tryMove(previousPos + Vector3.UnitY * offset.Y)) ;
            else if (tryMove(previousPos + Vector3.UnitZ * offset.Z + Vector3.UnitY * offset.Y)) ;
            else if (tryMove(previousPos + Vector3.UnitZ * offset.Z)) ;
#pragma warning restore 642

            if (game.SpectaterCamera.CameraPosition.Y < 1.4)
                game.SpectaterCamera.CameraPosition -= Vector3.UnitY * (game.SpectaterCamera.CameraPosition.Y - 1.4f);

            previousPos = game.SpectaterCamera.CameraPosition;
            game.SpectaterCamera.UpdateCameraInfo();
        }

        private bool tryMove(Vector3 pos)
        {
            if (checkCollision(pos)) return false;

            game.SpectaterCamera.CameraPosition = pos;
            return true;
        }

        private bool checkCollision(Vector3 pos)
        {
            if (pos.Y < 1.4) return true;
            for (int i = 0; i < blockFactory.BlockList.Count; i++)
            {
                var blockBB = new BoundingBox(blockFactory.BlockList[i].Position, blockFactory.BlockList[i].Position + new Vector3(1, 1, 1));
                var playerSphere1 = new BoundingSphere(pos, 0.4f);
                var playerSphere2 = new BoundingSphere(pos - Vector3.UnitY * 1, 0.4f);
                var playerSphere3 = new BoundingSphere(pos - Vector3.UnitY * 0.5f, 0.4f);

                if (blockBB.xna().Intersects(playerSphere1.xna()) || blockBB.xna().Intersects(playerSphere2.xna()) || blockBB.xna().Intersects(playerSphere3.xna()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
