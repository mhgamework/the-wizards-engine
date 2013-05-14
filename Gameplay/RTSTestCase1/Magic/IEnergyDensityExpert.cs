using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    //Responsible for providing the density at a locationKnows the density of crystals
    public interface IEnergyDensityExpert
    {
        float GetDensity(Vector3 position);
    }
}