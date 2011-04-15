using System.Collections.Generic;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.World.Static;

namespace MHGameWork.TheWizards.Entity
{
    public class SimpleAssetStaticWorldObjectFactory : IStaticWorldObjectFactory
    {
        private readonly MeshRenderer renderer;
        private ClientRenderingAssetFactory renderingFactory;

        private List<SimpleAssetStaticWorldObject> assets = new List<SimpleAssetStaticWorldObject>();

        public SimpleAssetStaticWorldObjectFactory(MeshRenderer renderer, ClientAssetSyncer syncer)
        {
            this.renderer = renderer;
            renderingFactory = new ClientRenderingAssetFactory(syncer);
        }


        public void ApplyUpdatePacket(IStaticWorldObject obj, StaticWorldObjectUpdatePacket p)
        {
            var o = obj as SimpleAssetStaticWorldObject;
            o.WorldMatrix = XnaMathExtensions.CreateMatrixFromFloatArray(p.WorldMatrix, 0);
            o.Mesh = renderingFactory.GetMesh(p.MeshGuid);
        }

        public IStaticWorldObject CreateNew()
        {
            var obj = new SimpleAssetStaticWorldObject(renderer);
            assets.Add(obj);
            return obj;
        }

        public void Delete(IStaticWorldObject obj)
        {
            ((SimpleAssetStaticWorldObject)obj).Delete();
        }

        public void Update()
        {
            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].Update();
            }
        }
    }
}
