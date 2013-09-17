using MHGameWork.TheWizards.Engine.Worlding;


namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    public class SimpleWorldDestroyer : IWorldDestroyer
    {
        public void Destroy(IPhysical item)
        {
            TW.Data.RemoveObject(item);
        }
    }
}