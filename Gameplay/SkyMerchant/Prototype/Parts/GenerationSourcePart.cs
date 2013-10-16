using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class GenerationSourcePart : EngineModelObject,IGameObjectComponent
    {
        public GenerationPart GenerationPart { get; private set; }
        public IPositionComponent Physical { get; private set; }
        public IMeshRenderComponent MeshRenderComponent { get; private set; }
      

        public GenerationSourcePart(GenerationPart generationPart, IPositionComponent physical, IMeshRenderComponent meshRenderComponent)
        {
            GenerationPart = generationPart;
            Physical = physical;
            MeshRenderComponent = meshRenderComponent;
        }


        public IMesh EmptyMesh { get; set; }
        public IMesh FullMesh { get; set; }

        public void SimulateGeneration()
        {
            GenerationPart.SimulateResourceGeneration();

            MeshRenderComponent.Mesh = GenerationPart.HasResource ? FullMesh : EmptyMesh;


        }
    }
}