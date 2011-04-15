//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	public class NxSpringAndDamperEffector : NxEffector
	{
		public NxSpringAndDamperEffector(IntPtr effectorPointer) : base(effectorPointer)
			{}
			
		new public static NxSpringAndDamperEffector createFromPointer(IntPtr effectorPointer)
		{
			if(effectorPointer==IntPtr.Zero)
				{return null;}
			return new NxSpringAndDamperEffector(effectorPointer);
		}
		





		virtual public void setBodies(NxActor actorBody1,Vector3 globalPos1,NxActor actorBody2,Vector3 globalPos2)
			{wrapper_SpringAndDamperEffector_setBodies(nxEffectorPtr,actorBody1.NxActorPtr,ref globalPos1,actorBody2.NxActorPtr,ref globalPos2);}

		virtual public void setLinearSpring(float distCompressSaturate,float distRelaxed,float distStretchSaturate,float maxCompressForce,float maxStretchForce)
			{wrapper_SpringAndDamperEffector_setLinearSpring(nxEffectorPtr,distCompressSaturate,distRelaxed,distStretchSaturate,maxCompressForce,maxStretchForce);}

		virtual public void getLinearSpring(out float distCompressSaturate,out float distRelaxed,out float distStretchSaturate,out float maxCompressForce,out float maxStretchForce)
			{wrapper_SpringAndDamperEffector_getLinearSpring(nxEffectorPtr,out distCompressSaturate,out distRelaxed,out distStretchSaturate,out maxCompressForce,out maxStretchForce);}

		virtual public void setLinearDamper(float velCompressSaturate,float velStretchSaturate,float maxCompressForce,float maxStretchForce)
			{wrapper_SpringAndDamperEffector_setLinearDamper(nxEffectorPtr,velCompressSaturate,velStretchSaturate,maxCompressForce,maxStretchForce);}

		virtual public void getLinearDamper(out float velCompressSaturate,out float velStretchSaturate,out float maxCompressForce,out float maxStretchForce)
			{wrapper_SpringAndDamperEffector_getLinearDamper(nxEffectorPtr,out velCompressSaturate,out velStretchSaturate,out maxCompressForce,out maxStretchForce);}

		virtual public void saveToDesc(NxSpringAndDamperEffectorDesc effectorDesc)
			{wrapper_SpringAndDamperEffector_saveToDesc(nxEffectorPtr,effectorDesc);}

		virtual public NxSpringAndDamperEffectorDesc getSpringAndDamperEffectorDesc()
		{
			NxSpringAndDamperEffectorDesc effectorDesc=NxSpringAndDamperEffectorDesc.Default;
			saveToDesc(effectorDesc);
			return effectorDesc;
		}





		public float SpringDistCompressSaturate
		{
			get{return getSpringAndDamperEffectorDesc().springDistCompressSaturate;}
			set
			{
				NxSpringAndDamperEffectorDesc d=getSpringAndDamperEffectorDesc();
				this.setLinearSpring(value,d.springDistRelaxed,d.springDistStretchSaturate,d.springMaxCompressForce,d.springMaxStretchForce);
			}
		}

		public float SpringDistRelaxed
		{
			get{return getSpringAndDamperEffectorDesc().springDistRelaxed;}
			set
			{
				NxSpringAndDamperEffectorDesc d=getSpringAndDamperEffectorDesc();
				this.setLinearSpring(d.springDistCompressSaturate,value,d.springDistStretchSaturate,d.springMaxCompressForce,d.springMaxStretchForce);
			}
		}

		public float SpringDistStretchSaturate
		{
			get{return getSpringAndDamperEffectorDesc().springDistStretchSaturate;}
			set
			{
				NxSpringAndDamperEffectorDesc d=getSpringAndDamperEffectorDesc();
				this.setLinearSpring(d.springDistCompressSaturate,d.springDistRelaxed,value,d.springMaxCompressForce,d.springMaxStretchForce);
			}
		}

		public float SpringMaxCompressForce
		{
			get{return getSpringAndDamperEffectorDesc().springMaxCompressForce;}
			set
			{
				NxSpringAndDamperEffectorDesc d=getSpringAndDamperEffectorDesc();
				this.setLinearSpring(d.springDistCompressSaturate,d.springDistRelaxed,d.springDistStretchSaturate,value,d.springMaxStretchForce);
			}
		}

		public float SpringMaxStretchForce
		{
			get{return getSpringAndDamperEffectorDesc().springMaxStretchForce;}
			set
			{
				NxSpringAndDamperEffectorDesc d=getSpringAndDamperEffectorDesc();
				this.setLinearSpring(d.springDistCompressSaturate,d.springDistRelaxed,d.springDistStretchSaturate,d.springMaxCompressForce,value);
			}
		}

		public float DamperVelCompressSaturate
		{
			get{return getSpringAndDamperEffectorDesc().damperVelCompressSaturate;}
			set
			{
				NxSpringAndDamperEffectorDesc d=getSpringAndDamperEffectorDesc();
				this.setLinearDamper(value,d.damperVelStretchSaturate,d.damperMaxCompressForce,d.damperMaxStretchForce);
			}
		}

		public float DamperVelStretchSaturate
		{
			get{return getSpringAndDamperEffectorDesc().damperVelStretchSaturate;}
			set
			{
				NxSpringAndDamperEffectorDesc d=getSpringAndDamperEffectorDesc();
				this.setLinearDamper(d.damperVelCompressSaturate,value,d.damperMaxCompressForce,d.damperMaxStretchForce);
			}
		}

		public float DamperMaxCompressForce
		{
			get{return getSpringAndDamperEffectorDesc().damperMaxCompressForce;}
			set
			{
				NxSpringAndDamperEffectorDesc d=getSpringAndDamperEffectorDesc();
				this.setLinearDamper(d.damperVelCompressSaturate,d.damperVelStretchSaturate,value,d.damperMaxStretchForce);
			}
		}

		public float DamperMaxStretchForce
		{
			get{return getSpringAndDamperEffectorDesc().damperMaxStretchForce;}
			set
			{
				NxSpringAndDamperEffectorDesc d=getSpringAndDamperEffectorDesc();
				this.setLinearDamper(d.damperVelCompressSaturate,d.damperVelStretchSaturate,d.damperMaxCompressForce,value);
			}
		}		









		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SpringAndDamperEffector_setBodies(IntPtr effector,IntPtr actorBody1,ref Vector3 globalPos1,IntPtr actorBody2,ref Vector3 globalPos2);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SpringAndDamperEffector_setLinearSpring(IntPtr effector,float distCompressSaturate,float distRelaxed,float distStretchSaturate,float maxCompressForce,float maxStretchForce);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SpringAndDamperEffector_getLinearSpring(IntPtr effector,out float distCompressSaturate,out float distRelaxed,out float distStretchSaturate,out float maxCompressForce,out float maxStretchForce);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SpringAndDamperEffector_setLinearDamper(IntPtr effector,float velCompressSaturate,float velStretchSaturate,float maxCompressForce,float maxStretchForce);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SpringAndDamperEffector_getLinearDamper(IntPtr effector,out float velCompressSaturate,out float velStretchSaturate,out float maxCompressForce,out float maxStretchForce);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SpringAndDamperEffector_saveToDesc(IntPtr effector,NxSpringAndDamperEffectorDesc effectorDesc);
	}
}


