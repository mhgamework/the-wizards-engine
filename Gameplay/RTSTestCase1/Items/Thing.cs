using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;
using MathHelper = DirectX11.MathHelper;

namespace MHGameWork.TheWizards.RTSTestCase1.Items
{
    [ModelObjectChanged]
    public class Thing : EngineModelObject
    {
        public ResourceType Type;

        public IMesh CreateMesh()
        {
            var builder = new MeshBuilder();
            builder.AddBox(MathHelper.One * -0.2f, MathHelper.One * 0.2f);
            var mesh = builder.CreateMesh();

            mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap = Type.Texture;

            mesh.GetCollisionData().Boxes.Add(new MeshCollisionData.Box(){ Dimensions = new Vector3(0.4f,0.4f,0.4f), Orientation = Matrix.Identity});

            return mesh;
        }
    }
}