using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NovodexWrapper;

namespace MHGameWork.TheWizards.Server.Entities
{
    public class Player : Wereld.IServerEntity
    {
        PlayerHitReport hitReport;
        public NxCapsuleController controller;


        public Wereld.Body body;

        private ServerMainNew main;

        public Player( ServerMainNew nMain )
        {
            main = nMain;

            body = new MHGameWork.TheWizards.Server.Wereld.Body();

            body.Positie = new Vector3( 0, 5, 0 );
            body.Scale = new Vector3( 1f );



            hitReport = new PlayerHitReport();

            NxCapsuleControllerDesc controllerDesc = NxCapsuleControllerDesc.Default;
            controllerDesc.position = body.Positie;
            controllerDesc.upDirection = NxHeightFieldAxis.NX_Y;
            controllerDesc.radius = 1.2f;// *10;
            controllerDesc.height = 0.1f;// *10;
            controllerDesc.stepOffset = 1;// *10;
            controllerDesc.Callback = hitReport;
            controllerDesc.slopeLimit = 0;
            controller = (NxCapsuleController)ControllerManager.createController( ServerMainNew.Instance.PhysicsScene, controllerDesc );
            /*NxCapsuleShapeDesc shapeDesc = NxCapsuleShapeDesc.Default;
            shapeDesc.height = 2.1f * 10;
            shapeDesc.radius = 2 * 10;
            shapeDesc.localPose = Matrix.CreateTranslation( 0, 0, -5 );*/



            /*'If (Not controller Is Nothing) Then
            '    controller.getActor.FlagFrozenRot = True
            '    controller.getActor.setName("CapsuleController")
            '    'If addArrowShapeFlag Then
            '    'controller.getActor.createShape(Me.createSpikeShapeDesc((capsuleRadius / 4.0!), (capsuleRadius / 4.0!), (capsuleRadius * 2.0!), New Vector3(0.0!, ((capsuleTotalHeight - (capsuleRadius * 2.0!)) / 2.0!), 0.0!)))
            '    controller.getActor.createShape(Me.createSpikeShapeDesc(2 / 4.0!, 2 / 4.0!, 2 / 4.0!, New Vector3(0.0!, 2 / 0.2!, 0.0!)))
            '    controller.getActor.getLastShape.FlagDisableCollision = True
            '    'End If
            'End If*/




            body.Actor = controller.getActor();



        }

        //#region IServerEntity Members

        //public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        //{

        //}

        //public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        //{

        //}

        //#endregion


        //public void EnablePhysics()
        //{

        //}

        //public void DisablePhysics()
        //{

        //}

        //public void FlyUp()
        //{
        //    if ( body.Actor != null ) body.Actor.addLocalForce( new Vector3( 0, 700, 0 ), NxForceMode.NX_ACCELERATION );
        //    Random r = new Random( DateTime.Now.Millisecond );

        //    main.Invoker.Invoke( FlyUp, main.ProcessEventArgs.Time + r.Next( 3000, 8000 ) );

        //}








        //public Wereld.Body Body { get { return body; } }


        public static Wereld.ServerEntityHolder CreatePlayerEntity( Entities.Player nEnt )
        {
            Wereld.ServerEntityHolder entH = new Server.Wereld.ServerEntityHolder( nEnt );

            entH.AddElement( nEnt.body );

            return entH;

        }











        #region IServerEntity Members

        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {

        }

        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {


        }


        public Microsoft.Xna.Framework.BoundingSphere BoundingSphere
        {
            get
            {
                //TODO: scale.Length() gebruikt veel tijd
                return new BoundingSphere( body.Positie, body.Scale.Length() * 5 );
            }
        }

        public void EnablePhysics()
        {

        }

        public void DisablePhysics()
        {

        }

        #endregion
    }

}
