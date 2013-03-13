using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    //Knows the density of crystals, (not looking to the energy crystals have.)
    public interface ICrystalDensityExpert
    {
        float GetDensity(Vector3 position);
    }
}