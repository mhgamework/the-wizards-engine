using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.GameObjects
{
    /// <summary>
    /// Pushes changes in position to the decorated position components
    /// TODO: this is a problem, and should be removed
    /// See <see cref="RelativePositionComponent"/>.UpdateDecoratedComponent for more info
    /// </summary>
    public class RelativePositioningSimulator : ISimulator
    {
        private readonly IGameObjectsRepository repository;

        public RelativePositioningSimulator(IGameObjectsRepository repository)
        {
            this.repository = repository;
        }

        public void Simulate()
        {
            foreach (var comp in repository.GetAllComponents<IRelativePositionComponent>()) // Requires using the interface, because the component is bound to this
            {
                ((RelativePositionComponent)comp).UpdateDecoratedComponent();
            }
        }
    }
}