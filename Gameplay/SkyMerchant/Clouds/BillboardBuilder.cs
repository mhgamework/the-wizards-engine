using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Clouds
{
    /// <summary>
    /// Creates quad meshes and can create billboard matrices
    /// </summary>
    public class BillboardBuilder
    {
         public IMesh CreatePanel(ITexture tex)
         {
             var positions = new[]
                 {
                     new Vector3(-1,-1,0),
                     new Vector3(-1,1,0),
                     new Vector3(1,1,0),
                     new Vector3(1,-1,0)
                 };
             var normals = new[]
                 {
                     Vector3.UnitZ,
                     Vector3.UnitZ,
                     Vector3.UnitZ,
                     Vector3.UnitZ
                 };
             var texcoords = new[]
                 {
                     new Vector2(0,0),
                     new Vector2(0,1),
                     new Vector2(1,1),
                     new Vector2(1,0)
                 };
             var builder = new MeshBuilder();
             builder.AddCustom(positions,normals,texcoords);
             builder.DiffuseTexture = tex;

             return builder.CreateMesh();

         }
    }
}