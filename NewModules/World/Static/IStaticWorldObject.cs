using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.World.Static
{
    public interface IStaticWorldObject
    {
        bool Change { get; set; }
        int ID { get; set; }
        

    }
}
