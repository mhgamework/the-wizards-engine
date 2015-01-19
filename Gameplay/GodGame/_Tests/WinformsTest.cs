using System.Windows.Forms;
using MHGameWork.TheWizards.Engine.Features.Testing;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [TestFixture]
    [EngineTest]
    public class WinformsTest
    {
        [Test]
        public void TestShowFrom()
        {
            var f = new Form();
            TW.Engine.RegisterOnClearEngineState(f.Dispose);
            var txt = new Label();
            f.Controls.Add(txt);
            txt.Dock = DockStyle.Fill;
            txt.Text = "Great success";

            f.Show(TW.Graphics.Form.Form);
            TW.Graphics.Form.Form.Focus();


        }
    }
}