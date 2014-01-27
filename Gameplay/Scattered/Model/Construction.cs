using System;

namespace MHGameWork.TheWizards.Scattered.Model
{
    public class Construction
    {
        public Construction()
        {
            UpdateAction = delegate {};
        }
        public string Name { get; set; }

        public Action UpdateAction { get; set; }
    }
}