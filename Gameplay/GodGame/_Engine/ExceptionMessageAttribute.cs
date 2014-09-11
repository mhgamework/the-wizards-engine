using System;
using System.Linq;
using PostSharp.Aspects;

namespace MHGameWork.TheWizards.GodGame._Engine
{
    [Serializable]
    public class ExceptionMessageAttribute : OnMethodBoundaryAspect
    {
        private readonly string msg;

        public ExceptionMessageAttribute(string msg)
        {
            this.msg = msg;
        }

        public override void OnException(MethodExecutionArgs args)
        {
            throw new Exception(msg + "(" + string.Join(", ", args.Arguments.Select(f => f.ToString())) + ")");

        }
    }
}