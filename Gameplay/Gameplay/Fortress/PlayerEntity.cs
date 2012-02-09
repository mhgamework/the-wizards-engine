using System;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Scripting.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlayerController = MHGameWork.TheWizards.GamePlay.PlayerController;

namespace MHGameWork.TheWizards.Gameplay.Fortress
{
    /// <summary>
    /// This represents the Current Player, not any player in the fortress map
    /// </summary>
    public class PlayerEntity : IScript, IUpdateHandler
    {
        private PlayerSceneComponent psc;
        private PlayerController controller;
        private IEntityHandle handle;

        private SimplePlayer player;
        private PlayerThirdPersonCamera cam;

        private FireMove move;

        private IEntity holdingCrystal;

        public IEntityHandle Handle
        {
            get { return handle; }
            private set { handle = value; }
        }

        public void Init(IEntityHandle _handle)
        {
            Handle = _handle;
            Handle.RegisterUpdateHandler();

            psc = Handle.GetSceneComponent<PlayerSceneComponent>();

            player = new SimplePlayer();

            controller = psc.CreateController(player.GetData());
            cam = psc.EnablePlayerCamera(player, controller);

            move = new FireMove(this, controller);

        }

        public void Destroy()
        {
        }

        public void Update()
        {
            move.FireDirection = CalculateFireDirection(cam);
            if (handle.Input.IsLeftButtonPressed())
            {
                move.StartPrimaryAttack();
            }
            if (handle.Input.IsRightButtonPressed())
            {
                move.StartSecondaryAttack();
            }
            if (handle.Input.IsRightButtonPressed())
            {
                move.EndPrimaryAttack();
            }
            if (handle.Input.IsRightButtonReleased())
            {
                move.EndSecondaryAttack();
            }

            if (holdingCrystal != null)
            {
                var normal = Vector3.TransformNormal(Vector3.Forward, cam.ViewInverse);
                holdingCrystal.Position = player.GetData().Position.xna() + normal*2;
            }
            if (Handle.Input.IsKeyDown(Keys.Z))
            {
                controller.DoMoveForward(Handle.Elapsed);
            }
            if (Handle.Input.IsKeyDown(Keys.S))
            {
                controller.DoMoveBackwards(Handle.Elapsed);
            }
            if (Handle.Input.IsKeyDown(Keys.Q))
            {
                controller.DoStrafeLeft(Handle.Elapsed);
            }
            if (Handle.Input.IsKeyDown(Keys.D))
            {
                controller.DoStrafeRight(Handle.Elapsed);
            }
            if (Handle.Input.IsKeyPressed(Keys.Space))
            {
                controller.DoJump();
            }

            if (Handle.Input.IsKeyPressed(Keys.E))
            {
                Console.WriteLine("Pressed!");
                if (holdingCrystal != null)
                {
                    holdingCrystal.Kinematic = false;
                    //holdingCrystal.Static = true;
                    holdingCrystal = null;
                    return;

                }
               

                var normal = Vector3.TransformNormal(Vector3.Forward, cam.ViewInverse);
                var ray = new Ray(player.GetData().Position.xna(), normal);
                var ent = Handle.RaycastScene(ray, o => o.Entity.GetAttachedScript<SpawnCrystal>() != null);

                if (ent.IsHit)
                {
                    holdingCrystal = ent.Entity;
                    holdingCrystal.Kinematic = true;
                    //holdingCrystal.Static = false;
                    //psc.RaiseUseEvent(ent.Entity, player);


                }
            }
        }



        public static Vector3 CalculateFireDirection(PlayerThirdPersonCamera cam)
        {
            var pos = Vector3.Transform(Vector3.Zero, cam.ViewInverse);
            Vector3 dir = Vector3.Transform(Vector3.Forward, cam.ViewInverse);
            return Vector3.Normalize(dir - pos);
        }

    }
}
