using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    public interface IWorldResource
    {
        Vector3 GenerationPoint { get; }
        Vector3 OutputPoint { get; }
        void TakeResource();
        bool ResourceAvailable { get; }
    }
}
