using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay
{
    /// <summary>
    /// Represents a bridge between 2 islands
    /// </summary>
    public class BridgePart
    {
        private readonly Physical ph;
        private readonly BridgeMeshBuilder meshBuilder;

        public BridgePart(Physical ph, BridgeMeshBuilder meshBuilder)
        {
            this.ph = ph;
            this.meshBuilder = meshBuilder;
        }

        public IslandPart IslandA { get; set; }
        public IslandPart IslandB { get; set; }

        public void UpdatePhysical()
        {
            if (ph.Mesh == null)
            {
                var mesh = meshBuilder.BuildMeshForEndpoint(IslandB.Physical.GetPosition() - IslandA.Physical.GetPosition());
                ph.Mesh = mesh;
            }
            ph.WorldMatrix =
                meshBuilder.GetMatrixForEndpoint(IslandB.Physical.GetPosition() - IslandA.Physical.GetPosition());
        }
    }
}