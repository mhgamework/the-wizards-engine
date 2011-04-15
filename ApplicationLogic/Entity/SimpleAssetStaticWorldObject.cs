using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.World.Static;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Entity
{
    public class SimpleAssetStaticWorldObject : IStaticWorldObject
    {
        private readonly MeshRenderer renderer;
        private MeshRenderElement renderElement;

        public SimpleAssetStaticWorldObject(MeshRenderer renderer)
        {
            this.renderer = renderer;
        }

        public bool Change
        { get; set; }

        public int ID
        { get; set; }

        private Matrix worldMatrix;
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value;
                if (renderElement != null)
                    renderElement.WorldMatrix = worldMatrix;
            }
        }

        private ClientMeshAsset mesh;
        public ClientMeshAsset Mesh
        {
            get { return mesh; }
            set { mesh = value; mesh.Asset.ChangePriority(AssetSynchronizationPriority.Normal); }
        }

        private MeshCoreData coreData;

        public void Update()
        {
            if (Mesh == null) return;
            if (renderElement != null) return;
            if (!Mesh.Asset.IsAvailable) return;

            if (coreData == null)
            {
                coreData = mesh.GetCoreData();
                for (int i = 0; i < coreData.Parts.Count; i++)
                {
                    var part = coreData.Parts[i];
                    var meshPartAsset = part.MeshPart as ClientMeshPartAsset;
                    meshPartAsset.Asset.ChangePriority(AssetSynchronizationPriority.Normal);
                    var tex = part.MeshMaterial.DiffuseMap as ClientTextureAsset;
                    if (tex != null)
                        tex.ClientAsset.ChangePriority(AssetSynchronizationPriority.Normal);


                }
            }

            for (int i = 0; i < coreData.Parts.Count; i++)
            {
                if (!(coreData.Parts[i].MeshPart as ClientMeshPartAsset).Asset.IsAvailable)
                    return;
                 if (coreData.Parts[i].MeshMaterial.DiffuseMap != null)
                    if (!(coreData.Parts[i].MeshMaterial.DiffuseMap as ClientTextureAsset).ClientAsset.IsAvailable)
                        return;
            }


            renderElement = renderer.AddMesh(Mesh);
            renderElement.WorldMatrix = WorldMatrix;
        }


        public void Delete()
        {
            if (renderElement != null)
                renderElement.WorldMatrix = new Matrix();

            renderElement = null;
        }

    }
}
