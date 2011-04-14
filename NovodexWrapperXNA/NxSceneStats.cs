//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxSceneStats
	{
		//collisions:
		public uint numContacts;
		public uint maxContacts;
		public uint	numPairs;
		public uint maxPairs;
		//sleep:
		public uint numDynamicActorsInAwakeGroups;
		public uint maxDynamicActorsInAwakeGroups;
		//solver:
		public uint numAxisConstraints;
		public uint maxAxisConstraints;
		public uint numSolverBodies;
		public uint maxSolverBodies;
		//scene:
		public uint numActors;
		public uint maxActors;
		public uint numDynamicActors;
		public uint maxDynamicActors;
		public uint numStaticShapes;
		public uint maxStaticShapes;
		public uint numDynamicShapes;
		public uint maxDynamicShapes;
		public uint numJoints;
		public uint maxJoints;
		

		public NxSceneStats()
			{reset();}
			
		public void reset()
		{
			numContacts = 0;
			maxContacts = 0;
			numPairs = 0;
			maxPairs = 0;
			numDynamicActorsInAwakeGroups = 0;
			maxDynamicActorsInAwakeGroups = 0;
			numAxisConstraints = 0;
			maxAxisConstraints = 0;
			numSolverBodies = 0;
			maxSolverBodies = 0;
			numActors = 0;
			maxActors = 0;
			numDynamicActors = 0;
			maxDynamicActors = 0;
			numStaticShapes = 0;
			maxStaticShapes = 0;
			numDynamicShapes = 0;
			maxDynamicShapes = 0;
			numJoints = 0;
			maxJoints = 0;

		}
	}
}

