using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// A crate that can store items!
    /// </summary>
    [ModelObjectChanged]
    public class StorageCrate : EngineModelObject, IPhysical, ICommandHolder, IItemStorage
    {
        public StorageCrate()
        {
            Physical = new Physical();
            CommandHolder = new CommandHolderPart();
            ItemStorage = new ItemStoragePart();

            CommandHolder.Holder = this;
            ItemStorage.Parent = this;





        }

        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            Physical.Mesh = TW.Assets.LoadMesh("RTS\\Crate\\Crate_Coverless");
            Physical.ObjectMatrix = Matrix.RotationY(MathHelper.Pi);

            //TODO: this should be invariant?
            ItemStorage.ContainerArea = new BoundingBox(new Vector3(-0.5f, 0.3f, -0.5f), new Vector3(0.8f, 1.5f, 0.8f));
            ItemStorage.Capacity = 27;
            CommandHolder.HoldingArea = new HoldingAreaDescription(){RelativeStart = new Vector3(-0.5f,0.5f,-1.1f), Direction = new Vector3(1,0,0) };
        }

        public CommandHolderPart CommandHolder { get; set; }
        public ItemStoragePart ItemStorage { get; set; }


    }
}