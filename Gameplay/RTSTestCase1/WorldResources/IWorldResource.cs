using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine._Core;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    public interface IWorldResource : IEngineModelObject
    {
        Vector3 GenerationPoint { get; }
        Vector3 OutputDirection  { get; }
        Thing TakeResource();
        bool ResourceAvailable { get; }
        void Grow(float elapsed);
    }
}
