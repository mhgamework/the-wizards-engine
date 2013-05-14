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

        public static float GetRadius()
        {
            return 0.2f;
        }

        public IMesh CreateMesh()
        {
            var builder = new MeshBuilder();
            builder.AddBox(MathHelper.One * -GetRadius(), MathHelper.One * GetRadius());
            var mesh = builder.CreateMesh();

            if (Type != null)
                mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap = Type.Texture;

            var skin = 0.02f;
            mesh.GetCollisionData().Boxes.Add(new MeshCollisionData.Box() { Dimensions = new Vector3(GetRadius() * 2 + skin, GetRadius() * 2 + skin, GetRadius() * 2 + skin), Orientation = Matrix.Identity });

            return mesh;
        }
    }
}