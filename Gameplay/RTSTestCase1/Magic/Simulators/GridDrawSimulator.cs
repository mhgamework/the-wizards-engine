using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators
{
    public class GridDrawSimulator:ISimulator
    {
        
        private IEnergyDensityExpert expert;
        public GridDrawSimulator(IEnergyDensityExpert expert)
        {
            this.expert = expert;
        }

        public void Simulate()
        {
            for (var i = -20; i < 20; i+= 2)
            {
                for (var j = -20; j < 20; j += 2)
                {
                    var position = new Vector3(i*5, 0, j*5);
                    var drawDensity = expert.GetDensity(position)/2000;
                    TW.Graphics.LineManager3D.AddRectangle(position + new Vector3(2.5f, 0, 2.5f), new Vector2(1, 1), new Vector3(1, 0, 0), new Vector3(0, 0, 1), new Color4(drawDensity, drawDensity, drawDensity));
                }
            }
        }
    }
}