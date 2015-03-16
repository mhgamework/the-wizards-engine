using Rhino.Mocks.Constraints;

namespace MHGameWork.TheWizards.LogicRTS.Framework
{
    public interface IGameComponent
    {
        GameObject GameObject { get; }
        void Destroy();
        bool IsDestroyed { get; }
    }
}