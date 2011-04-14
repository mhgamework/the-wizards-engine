//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxSceneLimits
	{
		public uint maxNbActors;			//!< Expected max number of actors
		public uint maxNbBodies;			//!< Expected max number of bodies
		public uint maxNbStaticShapes;		//!< Expected max number of static shapes
		public uint maxNbDynamicShapes;		//!< Expected max number of dynamic shapes
		public uint maxNbJoints;			//!< Expected max number of joints

		public NxSceneLimits Default
			{get{return new NxSceneLimits();}}
		
		public NxSceneLimits()
			{setToDefault();}

		public NxSceneLimits(uint maxNumActors,uint maxNumBodies,uint maxNumStaticShapes,uint maxNumDynamicShapes,uint maxNumJoints)
		{
			this.maxNbActors=maxNumActors;
			this.maxNbBodies=maxNumBodies;
			this.maxNbStaticShapes=maxNumStaticShapes;
			this.maxNbDynamicShapes=maxNumDynamicShapes;
			this.maxNbJoints=maxNumJoints;
		}
			
		public void setToDefault()
		{
			maxNbActors			= 0;
			maxNbBodies			= 0;
			maxNbStaticShapes	= 0;
			maxNbDynamicShapes	= 0;
			maxNbJoints			= 0;
		}

	}
}
