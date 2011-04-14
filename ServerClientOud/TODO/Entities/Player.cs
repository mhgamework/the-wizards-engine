using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NovodexWrapper;
namespace MHGameWork.TheWizards.ServerClient.Entities
{
    public class Player : Wereld.IClientEntity
    {
        //TestPlayerHitReport hitReport;
        //NxCapsuleController controller;

        private Wereld.Body body;
        //private Wereld.ClientEntityHolder entityHolder;

        public Player()
        {
            body = new MHGameWork.TheWizards.ServerClient.Wereld.Body();


            //hitReport = new TestPlayerHitReport();

            //NxCapsuleControllerDesc controllerDesc = NxCapsuleControllerDesc.Default;
            //controllerDesc.position = body.Positie;
            //controllerDesc.upDirection = NxHeightFieldAxis.NX_Y;
            //controllerDesc.radius = 2 * 10;
            //controllerDesc.height = 1 * 10;
            //controllerDesc.stepOffset = 1 * 10;
            //controllerDesc.Callback = hitReport;
            //controllerDesc.slopeLimit = 0;
            //controller = (NxCapsuleController)ControllerManager.createController( ServerMainNew.Instance.PhysicsScene, controllerDesc );
            ///*NxCapsuleShapeDesc shapeDesc = NxCapsuleShapeDesc.Default;
            //shapeDesc.height = 2.1f * 10;
            //shapeDesc.radius = 2 * 10;
            //shapeDesc.localPose = Matrix.CreateTranslation( 0, 0, -5 );*/



            ///*'If (Not controller Is Nothing) Then
            //'    controller.getActor.FlagFrozenRot = True
            //'    controller.getActor.setName("CapsuleController")
            //'    'If addArrowShapeFlag Then
            //'    'controller.getActor.createShape(Me.createSpikeShapeDesc((capsuleRadius / 4.0!), (capsuleRadius / 4.0!), (capsuleRadius * 2.0!), New Vector3(0.0!, ((capsuleTotalHeight - (capsuleRadius * 2.0!)) / 2.0!), 0.0!)))
            //'    controller.getActor.createShape(Me.createSpikeShapeDesc(2 / 4.0!, 2 / 4.0!, 2 / 4.0!, New Vector3(0.0!, 2 / 0.2!, 0.0!)))
            //'    controller.getActor.getLastShape.FlagDisableCollision = True
            //'    'End If
            //'End If*/




            //body.serverBody.Actor = controller.getActor();
        }
        public static Wereld.ClientEntityHolder CreatePlayerEntity( Player ent )
        {
            Wereld.ClientEntityHolder entH = new MHGameWork.TheWizards.ServerClient.Wereld.ClientEntityHolder( ent );
            entH.AddElement( ent.body );

            return entH;
        }

        #region IClientEntity Members

        public void Render()
        {

        }

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
                throw new Exception();
            }
        }

        #endregion


        //public Wereld.ClientEntityHolder EntityHolder
        //{
        //    get { return entityHolder; }
        //}


    }
}
