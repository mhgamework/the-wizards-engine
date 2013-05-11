using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1._Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// A cart!!
    /// </summary>
    [ModelObjectChanged]
    public class Cart : EngineModelObject, IPhysical, ICommandHolder
    {
        public Cart()
        {
            Physical = new Physical();
            CommandHolder = new CommandHolderPart();
            CommandHolder.Holder = this;

            CommandHolder.HoldingArea = new HoldingAreaDescription()
            {
                RelativeStart =  new Vector3(-0.4f,0.5f,1.6f),
                Direction = new Vector3(1,0,0)
            };
        }

        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            Physical.Mesh = TW.Assets.LoadMesh("RTS\\WheelCart\\WheelCart");
            Physical.ObjectMatrix = Matrix.Scaling(0.2f, 0.2f, 0.2f) * Matrix.RotationY(MathHelper.Pi);
        }

        public CommandHolderPart CommandHolder { get; set; }
    }
}