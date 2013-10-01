﻿using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    /// <summary>
    /// Simulates rendering for all skymerchant features
    /// </summary>
    public class SkyMerchantRenderingSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var i in TW.Data.Objects.OfType<IslandPart>())
                i.FixPhysical();
            foreach (var i in TW.Data.Objects.OfType<TraderVisualizerPart>().ToArray())
                i.FixMesh();
            foreach (var i in TW.Data.Objects.OfType<ItemPart>())
                i.FixPosition();
        }
    }
}