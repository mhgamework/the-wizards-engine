using DirectX11;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay
{
    /// <summary>
    /// Responsible for building bridge meshes
    /// </summary>
    public class BridgeMeshBuilder
    {
        private readonly AssetsRepository repo;
        private readonly DynamicAssetsFactory factory;
        private IMesh plank;
        private IMesh rope;
        private IMesh stick;
        private float bridgeWidth = 0.5f;

        public BridgeMeshBuilder(AssetsRepository repo, DynamicAssetsFactory factory)
        {
            this.repo = repo;
            this.factory = factory;
            plank = repo.LoadMesh("SkyMerchant\\Bridge\\BridgePartPlank");
            rope = repo.LoadMesh("SkyMerchant\\Bridge\\BridgePartRope");
            stick = repo.LoadMesh("SkyMerchant\\Bridge\\BridgePartStick");
        }

        /// <summary>
        /// Build a bridge with given number planks in the Z direction
        /// Normalized to unit length
        /// </summary>
        /// <param name="numPlanks"></param>
        /// <returns></returns>
        public IMesh BuildMesh(int numPlanks)
        {
            // Currently straight
            var ret = factory.CreateEmptyDynamicMesh();

            var endSticksZ = numPlanks - 0.2f;

            for (int i = 0; i < numPlanks; i++)
            {
                MeshBuilder.AppendMeshTo(plank, ret, Matrix.Translation(0, 0, 0.4f + i * 1f));
            }

            MeshBuilder.AppendMeshTo(stick, ret, Matrix.RotationY(MathHelper.Pi) * Matrix.RotationY(-MathHelper.PiOver2) * Matrix.Translation(-0.9f, 0, 0));
            MeshBuilder.AppendMeshTo(stick, ret, Matrix.RotationY(MathHelper.Pi) * Matrix.Translation(0.9f, 0, 0));

            MeshBuilder.AppendMeshTo(stick, ret, Matrix.RotationY(-MathHelper.PiOver2) * Matrix.Translation(-0.9f, 0, endSticksZ));
            MeshBuilder.AppendMeshTo(stick, ret, Matrix.Translation(0.9f, 0, endSticksZ));

            MeshBuilder.AppendMeshTo(rope, ret, Matrix.Translation(-0.9f, 0, 5) * Matrix.Scaling(1, 1, endSticksZ / 10f));
            MeshBuilder.AppendMeshTo(rope, ret, Matrix.Translation(0.9f, 0, 5) * Matrix.Scaling(1, 1, endSticksZ / 10f));


            foreach (var part in ret.GetCoreData().Parts)
            {
                part.ObjectMatrix = part.ObjectMatrix * Matrix.Scaling(1, 1, 1f / endSticksZ).xna();
            }

            return ret;
        }

        /// <summary>
        /// Builds a bridge mesh with unit length for going from 0,0,0 to the endpoint
        /// Use the GetMatrixForEndpoint to get the matrix to correctly transform the mesh
        /// </summary>
        /// <param name="bridgeEndPoint"></param>
        /// <returns></returns>
        public IMesh BuildMeshForEndpoint(Vector3 endpoint)
        {
            return BuildMesh((int)endpoint.Length());
        }
        /// <summary>
        /// Returns a matrix that transforms a normalized bridge to a bridge from 0,0,0 to the endpoint.
        /// </summary>
        /// <param name="bridgeEndPoint"></param>
        /// <returns></returns>
        public Matrix GetMatrixForEndpoint(Vector3 endpoint)
        {
            return Matrix.RotationY(MathHelper.Pi) *
                Matrix.Scaling(1, 1, endpoint.Length()) *
                 Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(Functions.CreateFromLookDir(Vector3.Normalize(endpoint).xna())).dx();
        }
    }
}