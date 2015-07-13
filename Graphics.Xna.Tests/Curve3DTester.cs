using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using System.Threading;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.XNA
{
    [TestFixture]
    public class Curve3DTester
    {
        [Test]
        [RequiresThread( ApartmentState.STA )]
        public void TestFollowCurve()
        {
            XNAGame game = new XNAGame();

            Curve3D curve = Curve3D.CreateTestCurve();

            game.SpectaterCamera.CameraDirection = Vector3.Normalize( new Vector3( -0.2f, -1f, -0.4f ) );
            BoundingSphere sphere = curve.CalculateBoundingSphere();
            sphere.Radius += 1.5f;
            game.SpectaterCamera.FitInView( sphere );

            float time = 0;

            game.DrawEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed( Keys.V ))
                    {
                        game.SpectaterCamera.CameraDirection = Vector3.Normalize( new Vector3( -0.2f, -1f, -0.4f ) );
                        
                    }
                    Render(curve, game, Color.Red );
                    game.LineManager3D.AddCenteredBox( curve.Evaluate( time ), 0.2f, Color.Green );
                    time += game.Elapsed;
                };


            game.Run();
        }

      
        public void Render(Curve3D curve, IXNAGame game, Color color)
        {
            float segmentLength = 0.1f;

            float start = curve.GetStart();
            float end = curve.GetEnd();

            Vector3 lastPoint = curve.Evaluate(start);

            for (float i = start + segmentLength; i < end; i += segmentLength)
            {
                Vector3 newPoint = curve.Evaluate(i);
                game.LineManager3D.AddLine(lastPoint, newPoint, color);
                lastPoint = newPoint;
            }

            game.LineManager3D.AddLine(lastPoint, curve.Evaluate(end), color);
        }
    }
}
