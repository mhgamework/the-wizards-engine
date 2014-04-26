using System;
using Autofac;
using Autofac.Core;

namespace MHGameWork.TheWizards.Scattered.Bindings
{
    public class LogRequestsModule : Module
    {
        protected override void AttachToComponentRegistration(
            IComponentRegistry componentRegistry,
            IComponentRegistration registration)
        {
            // Use the event args to log detailed info
            registration.Preparing += (sender, args) =>
                                      Console.WriteLine(
                                          "Resolving concrete type {0}",
                                          args.Component.Activator.LimitType);
        }
    }
}