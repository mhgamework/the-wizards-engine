using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.ModelBuilder;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.Windsor
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NonOptionalAttribute : Attribute
    {
    }

    public class NonOptionalPropertiesFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.ComponentModelBuilder.AddContributor(new NonOptionalInspector());
        }
    }

    public class NonOptionalInspector : IContributeComponentModelConstruction
    {
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            foreach (var prop in model.Properties.Where(prop => prop.Property.IsDefined(typeof(NonOptionalAttribute), false)))
            {
                prop.Dependency.IsOptional = false;
            }
        }
    }
}