namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Script that can be attached to an object.
    /// </summary>
    public interface IWorldScript
    {
        void Initialize(IWorldObject obj);
        void Update();

    }
}