using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using System.Threading;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Graphics
{
    [TestFixture]
    public class Curve3DTester
    {
        [Test]
        [RequiresThread( ApartmentState.STA )]
        public void TestFollowCurve()
        {
            XNAGame game = new XNAGame();

            Curve3D curve = CreateTestCurve();

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
                    curve.Render( game, Color.Red );
                    game.LineManager3D.AddCenteredBox( curve.Evaluate( time ), 0.2f, Color.Green );
                    time += game.Elapsed;
                };


            game.Run();
        }

        public static Curve3D CreateTestCurve()
        {
            Curve3D curve = new Curve3D();

            curve.PreLoop = CurveLoopType.Constant;
            curve.PostLoop = CurveLoopType.Cycle;

            curve.AddKey( 0, new Vector3( 2, 2, 2 ) );
            curve.AddKey( 1, new Vector3( 4, 0, 2 ) );
            curve.AddKey( 5, new Vector3( 8, 0, 7 ) );
            curve.AddKey( 7, new Vector3( 2, 1, 5 ) );
            curve.AddKey( 9, new Vector3( 2, 2, 2 ) );

            curve.SetTangents();
            return curve;
        }
    }
}
