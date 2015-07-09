using System;
using System.Collections.Generic;
using PostSharp;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace MHGameWork.TheWizards.Data
{
    /// <summary>
    /// Responsible for setting the persistance scope for methods that need persistance
    /// </summary>
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method, PersistMetaData = true)]
    public sealed class PersistanceScopeAttribute : OnMethodBoundaryAspect
    {
        private static Stack<bool> persistanceStack = new Stack<bool>();
        public override void OnEntry(MethodExecutionArgs args)
        {
            persistanceStack.Push(TW.Data.InPersistenceScope);
            TW.Data.InPersistenceScope = true;
        }
        public override void OnExit(MethodExecutionArgs args)
        {
            TW.Data.InPersistenceScope = persistanceStack.Pop();
        }
    }

}
