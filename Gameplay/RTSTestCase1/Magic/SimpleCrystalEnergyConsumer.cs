using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    [ModelObjectChanged]
    public class SimpleCrystalEnergyConsumer : EngineModelObject,IEnergyConsumer
    {
        private SimpleCrystal consumedCrystal;

        public SimpleCrystalEnergyConsumer()
        {
            consumedCrystal = new SimpleCrystal() { Capacity = 60,Visible = false};
            
        }
        public void ConsumeEnergy(float elapsedTime)
        {
            if (consumedCrystal.Energy - elapsedTime < 0)
            { Active = false; }
            if (consumedCrystal.Energy > 4)
            { Active = true;}
            if (Active)
            { consumedCrystal.Energy -= elapsedTime; }
        }

        public bool Active { get; set; }
        public Vector3 Position
        {
            get { return consumedCrystal.Position; }
            set { consumedCrystal.Position = value; }
        }
    }
}