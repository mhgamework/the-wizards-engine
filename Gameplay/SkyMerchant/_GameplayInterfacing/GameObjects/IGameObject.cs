namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects
{
    /// <summary>
    /// Responsible for providing access to connected components, which represents a single game object.
    /// </summary>
    public interface IGameObject
    {
        T GetComponent<T>() where T : IGameObjectComponent;
        bool HasComponent<T>();
    }
}