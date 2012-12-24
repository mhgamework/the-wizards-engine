using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Gameplay
{
    public static class EngineFactory
    {
        static EngineFactory()
        {
            Instance = new EngineFactoryContext();
        }

        public static EngineFactoryContext Instance { get; set; }
        public static TWEngine CreateEngine()
        {
            return Instance.CreateEngine();
        }

        public class EngineFactoryContext
        {
            public virtual TWEngine CreateEngine()
            {
                return new TWEngine();
            }
        }
    }
}
