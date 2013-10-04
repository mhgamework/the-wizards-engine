using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay
{
    /// <summary>
    /// Represents a bridge between 2 islands
    /// </summary>
    [ModelObjectChanged]
    public class BridgePart:EngineModelObject
    {
        private readonly Physical ph;
        private readonly BridgeMeshBuilder meshBuilder;

        public BridgePart(Physical ph, BridgeMeshBuilder meshBuilder)
        {
            this.ph = ph;
            this.meshBuilder = meshBuilder;
        }

        public BridgeAnchor AnchorA { get; set; }
        public BridgeAnchor AnchorB { get; set; }

        public void UpdatePhysical()
        {
            if (ph.Mesh == null)
            {
                var mesh = meshBuilder.BuildMeshForEndpoint(GetAnchorB() - GetAnchorA());
                ph.Mesh = mesh;
            }
            ph.WorldMatrix =
                meshBuilder.GetMatrixForEndpoint(GetAnchorB() - GetAnchorA())
                * Matrix.Translation(GetAnchorA());
        }

        private Vector3 GetAnchorB()
        {
            return AnchorB.GetPosition();
        }

        private Vector3 GetAnchorA()
        {
            return AnchorA.GetPosition();
        }

        public struct BridgeAnchor
        {
            public IWorldObject Island { get; set; }
            public Vector3 RelativePosition { get; set; }

            public bool IsEmpty
            {
                get { return Island == null; }
            }

            public Vector3 GetPosition()
            {
                return Island.Position + RelativePosition;
            }
        }
    }
}