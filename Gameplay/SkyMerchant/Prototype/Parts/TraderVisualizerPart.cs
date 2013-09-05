using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    /// <summary>
    /// Responsible for visualizing a traderpart
    /// </summary>
    [ModelObjectChanged]
    public class TraderVisualizerPart : EngineModelObject, IPhysical
    {
        public TraderPart TraderPart { get; set; }
        public Physical Physical { get; set; }


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