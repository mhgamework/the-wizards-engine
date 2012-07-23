using System;
using System.Collections.Generic;
using System.Text;
using NovodexWrapper;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Server;
using Wereld = MHGameWork.TheWizards.Server.Wereld;


namespace MHGameWork.TheWizards.Server.Entities
{
    public class Shuriken003 : Wereld.IServerEntity
    {
        private Wereld.Body body;

        private ServerMainNew main;

        public Shuriken003( ServerMainNew nMain )
        {
            main = nMain;

            body = new MHGameWork.TheWizards.Server.Wereld.Body(); //MHGameWork.TheWizards.Server.Wereld.Body();

            body.Positie = new Vector3( 0, 5, 0 );
            body.Scale = new Vector3( 20f );


            NxActorDesc actorDesc = new NxActorDesc();
            NxBodyDesc bodyDesc = new NxBodyDesc();
            //NxSphereShapeDesc sphereDesc = new NxSphereShapeDesc();
            NxBoxShapeDesc boxDesc = new NxBoxShapeDesc();

            boxDesc.dimensions = new Vector3( 1f, 0.1f, 1f ) * body.Scale;// Vector3.One * 10f;
            //sphereDesc.radius = 1;
            //actorDesc.addShapeDesc( sphereDesc );
            actorDesc.addShapeDesc( boxDesc );


            actorDesc.BodyDesc = bodyDesc;
            actorDesc.density = 10;
            actorDesc.globalPose = Matrix.CreateTranslation( body.Positie );

            NxActor actor;
            actor = ServerMainNew.Instance.PhysicsScene.createActor( actorDesc );
            while ( actor == null )
            {
                actor = ServerMainNew.Instance.PhysicsScene.createActor( actorDesc );

            }

            body.Actor = actor;



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
                //de *2 is omdat dit een bewegend object is zodat het niet door de grond zakt ... todo
                return new BoundingSphere( body.Positie, body.Scale.Length() * 20 );
            }
        }
        #endregion


        public void EnablePhysics()
        {

        }

        public void DisablePhysics()
        {

        }

        public void FlyUp()
        {
            if ( body.Actor != null ) body.Actor.addLocalForce( new Vector3( 0, 700, 0 ), NxForceMode.NX_ACCELERATION );
            Random r = new Random( DateTime.Now.Millisecond );

            main.Invoker.Invoke( FlyUp, main.ProcessEventArgs.Time + r.Next( 3000, 8000 ) );

        }








        public Wereld.Body Body { get { return body; } }


        public static Wereld.ServerEntityHolder CreateShuriken003Entity( Entities.Shuriken003 nEnt )
        {
            Wereld.ServerEntityHolder entH = new Server.Wereld.ServerEntityHolder( nEnt );

            entH.AddElement( nEnt.body );

            return entH;

        }











    }
}
