namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects
{
    /// <summary>
    /// Responsible for updating game object scripting logic.
    /// </summary>
    public interface IScriptComponent : IGameObjectComponent
    {
        void Update();
    }
}