using MHGameWork.TheWizards.DirectX11.Graphics;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11
{
    [TestFixture]
    public class SpectaterCameraTest
    {
        [Test]
        public void TestSetDirection()
        {
            var cam = new SpectaterCamera();

            checkDirectionSet(cam, new Vector3(1, 0, 0));
            checkDirectionSet(cam, new Vector3(0, 1, 0));
            checkDirectionSet(cam, new Vector3(0, 0, 1));

            checkDirectionSet(cam, new Vector3(-1, 0, 0));
            checkDirectionSet(cam, new Vector3(0, -1, 0));
            checkDirectionSet(cam, new Vector3(0, 0, -1));

            checkDirectionSet(cam, new Vector3(1, 0, 1));
            checkDirectionSet(cam, new Vector3(1, 1, 0));

            checkDirectionSet(cam, new Vector3(1, 1, 0));
            checkDirectionSet(cam, new Vector3(1, -1, 0));

            checkDirectionSet(cam, new Vector3(-1, 1, 0));
            checkDirectionSet(cam, new Vector3(-1, -1, 0));

            checkDirectionSet(cam, new Vector3(0, 1, 1));
            checkDirectionSet(cam, new Vector3(0, -1, 1));
        }

        private void checkDirectionSet(SpectaterCamera cam, Vector3 dir)
        {
            dir = Vector3.Normalize(dir);
            cam.CameraDirection = dir;
            Assert.True( Vector3.Distance(cam.CameraDirection,dir) < 0.001f);
        }
    }
}