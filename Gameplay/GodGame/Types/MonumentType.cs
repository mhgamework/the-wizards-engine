using System.Drawing;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class MonumentType : GameVoxelType
    {
        public MonumentType()
            : base("Monument")
        {
            Color = Color.Green;

        }

        private float nextInfuse;

        public override void Tick(IVoxelHandle handle)
        {
            if (nextInfuse > handle.TotalTime) return;
            nextInfuse = handle.TotalTime + 1;

            foreach (var v in handle.GetRange(5))
            {
                v.MagicLevel = 3;
                if (v.Type == Infestation)
                    v.ChangeType(Land);

            }

        }



    }
}