namespace MHGameWork.TheWizards.Rendering
{
    public interface ISimpleMeshRenderer
    {
        SimpleMeshRenderElement AddMesh(IMesh mesh);
        void DeleteMesh(SimpleMeshRenderElement el);
    }
}