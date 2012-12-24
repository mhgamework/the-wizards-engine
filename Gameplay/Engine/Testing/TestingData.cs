using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Engine.Testing
{
    [ModelObjectChanged]
    public class TestingData : EngineModelObject
    {
        public String ActiveTestClass { get; set; }

        public string ActiveTestMethod { get; set; }
    }
}
