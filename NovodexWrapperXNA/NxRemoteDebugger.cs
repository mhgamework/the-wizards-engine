//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	public class NxRemoteDebugger
	{
		public static readonly uint NX_DBG_DEFAULT_PORT								=5425;
		public static readonly uint NX_DBG_EVENTMASK_EVERYTHING						=0xFFFFFFFF;
		public static readonly uint NX_DBG_EVENTGROUP_BASIC_OBJECTS					=0x00000001;
		public static readonly uint NX_DBG_EVENTGROUP_BASIC_OBJECTS_DYNAMIC_DATA	=0x00000002;
		public static readonly uint NX_DBG_EVENTGROUP_BASIC_OBJECTS_STATIC_DATA		=0x00000004;
		public static readonly uint NX_DBG_EVENTGROUP_JOINTS						=0x00000008;
		public static readonly uint NX_DBG_EVENTGROUP_JOINTS_DATA					=0x00000010;
		public static readonly uint NX_DBG_EVENTGROUP_CONTACTS						=0x00000020;
		public static readonly uint NX_DBG_EVENTGROUP_CONTACTS_DATA					=0x00000040;
		public static readonly uint NX_DBG_EVENTGROUP_TRIGGERS						=0x00000080;
		public static readonly uint NX_DBG_EVENTGROUP_PROFILING						=0x00000100;
		public static readonly uint NX_DBG_EVENTMASK_BASIC_OBJECTS					=(NX_DBG_EVENTGROUP_BASIC_OBJECTS);
		public static readonly uint NX_DBG_EVENTMASK_BASIC_OBJECTS_DYNAMIC_DATA		=(NX_DBG_EVENTGROUP_BASIC_OBJECTS | NX_DBG_EVENTGROUP_BASIC_OBJECTS_DYNAMIC_DATA);
		public static readonly uint NX_DBG_EVENTMASK_BASIC_OBJECTS_STATIC_DATA		=(NX_DBG_EVENTGROUP_BASIC_OBJECTS | NX_DBG_EVENTGROUP_BASIC_OBJECTS_STATIC_DATA);
		public static readonly uint NX_DBG_EVENTMASK_BASIC_OBJECTS_ALL_DATA			=(NX_DBG_EVENTMASK_BASIC_OBJECTS_DYNAMIC_DATA | NX_DBG_EVENTMASK_BASIC_OBJECTS_STATIC_DATA);
		public static readonly uint NX_DBG_EVENTMASK_JOINTS							=(NX_DBG_EVENTGROUP_JOINTS | NX_DBG_EVENTMASK_BASIC_OBJECTS);
		public static readonly uint NX_DBG_EVENTMASK_JOINTS_DATA					=(NX_DBG_EVENTGROUP_JOINTS | NX_DBG_EVENTGROUP_JOINTS_DATA |NX_DBG_EVENTMASK_BASIC_OBJECTS);
		public static readonly uint NX_DBG_EVENTMASK_CONTACTS						=(NX_DBG_EVENTGROUP_CONTACTS | NX_DBG_EVENTMASK_BASIC_OBJECTS);
		public static readonly uint NX_DBG_EVENTMASK_CONTACTS_DATA					=(NX_DBG_EVENTGROUP_CONTACTS | NX_DBG_EVENTGROUP_CONTACTS_DATA | NX_DBG_EVENTMASK_BASIC_OBJECTS);
		public static readonly uint NX_DBG_EVENTMASK_TRIGGERS						=(NX_DBG_EVENTGROUP_TRIGGERS);
		public static readonly uint NX_DBG_EVENTMASK_PROFILING						=(NX_DBG_EVENTGROUP_PROFILING);



		protected IntPtr nxRemoteDebuggerPtr;

		public NxRemoteDebugger(IntPtr remoteDebuggerPointer)
			{nxRemoteDebuggerPtr=remoteDebuggerPointer;}

		public static NxRemoteDebugger createFromPointer(IntPtr remoteDebuggerPointer)
		{
			if(remoteDebuggerPointer==IntPtr.Zero)
				{return null;}
			return new NxRemoteDebugger(remoteDebuggerPointer);
		}

		public IntPtr NxRemoteDebuggerPtr
			{get{return nxRemoteDebuggerPtr;}}







		virtual public void connect(string host)
			{connect(host,NX_DBG_DEFAULT_PORT,NX_DBG_EVENTMASK_EVERYTHING);}

		virtual public void connect(string host,uint port)
			{connect(host,port,NX_DBG_EVENTMASK_EVERYTHING);}

		virtual public void connect(string host,uint port,uint eventMask)
			{wrapper_RemoteDebugger_connect(nxRemoteDebuggerPtr,host,port,eventMask);}

		virtual public void flush()
			{wrapper_RemoteDebugger_flush(nxRemoteDebuggerPtr);}

		virtual public Vector3 getPickPoint()
		{
			Vector3 pickPoint=new Vector3(0,0,0);
			wrapper_RemoteDebugger_getPickPoint(nxRemoteDebuggerPtr,ref pickPoint);
			return pickPoint;
		}
		
		virtual public uint getMask()
			{return wrapper_RemoteDebugger_getMask(nxRemoteDebuggerPtr);}


		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RemoteDebugger_connect(IntPtr remoteDebugger,string host,uint port,uint eventMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RemoteDebugger_flush(IntPtr remoteDebugger);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_RemoteDebugger_isConnected(IntPtr remoteDebugger);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RemoteDebugger_frameBreak(IntPtr remoteDebugger);
	
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RemoteDebugger_getPickPoint(IntPtr remoteDebugger,ref Vector3 pickPoint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_RemoteDebugger_getMask(IntPtr remoteDebugger);
	}
}

/*
X	virtual void connect(const char* host, unsigned int port = NX_DBG_DEFAULT_PORT, NxU32 eventMask = NX_DBG_EVENTMASK_EVERYTHING) = 0;
-	virtual void flush() = 0;
-	virtual bool isConnected() = 0;
-	virtual void frameBreak() = 0;
	virtual void createObject(void *object, NxRemoteDebuggerObjectType type, const char *className, NxU32 mask) = 0;
	virtual void removeObject(void *object, NxU32 mask) = 0;
	virtual void addChild(void *object, void *child, NxU32 mask) = 0;
	virtual void removeChild(void *object, void *child, NxU32 mask) = 0;
	virtual void writeParameter(const NxReal &parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void writeParameter(const NxU32 &parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void writeParameter(const NxVec3 &parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void writeParameter(const NxPlane &parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void writeParameter(const NxMat34 &parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void writeParameter(const NxMat33 &parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void writeParameter(const NxU8 *parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void writeParameter(const char *parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void writeParameter(const bool &parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void writeParameter(const void *parameter, void *object, bool create, const char *name, NxU32 mask) = 0;
	virtual void *getPickedObject() = 0;
-	virtual NxVec3 getPickPoint() = 0;
-	virtual NxU32 getMask() = 0;
*/




