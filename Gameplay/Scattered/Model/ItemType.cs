using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Scattered.Model
{
    public class ItemType
    {
        public ItemType()
        {
            Name = "UNSET";
            //Mesh = // Default mesh!!
        }

        public string Name { get; set; }

        public IMesh Mesh { get; set; }

        public string TexturePath { get; set; }
    }
}