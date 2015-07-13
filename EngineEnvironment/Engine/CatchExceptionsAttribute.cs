using System;
using System.Collections.Generic;
using PostSharp;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace MHGameWork.TheWizards.Data
{
    /// <summary>
    /// Catches all exceptions in the method and writes them to the console
    /// </summary>
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method, PersistMetaData = true)]
    public sealed class CatchExceptionsAttribute : OnMethodBoundaryAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            args.FlowBehavior = FlowBehavior.Continue;
            Console.WriteLine(args.Exception);
        }
    }

}
