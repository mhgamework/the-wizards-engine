using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// A cart!!
    /// </summary>
    [ModelObjectChanged]
    public class Cart : EngineModelObject, IPhysical, ICommandHolder, IItemStorage
    {
        public Cart()
        {
            Physical = new Physical();
            CommandHolder = new CommandHolderPart();
            CommandHolder.Holder = this;

            ItemStorage = new ItemStoragePart();
            ItemStorage.Parent = this;

            
        }

        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            Physical.Mesh = TW.Assets.LoadMesh("RTS\\WheelCart\\WheelCart");
            Physical.ObjectMatrix = Matrix.Scaling(0.2f, 0.2f, 0.2f) * Matrix.RotationY(MathHelper.Pi);


            CommandHolder.HoldingArea = new HoldingAreaDescription()
            {
                RelativeStart = new Vector3(-0.4f, 0.5f, 1.6f),
                Direction = new Vector3(1, 0, 0)
            };


            ItemStorage.ContainerArea = new BoundingBox(new Vector3(-0.25f, 0.9f, -0.5f), new Vector3(0.35f, 1f, 0.9f));
            ItemStorage.Capacity = 8;
        }

        public CommandHolderPart CommandHolder { get; set; }
        public ItemStoragePart ItemStorage { get; set; }
    }
}