//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxSpringAndDamperEffectorDesc
	{
		public IntPtr body1_actorPtr;
		public IntPtr body2_actorPtr;
		public Vector3 pos1;
		public Vector3 pos2;
		//linear spring parameters:
		public float springDistCompressSaturate;
		public float springDistRelaxed;
		public float springDistStretchSaturate;
		public float springMaxCompressForce;
		public float springMaxStretchForce;
		//linear damper parameters:
		public float damperVelCompressSaturate;
		public float damperVelStretchSaturate;
		public float damperMaxCompressForce;
		public float damperMaxStretchForce;


		public static NxSpringAndDamperEffectorDesc Default
			{get{return new NxSpringAndDamperEffectorDesc();}}
		
		public NxSpringAndDamperEffectorDesc()
			{setToDefault();}


		public NxSpringAndDamperEffectorDesc(NxActor actorBody1,NxActor actorBody2,Vector3 pos1,Vector3 pos2,float springDistCompressSaturate,float springDistRelaxed,float springDistStretchSaturate,float springMaxCompressForce,float springMaxStretchForce,float damperVelCompressSaturate,float damperVelStretchSaturate,float damperMaxCompressForce,float damperMaxStretchForce)
		{
			this.body1_actorPtr				= actorBody1.NxActorPtr;
			this.body2_actorPtr				= actorBody2.NxActorPtr;
			this.pos1						= pos1;
			this.pos2						= pos2;
			this.springDistCompressSaturate	= springDistCompressSaturate;
			this.springDistRelaxed			= springDistRelaxed;
			this.springDistStretchSaturate	= springDistStretchSaturate;
			this.springMaxCompressForce		= springMaxCompressForce;
			this.springMaxStretchForce		= springMaxStretchForce;
			this.damperVelCompressSaturate	= damperVelCompressSaturate;
			this.damperVelStretchSaturate	= damperVelStretchSaturate;
			this.damperMaxCompressForce		= damperMaxCompressForce;
			this.damperMaxStretchForce		= damperMaxStretchForce;
		}

		public void setToDefault()
		{
			body1_actorPtr				= IntPtr.Zero;
			body2_actorPtr				= IntPtr.Zero;
			pos1						= new Vector3(0,0,0);
			pos2						= new Vector3(0,0,0);
			springDistCompressSaturate	= 0;
			springDistRelaxed			= 0;
			springDistStretchSaturate	= 0;
			springMaxCompressForce		= 0;
			springMaxStretchForce		= 0;
			damperVelCompressSaturate	= 0;
			damperVelStretchSaturate	= 0;
			damperMaxCompressForce		= 0;
			damperMaxStretchForce		= 0;
		}
		
		
		public NxActor body1
		{
			get{return NxActor.createFromPointer(body1_actorPtr);}
			set{body1_actorPtr=value.NxActorPtr;}
		}

		public NxActor body2
		{
			get{return NxActor.createFromPointer(body2_actorPtr);}
			set{body2_actorPtr=value.NxActorPtr;}
		}
	}
}






