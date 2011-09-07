using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.WorldSimulation
{
    public class CreatureProperty<T>
    {
        public string Description { get; private set; }
        public CreatureProperty(string desc)
        {
            Description = desc;
        }
        //private T data;
        //public T Get()
        //{
        //    return data;
        //}
        //public void Set(T value)
        //{
        //    data = value;
        //}
    }
}
