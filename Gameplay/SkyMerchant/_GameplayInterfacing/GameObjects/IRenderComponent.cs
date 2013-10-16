namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects
{
    /// <summary>
    /// Responsible for rendering a game object
    /// WARNING: has no responsibilities?
    /// </summary>
    public interface IRenderComponent:IGameObjectComponent
    {
        // Note: this method is meant for the engine, not for the user of the gameplay interfaces, so this must be removed??
        //void UpdateRenderState();
    }
}