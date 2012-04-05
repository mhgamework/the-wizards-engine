namespace MHGameWork.TheWizards._XNA.Scripting.API
{
    public interface IScript
    {
        void Init(IEntityHandle handle);
        void Destroy();
    }
}
