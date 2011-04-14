using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NovodexWrapper;

namespace MHGameWork.TheWizards.Server.Entities
{
	public class Shuriken001 : Wereld.IServerEntity
	{
		private Vector3 positie;
		private Vector3 scale;
		private NxActor actor;

		public Shuriken001()
		{
			positie = new Vector3( 0 );
			scale = new Vector3( 4 );

			NxSphereShapeDesc shapeDesc = new NxSphereShapeDesc( scale.X );
			NxActorDesc actorDesc = new NxActorDesc();
			NxBodyDesc bodyDesc = new NxBodyDesc();


			actorDesc.addShapeDesc( shapeDesc );
			actorDesc.BodyDesc = bodyDesc;
			actorDesc.density = 1;
			actorDesc.globalPose = Matrix.CreateTranslation( positie );



			actor = ServerMainNew.Instance.PhysicsScene.createActor( actorDesc );

		}


		public void Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
		{

		}

		public void Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e)
		{
			positie = actor.getGlobalPosition();
		}

		public Microsoft.Xna.Framework.BoundingSphere BoundingSphere
		{
			get
			{
				//TODO: scale.Length() gebruikt veel tijd
				return new BoundingSphere( positie, scale.Length() );
			}
		}

		public Vector3 Positie
		{
			get { return positie; }
			set
			{
				positie = value;
				actor.setGlobalPosition( positie );
			}
		}

		public Vector3 Scale
		{ get { return scale; } set { scale = value; } }









        #region IServerEntity Members


        public void EnablePhysics()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public void DisablePhysics()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }

}
