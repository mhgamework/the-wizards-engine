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
            ITexture tex = null;
            if (Type != null)
                tex = Type.Texture;

            return UtilityMeshes.CreateMeshWithTexture(GetRadius(), tex);
        }
    }
}