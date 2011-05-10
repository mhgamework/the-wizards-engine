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


        private IEntity holdingCrystal;

        public void Init(IEntityHandle _handle)
        {
            handle = _handle;
            handle.RegisterUpdateHandler();

            psc = handle.GetSceneComponent<PlayerSceneComponent>();

            player = new SimplePlayer();

            controller = psc.CreateController(player.GetData());
            cam = psc.EnablePlayerCamera(player, controller);

        }

        public void Destroy()
        {
        }

        public void Update()
        {
            if (holdingCrystal != null)
            {
                var normal = Vector3.TransformNormal(Vector3.Forward, cam.ViewInverse);
                holdingCrystal.Position = player.GetData().Position + normal*2;
            }
            if (handle.IsKeyDown(Keys.Z))
            {
                controller.DoMoveForward(handle.Elapsed);
            }
            if (handle.IsKeyDown(Keys.S))
            {
                controller.DoMoveBackwards(handle.Elapsed);
            }
            if (handle.IsKeyDown(Keys.Q))
            {
                controller.DoStrafeLeft(handle.Elapsed);
            }
            if (handle.IsKeyDown(Keys.D))
            {
                controller.DoStrafeRight(handle.Elapsed);
            }
            if (handle.IsKeyPressed(Keys.Space))
            {
                controller.DoJump();
            }

            if (handle.IsKeyPressed(Keys.E))
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
                var ray = new Ray(player.GetData().Position, normal);
                var ent = handle.RaycastScene(ray, o => o.Entity.GetAttachedScript<SpawnCrystal>() != null);

                if (ent.IsHit)
                {
                    holdingCrystal = ent.Entity;
                    holdingCrystal.Kinematic = true;
                    //holdingCrystal.Static = false;
                    //psc.RaiseUseEvent(ent.Entity, player);


                }
            }
        }
    }
}
