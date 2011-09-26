using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Scripting
{
    public abstract class ScriptWrapper
    {
        public object Script
        {
            get { return getScript(); }
            set { setScript(value); }
        }

        protected abstract void setScript(object value);

        protected abstract object getScript();

    }
}
