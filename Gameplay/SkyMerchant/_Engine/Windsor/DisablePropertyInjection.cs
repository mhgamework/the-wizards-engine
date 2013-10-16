using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.Facilities.TypedFactory;
using Castle.Facilities.TypedFactory.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Castle.MicroKernel.Registration;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.Windsor
{
    /// <summary>
    /// Allows disabling property injection
    /// NOT IMPLEMENTED. Currently using the Component.Properties extension method. 
    /// However, the original intent of this was to be able to disable property injection altogether, which was not solved yet
    /// </summary>
    public class DisablePropertyInjectionFacility : AbstractFacility
    {
        public static string DisableAttribute = "DisablePropertyWiring";
        protected override void Init()
        {
            Kernel.ComponentModelBuilder.AddContributor(new DisablePropertyInjectionInspector());
        }
    }

    public class DisablePropertyInjectionInspector : IContributeComponentModelConstruction
    {
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            //if (model.Configuration.Attributes[DisablePropertyInjectionFacility.DisableAttribute] == null) return;

            // Disable all property dependencies
            foreach (var prop in model.Properties)
            {
                //model.Dependencies.Remove(prop.Dependency);
            }
        }
    }
    

    /// <summary>
    /// Note: this IS implemented
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class TypedFactoryRegistrationExtensions
    {
        /// <summary>
        /// Disables property wiring for these components.
        /// Note that this must be called every time the component is registered, otherwise properties might still be used as dependencies.
        /// </summary>
        public static BasedOnDescriptor DisablePropertyWiring(this BasedOnDescriptor registration)
        {
            registration.Configure(delegate(ComponentRegistration componentRegistration)
                {
                    componentRegistration.Properties(PropertyFilter.IgnoreAll);
                });
            return registration;
        }
    }
}