/*
-	virtual void saveToDesc(NxSpringAndDamperEffectorDesc &desc) = 0;
-	virtual void setBodies(NxActor* body1, const NxVec3  & global1, NxActor* body2, const NxVec3  & global2) = 0;
-	virtual void setLinearSpring(NxReal distCompressSaturate, NxReal distRelaxed, NxReal distStretchSaturate, NxReal maxCompressForce, NxReal maxStretchForce) = 0;
-	virtual void getLinearSpring(NxReal & distCompressSaturate, NxReal & distRelaxed, NxReal & distStretchSaturate, NxReal & maxCompressForce, NxReal & maxStretchForce) = 0;
-	virtual void setLinearDamper(NxReal velCompressSaturate, NxReal velStretchSaturate, NxReal maxCompressForce, NxReal maxStretchForce) = 0;
-	virtual void getLinearDamper(NxReal & velCompressSaturate, NxReal & velStretchSaturate, NxReal & maxCompressForce, NxReal & maxStretchForce) = 0;
*/



/*
//This is the old code I used before there was a saveToDesc() method.
		private void saveToDesc(NxSpringAndDamperEffectorDesc j)
		{
			//These aren't accesible at all [body1_actorPtr, body2_actorPtr, pos1, pos2]
			wrapper_SpringAndDamperEffector_getLinearSpring(nxEffectorPtr,out j.springDistCompressSaturate,out j.springDistRelaxed,out j.springDistStretchSaturate,out j.springMaxCompressForce,out j.springMaxStretchForce);
			wrapper_SpringAndDamperEffector_getLinearDamper(nxEffectorPtr,out j.damperVelCompressSaturate,out j.damperVelStretchSaturate,out j.damperMaxCompressForce,out j.damperMaxStretchForce);
		}
*/

