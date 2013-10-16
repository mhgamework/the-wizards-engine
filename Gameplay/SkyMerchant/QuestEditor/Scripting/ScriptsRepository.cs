using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.Scripting
{
    /// <summary>
    /// Provides access to the script types in the gameplay dll
    /// </summary>
    public class ScriptsRepository
    {
        private readonly IKernel kernel;
        private Dictionary<Type, IScriptType> scripts = new Dictionary<Type, IScriptType>();

        public ScriptsRepository(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public IScriptType GetScriptType(Type type)
        {
            return scripts.GetOrCreate(type, () => new ScriptType(type, this));
        }

        public IWorldScript ActivateScript(Type scriptType)
        {
            return (IWorldScript)kernel.Resolve(scriptType);
        }
    }

    public class ScriptType : IScriptType
    {
        private readonly Type type;
        private readonly ScriptsRepository repository;

        public ScriptType(Type type, ScriptsRepository repository)
        {
            this.type = type;
            this.repository = repository;
        }

        public IWorldScript CreateInstance()
        {
            return repository.ActivateScript(type);
        }

        public string Name { get { return type.Name; } }
    }
}