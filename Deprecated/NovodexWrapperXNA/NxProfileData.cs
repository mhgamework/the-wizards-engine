//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxProfileZone
	{
		public IntPtr	internalNamePtr=IntPtr.Zero;	//!< Name of the zone.
		public uint		callCount=0;					//!< The number of times this zone was executed over the last profiling run (since readProfileData(true) was called.)
		public uint		hierTime=0;						//!< Time in clock cycles that it took to execute the total of the calls to this zone.
		public uint		selfTime=0;						//!< Time in clock cycles that it took to execute the total of the calls to this zone, minus the time it took to execute the zones called from this zone.  
		public uint		recursionLevel=0;				//!< The number of parent zones this zone has, each of which called the next until this zone was called.  Can be used to indent a tree display of the zones.  Sometimes a zone could have multiple rec. levels as it was called from different places.  In this case the first encountered rec level is displayed.
		public float	percent=0;						//!< The percentage time this zone took of its parent zone's time.  If this zone has multiple parents (the code was called from multiple places), this is zero. 
		public uint		counter=0;						//Not used yet

		public string Name
		{
			get
			{
				if(internalNamePtr==IntPtr.Zero)
					{return null;}
				return Marshal.PtrToStringAnsi(internalNamePtr);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public class NxProfileData
	{
		public uint numZones;
		public NxProfileZone[] profileZones;

		public static NxProfileData createFromPointer(IntPtr profileDataPtr)
		{
			if(profileDataPtr==IntPtr.Zero)
				{return null;}

			NxProfileData profileData=new NxProfileData();
			profileData.numZones=wrapper_ProfileData_getNumZones(profileDataPtr);
			profileData.profileZones=new NxProfileZone[profileData.numZones];
			for(uint i=0;i<profileData.numZones;i++)
			{
				profileData.profileZones[i]=new NxProfileZone();
				wrapper_ProfileData_getProfileZone(profileDataPtr,i,profileData.profileZones[i]);
			}
			
			return profileData;
		}

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ProfileData_getNumZones(IntPtr profileData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_ProfileData_getProfileZone(IntPtr profileDataPtr,uint zoneIndex,NxProfileZone profileZone);
	}

}
