using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace MHGameWork.TheWizards.Model
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Property)] 
    public sealed class ModelObjectChangedAttribute :LocationInterceptionAspect
    {
        public override void OnSetValue(LocationInterceptionArgs args)
        {
            
            if (args.Value != args.GetCurrentValue())
            {
                Console.WriteLine("Changeed!");
            }
            args.ProceedSetValue();
            base.OnSetValue(args);
        }
    }

}
