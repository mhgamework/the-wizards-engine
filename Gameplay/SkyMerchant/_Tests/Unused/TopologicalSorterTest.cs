using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant.GameObjects;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using NSubstitute;
using NUnit.Framework;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Unused
{
    /// <summary>
    /// Allows updating a graph of objects, parents first
    /// </summary>
    [Category("RunsAutomated")]
    [TestFixture]
    public class TopologicalSorterTest
    {
        [Test]
        public void TestTopologicalUpdater()
        {

            var calls = new List<IGraphNode>();

            var list = new List<IGraphNode>();
            for (int i = 0; i < 5; i++)
            {
                list.Add(Substitute.For<IGraphNode>());
            }

            list[0].Parent = list[4];
            list[1].Parent = null;
            list[2].Parent = list[4];
            list[3].Parent = list[0];
            list[4].Parent = null;




            var updater = new TopologicalUpdater();
            updater.UpdateInTopologicalOrder(list, o => o.Parent, calls.Add);

            foreach (var item in list)
            {
                Assert.True(calls.Contains(item));
                if (item.Parent != null)
                    Assert.True(calls.IndexOf(item.Parent) < calls.IndexOf(item));
            }
        }

        private interface IGraphNode
        {
            IGraphNode Parent { get; set; }
        }

    }
}