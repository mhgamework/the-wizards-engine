using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant
{
    /// <summary>
    /// Responsible for the World related aspects of an item
    /// Implemented by the engine
    /// </summary>
    public interface IItem
    {
        Vector3 IslandPosition { get; }
        IIsland Island { get; }

        float Durability { get; set; }

        IMachine Machine { get; }

        IItemType Type { get; }

    }

    [ModelObjectChanged]
    public class SimpleItem : EngineModelObject, IItem
    {
        public SimpleItem(IIsland island)
        {
            Island = island;
        }

        public Physical Physical = new Physical();
        private IItemType type;
        public Vector3 IslandPosition { get { return Physical.GetPosition(); } }
        public IIsland Island { get; private set; }
        public float Durability { get; set; }


        public IMachine Machine
        { get { return TW.Data.Objects.OfType<SimpleMachine>().FirstOrDefault(s => s.Items.Contains(this)); } }

        public IItemType Type
        {
            get { return type; }
            set
            {
                type = value;
                Physical.Mesh = ((SimpleItemType)type).Mesh;
            }
        }
    }
}