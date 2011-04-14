//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	public class NxContactStreamIterator
	{
		protected IntPtr nxContactStreamIteratorPtr;
		
		public NxContactStreamIterator(IntPtr stream)
			{nxContactStreamIteratorPtr=wrapper_ContactStreamIterator_create(stream);}

		~NxContactStreamIterator()
			{wrapper_ContactStreamIterator_delete(nxContactStreamIteratorPtr);}
		



		public IntPtr NxContactStreamIteratorPtr
			{get{return nxContactStreamIteratorPtr;}}






		virtual public bool goNextPair()
			{return wrapper_ContactStreamIterator_goNextPair(nxContactStreamIteratorPtr);}

		virtual public bool goNextPatch()
			{return wrapper_ContactStreamIterator_goNextPatch(nxContactStreamIteratorPtr);}

		virtual public bool goNextPoint()
			{return wrapper_ContactStreamIterator_goNextPoint(nxContactStreamIteratorPtr);}

		virtual public int getNumPairs()
			{return (int)wrapper_ContactStreamIterator_getNumPairs(nxContactStreamIteratorPtr);}

		virtual public NxShape getShape(int shapeIndex)
		{
			IntPtr shapePtr=wrapper_ContactStreamIterator_getShape(nxContactStreamIteratorPtr,(uint)shapeIndex);
			return NxShape.createFromPointer(shapePtr);
		}

		virtual public ushort getShapeFlags()
			{return wrapper_ContactStreamIterator_getShapeFlags(nxContactStreamIteratorPtr);}

		virtual public int getNumPatches()
			{return (int)wrapper_ContactStreamIterator_getNumPatches(nxContactStreamIteratorPtr);}

		virtual public int getNumPatchesRemaining()
			{return (int)wrapper_ContactStreamIterator_getNumPatchesRemaining(nxContactStreamIteratorPtr);}

		virtual public Vector3 getPatchNormal()
		{
			Vector3 patchNormal;
			wrapper_ContactStreamIterator_getPatchNormal(nxContactStreamIteratorPtr,out patchNormal);
			return patchNormal;
		}

		virtual public int getNumPoints()
			{return (int)wrapper_ContactStreamIterator_getNumPoints(nxContactStreamIteratorPtr);}

		virtual public uint getNumPointsRemaining()
			{return wrapper_ContactStreamIterator_getNumPointsRemaining(nxContactStreamIteratorPtr);}

		virtual public Vector3 getPoint()
		{
			Vector3 point;
			wrapper_ContactStreamIterator_getPoint(nxContactStreamIteratorPtr,out point);
			return point;
		}
		
		virtual public float getSeparation()
			{return wrapper_ContactStreamIterator_getSeparation(nxContactStreamIteratorPtr);}

		virtual public uint getFeatureIndex0()
			{return wrapper_ContactStreamIterator_getFeatureIndex0(nxContactStreamIteratorPtr);}

		virtual public uint getFeatureIndex1()
			{return wrapper_ContactStreamIterator_getFeatureIndex1(nxContactStreamIteratorPtr);}

		virtual public uint getExtData()
			{return wrapper_ContactStreamIterator_getExtData(nxContactStreamIteratorPtr);}

		virtual public float getPointNormalForce()
			{return wrapper_ContactStreamIterator_getPointNormalForce(nxContactStreamIteratorPtr);}



		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_ContactStreamIterator_create(IntPtr stream);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_ContactStreamIterator_delete(IntPtr contactStreamIterator);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_ContactStreamIterator_goNextPair(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_ContactStreamIterator_goNextPatch(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_ContactStreamIterator_goNextPoint(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ContactStreamIterator_getNumPairs(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_ContactStreamIterator_getShape(IntPtr contactStreamIterator,uint shapeIndex);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern ushort wrapper_ContactStreamIterator_getShapeFlags(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ContactStreamIterator_getNumPatches(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ContactStreamIterator_getNumPatchesRemaining(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_ContactStreamIterator_getPatchNormal(IntPtr contactStreamIterator,out Vector3 patchNormal);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ContactStreamIterator_getNumPoints(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ContactStreamIterator_getNumPointsRemaining(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_ContactStreamIterator_getPoint(IntPtr contactStreamIterator,out Vector3 point);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_ContactStreamIterator_getSeparation(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ContactStreamIterator_getFeatureIndex0(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ContactStreamIterator_getFeatureIndex1(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ContactStreamIterator_getExtData(IntPtr contactStreamIterator);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_ContactStreamIterator_getPointNormalForce(IntPtr contactStreamIterator);
	}
}


/*
-	NX_INLINE NxContactStreamIterator(NxConstContactStream stream);

-	NX_INLINE bool goNextPair();	//!< Goes on to the next pair, silently skipping invalid pairs.  Returns false if there are no more pairs.  Note that getNumPairs() also includes invalid pairs in the count.
-	NX_INLINE bool goNextPatch();	//!< Goes on to the next patch (contacts with the same normal).  Returns false if there are no more.
-	NX_INLINE bool goNextPoint();	//!< Goes on to the next contact point.  Returns false if there are no more.
-	NX_INLINE NxU32 getNumPairs();	//!< May be called at any time.  Returns the number of pairs in the structure.  Note: now some of these pairs may be marked invalid using getShapeFlags() & NX_SF_IS_INVALID, so the effective number may be lower.  goNextPair() will automatically skip these!
-	NX_INLINE NxShape * getShape(NxU32 shapeIndex);
-	NX_INLINE NxU16 getShapeFlags();	//!< may be called after goNextPair() returned true
-	NX_INLINE NxU32 getNumPatches();	//!< may be called after goNextPair() returned true
-	NX_INLINE NxU32 getNumPatchesRemaining();//!< may be called after goNextPair() returned true
-	NX_INLINE const NxVec3 & getPatchNormal();	//!< may be called after goNextPatch() returned true
-	NX_INLINE NxU32 getNumPoints();//!< may be called after goNextPatch() returned true
-	NX_INLINE NxU32 getNumPointsRemaining();//!< may be called after goNextPatch() returned true
-	NX_INLINE const NxVec3 & getPoint();
-	NX_INLINE NxReal getSeparation();
-	NX_INLINE NxU32 getFeatureIndex0();
-	NX_INLINE NxU32 getFeatureIndex1();
-	NX_INLINE NxU32 getExtData();
-	NX_INLINE NxReal getPointNormalForce();
*/
