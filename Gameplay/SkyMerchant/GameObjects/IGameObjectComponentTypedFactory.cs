using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.GameObjects
{
    public interface IGameObjectComponentTypedFactory
    {
        T CreateComponent<T>() where T : IGameObjectComponent;
    }
}