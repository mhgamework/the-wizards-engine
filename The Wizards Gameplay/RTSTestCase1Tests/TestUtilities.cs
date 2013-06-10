using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using NSubstitute;

namespace MHGameWork.TheWizards.RTSTestCase1.Tests
{
    public static class TestUtilities
    {
        public static void SetupTWContext()
        {
            var ctx = new TW.Context();
            TW.SetContext(ctx);
            ctx.Data = new DataWrapper(Substitute.For<ITraceLogger>());
            ctx.Assets = new AssetsWrapper();
        }
    }
}