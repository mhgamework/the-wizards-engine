using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
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


        public override void Tick(IVoxelHandle handle)
        {
            var nextInfuse = handle.Data.DataValue / 1000;

            if (nextInfuse > handle.TotalTime) return;
            handle.Data.DataValue = (int)((handle.TotalTime + 1) * 1000);

            foreach (var v in handle.GetRange(5))
            {
                v.Data.MagicLevel = 3;
                if (v.Type == Infestation)
                    v.ChangeType(Land);

            }

        }



    }
}