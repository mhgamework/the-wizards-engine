using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.RTSTestCase1.Characters;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;
using StillDesign.PhysX;
using Ray = SlimDX.Ray;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    public interface IUserPlayer : IItemStorage, IPhysical, ICartHolder,IPlayerInteraction
    {
    }

    [ModelObjectChanged]
    public class UserPlayer : EngineModelObject, IRTSCharacter, IUserPlayer
    {

        public Entity Used { get; set; }
        public Entity Attacked { get; set; }
        public Vector3 Position { get; set; }

        public string Name { get; set; }
        public Entity Targeted { get; set; }

        public DroppedThing Holding { get; set; }

        public float TargetDistance { get; set; }

        public Vector3 LookDirection { get; set; }

        public Actor GetHoldingActor()
        {
            try
            {
                return Holding.get<Entity>()
                    .get<EntityPhysXUpdater.EntityPhysX>()
                    .getCurrentActor();
            }
            catch (NullReferenceException) { }
            return null;

        }

        public UserPlayer()
        {
            LookDirection = -Vector3.UnitZ;
            ItemStorage = new ItemStoragePart();
            Physical = new Physical();
            ItemStorage.Parent = this;
            CartHolder = new CartHolderPart();
            CartHolder.Parent = this;
            PlayerInteraction = new PlayerInteractionPart();
            PlayerInteraction.Player = this;

        }


        public Vector3 GetHoldingPosition()
        {
            //Matrix viewInverse = TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse;
            //return (viewInverse.xna().Translation + viewInverse.xna().Forward * 3).dx();
            return Position;
        }

        public void DropHolding()
        {
            Holding = null;
        }

        public Ray GetTargetingRay()
        {
            //TODO: support multi-user

            return TW.Data.Get<CameraInfo>().GetCenterScreenRay();
        }

        public ItemStoragePart ItemStorage { get; set; }
        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            var dir = LookDirection;
            dir.Y = 0;
            dir = Vector3.Normalize(-dir);

            Physical.WorldMatrix = Microsoft.Xna.Framework.Matrix.CreateFromQuaternion( Functions.CreateFromLookDir(dir.xna())).dx()
                * Matrix.Translation(Position);
            ItemStorage.Parent = this;
            ItemStorage.Capacity = 1;
            ItemStorage.ContainerArea = new BoundingBox(new Vector3(0f, -0.3f, 0f), new Vector3(2f, 1f, 2f));
            CartHolder.Parent = this;
            PlayerInteraction.Player = this;
        }

        public CartHolderPart CartHolder { get; set; }
        public PlayerInteractionPart PlayerInteraction { get; set; }
    }
}
