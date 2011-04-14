//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxContactPair
	{
		public IntPtr actorPtr0;
		public IntPtr actorPtr1;
		public IntPtr streamPtr;
		public Vector3 sumNormalForce;		//!< the total contact normal force that was applied for this pair, to maintain nonpenetration constraints.
		public Vector3 sumFrictionForce;	//!< the total tangential force that was applied for this pair.


		public NxActor Actor0
			{get{return NxActor.createFromPointer(actorPtr0);}}

		public NxActor Actor1
			{get{return NxActor.createFromPointer(actorPtr1);}}
	}
}







