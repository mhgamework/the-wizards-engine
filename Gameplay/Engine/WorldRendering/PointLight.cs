using MHGameWork.TheWizards.Data;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    [ModelObjectChanged]
    public class PointLight : EngineModelObject
    {
        public float Intensity { get; set; }

        public PointLight()
        {
            Enabled = true;
            Size = 10;
            Intensity = 1;
        }

        public Vector3 Position { get; set; }
        public float Size { get; set; }
        public bool Enabled { get; set; }

    }
}
