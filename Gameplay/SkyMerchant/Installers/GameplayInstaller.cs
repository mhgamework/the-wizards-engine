﻿using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay
{
    /// <summary>
    /// Installs the quest editor's production dependencies
    /// </summary>
    public class GameplayInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn<IWorldScript>().WithServiceSelf().LifestyleTransient());
            container.Register(Classes.FromThisAssembly()
                   .InNamespace("MHGameWork.TheWizards.SkyMerchant.Gameplay", true)
                   .WithServiceSelf()
                   .WithServiceDefaultInterfaces());
        }
    }
}