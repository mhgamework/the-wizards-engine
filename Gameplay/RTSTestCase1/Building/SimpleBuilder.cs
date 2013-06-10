using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using MHGameWork.TheWizards.RTSTestCase1._Tests;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Building
{
    /// <summary>
    /// Builds IBuildable's by taking resources from the environment.
    /// </summary>
    public class SimpleBuilder
    {
        private readonly SimpleWorldLocator locator;
        private readonly IWorldDestroyer destroyer;

        public SimpleBuilder(SimpleWorldLocator locator, IWorldDestroyer destroyer)
        {
            this.locator = locator;
            this.destroyer = destroyer;
            BuildRange = 5;
        }

        public int BuildRange { get; set; }

        public void BuildSingleResource(IBuildable buildable)
        {
            var item = locator.AtObject((IPhysical)buildable, BuildRange).OfType<DroppedThing>().FirstOrDefault(d => buildable.Buildable.StillNeedsResource(d.Thing.Type));

            if (item == null) return;

            destroyer.Destroy(item);

            buildable.Buildable.ProvideResource(item.Thing.Type);

        }
    }
}