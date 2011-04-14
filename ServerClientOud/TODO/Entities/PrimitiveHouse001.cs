using System;
using System.Collections.Generic;
using System.Text;
using NovodexWrapper;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Server;
using Wereld = MHGameWork.TheWizards.Server.Wereld;


namespace MHGameWork.TheWizards.ServerClient.Entities
{
	public class PrimitiveHouse001 : Wereld.IServerClientEntity
	{
		private Engine.Model model;

		private Wereld.ServerClientBody body;

		public PrimitiveHouse001()
		{
			//positie = new Vector3( 0 );
			//scale = new Vector3( 4 );

            model = new Engine.Model( ProgramOud.SvClMain, "Content\\PrimitiveHouse001" );



			body = new MHGameWork.TheWizards.ServerClient.Wereld.ServerClientBody(); //MHGameWork.TheWizards.Server.Wereld.Body();

			body.Positie = new Vector3( 0, 5, 0 );
			body.Scale = new Vector3( 10 );


			/*NxActorDesc actorDesc = new NxActorDesc();
			NxBodyDesc bodyDesc = new NxBodyDesc();
			//NxSphereShapeDesc sphereDesc = new NxSphereShapeDesc();
			NxBoxShapeDesc boxDesc = new NxBoxShapeDesc();

			boxDesc.dimensions = Vector3.One * 10f;
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

			body.Actor = actor;*/



		}


		#region IClientEntity Members


		void MHGameWork.TheWizards.ServerClient.Wereld.IClientEntity.Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
		{

		}

		void MHGameWork.TheWizards.ServerClient.Wereld.IClientEntity.Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e)
		{

		}

		/*BoundingSphere MHGameWork.TheWizards.ServerClient.Wereld.IClientEntity.BoundingSphere
		{
			get { throw new Exception( "The method or operation is not implemented." ); }
		}*/

		#endregion

		#region IServerEntity Members

		void MHGameWork.TheWizards.Server.Wereld.IServerEntity.Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
		{

		}

		void MHGameWork.TheWizards.Server.Wereld.IServerEntity.Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e)
		{

		}

		/*BoundingSphere MHGameWork.TheWizards.Server.Wereld.IServerEntity.BoundingSphere
		{
			get { throw new Exception( "The method or operation is not implemented." ); }
		}*/

		#endregion



		public void Render()
		{
			

			model.TempRender( Matrix.CreateScale( body.Scale )
				* body.Rotatie
				* Matrix.CreateTranslation( body.Positie ) );
		}



		public Microsoft.Xna.Framework.BoundingSphere BoundingSphere
		{
			get
			{
				//TODO: scale.Length() gebruikt veel tijd
				return new BoundingSphere( body.Positie, body.Scale.Length() );
			}
		}



		public Wereld.ServerClientBody Body { get { return body; } }


		public static void CreatePrimitiveHouse001Entity(Entities.PrimitiveHouse001 nEnt, out Wereld.ClientEntityHolder nClientEntH, out Server.Wereld.ServerEntityHolder nServerEntH)
		{
			nClientEntH = new Wereld.ClientEntityHolder( nEnt );
			nServerEntH = new Server.Wereld.ServerEntityHolder( nEnt );

			nClientEntH.AddElement( nEnt.body );
			nServerEntH.AddElement( nEnt.body );

			//return entH;

		}









        #region IClientEntity Members

        void MHGameWork.TheWizards.ServerClient.Wereld.IClientEntity.Render()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        BoundingSphere MHGameWork.TheWizards.ServerClient.Wereld.IClientEntity.BoundingSphere
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        #endregion

        #region IServerEntity Members


        BoundingSphere MHGameWork.TheWizards.Server.Wereld.IServerEntity.BoundingSphere
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        void MHGameWork.TheWizards.Server.Wereld.IServerEntity.EnablePhysics()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        void MHGameWork.TheWizards.Server.Wereld.IServerEntity.DisablePhysics()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}
