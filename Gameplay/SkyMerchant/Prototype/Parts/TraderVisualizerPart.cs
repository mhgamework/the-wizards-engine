using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    /// <summary>
    /// Responsible for visualizing a traderpart
    /// </summary>
    [ModelObjectChanged]
    public class TraderVisualizerPart : EngineModelObject, IGameObjectComponent
    {
        public TraderPart TraderPart { get; set; }
        public IMeshRenderComponent Physical { get; set; }

        public IPositionComponent PositionComponent { get; private set; }

        public TraderVisualizerPart(TraderPart traderPart, IMeshRenderComponent physical, IPositionComponent positionComponent)
        {
            this.PositionComponent = positionComponent;
            TraderPart = traderPart;
            Physical = physical;
        }

        public TraderVisualizerPart()
        {

        }
        public void UpdatePhysical()
        {

        }

        public void FixMesh()
        {
            var text = "Wants: " + TraderPart.WantsAmount + " " + TraderPart.WantsType.Name + "\n"
                       + "You get: " + TraderPart.GivesAmount + " " + TraderPart.GivesType.Name;
            if (TraderPart.StoredResourcesCount < TraderPart.GivesAmount)
                text += "\nOut of resources";

            //text = "hello";


            Physical.Mesh = UtilityMeshes.CreateMeshWithText(1, text, TW.Graphics);
            Physical.ObjectMatrix = Matrix.Translation(new Vector3(0, 1, 0));

        }


    }
}