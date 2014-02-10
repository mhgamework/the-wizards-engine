using System;

namespace MHGameWork.TheWizards.Scattered.Model
{
    public class Construction
    {
        public Construction()
        {
            UpdateAction = new NullConstructionAction();
        }
        public string Name { get; set; }

        public IConstructionAction UpdateAction { get; set; }

        public string LevelConstructorMethod { get; set; }
    }
}