using System;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Player;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Tests.Player
{
    [TestFixture]
    public class XNATaskQueueTester
    {
        private float tempMeasurement;

        [Test]
        [RequiresThread( System.Threading.ApartmentState.STA )]
        public void TestTaskQueue()
        {
            tempMeasurement = 0;

            XNAGame game = new XNAGame();

            XNATaskQueue queue = new XNATaskQueue( game );


            game.AddXNAObject( queue );

            queue.AddUpdateTask( 1,
                delegate( float elapsed )
                {
                    tempMeasurement += elapsed;
                    // Do nothing
                } );
            queue.AddUpdateTask( 2,
                delegate( float elapsed )
                {
                    tempMeasurement -= elapsed;
                    game.LineManager3D.AddBox( new BoundingBox( new Vector3( 1, 0, 1 ), new Vector3( 3, 3, 3 ) ), Color.Red );
                } );

            queue.AddUpdateTask( 1,
                delegate( float elapsed )
                {
                    tempMeasurement += elapsed;
                    // Do nothing
                } );

            queue.AddUpdateTask( 0,
                delegate( float elapsed )
                {
                    game.Exit();
                } );


            game.Run();


            // This should be 0, (floating imprecesion possible)
            if ( Math.Abs( tempMeasurement ) > 0.001f )
            {
                throw new Exception( "Taskqueue works incorrectly" );
            }

        }
    }
}
