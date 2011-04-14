using System;
using System.Collections.Generic;
using System.Text;
using NovodexWrapper;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Entities
{
	public class TestPlayerHitReport : NxUserControllerHitReport
	{

		public override NxControllerAction onControllerHit(NxControllersHit hit)
		{
			return NxControllerAction.NX_ACTION_NONE;
		}

		public override NxControllerAction onShapeHit(NxControllerShapeHit hit)
		{
			if ( hit.Shape == null ) return NxControllerAction.NX_ACTION_NONE ; //Vreemde edit in the wrapper
			NxActor actor = hit.Shape.getActor();
			if ( actor == null ) return NxControllerAction.NX_ACTION_NONE;
			if ( ( ( hit.dir.Y == 0.0f ) && actor.isDynamic() ) && !actor.FlagKinematic )
			{
				float num = ( ( actor.Mass * hit.length ) * 0.4f );
				actor.addForceAtLocalPos( (Vector3)( hit.dir * num ), new Vector3( 0.0f, 0.0f, 0.0f ), NxForceMode.NX_IMPULSE );
				actor.addForceAtPos( (Vector3)( hit.dir * num ), hit.Controller.getPosition(), NxForceMode.NX_IMPULSE );
				actor.addForceAtPos( (Vector3)( hit.dir * num ), hit.WorldPos, NxForceMode.NX_IMPULSE );
			}
			return NxControllerAction.NX_ACTION_NONE;
		}



	}
}
