//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	public delegate NxControllerAction OnShapeHitDelegate(NxControllerShapeHit hit);
	public delegate NxControllerAction OnControllerHitDelegate(NxControllersHit hit);

/*	
	NxController*	controller;		//!< Current controller
	NxShape*		shape;			//!< Touched shape
	NxExtendedVec3	worldPos;		//!< Contact position in world space
	NxVec3			worldNormal;	//!< Contact normal in world space
	NxU32			id;				//!< Feature identifier (e.g. triangle index for meshes)
	NxVec3			dir;			//!< Motion direction
	NxF32			length;			//!< Motion length
*/
	
	[StructLayout(LayoutKind.Sequential)]
	public class NxControllerShapeHit
	{
		public IntPtr	controllerPtr;	//!< Current controller	//Originally NxController*
		public IntPtr	shapePtr;		//!< Touched shape
		private double	worldPosX;		//!< Contact position in world space
		private double	worldPosY;			//On the native side there is a single NxExtendedVec3 for worldPos. NxExtendedVec3 is made of doubles
		private double	worldPosZ;
		public Vector3	worldNormal;	//!< Contact normal in world space
		public uint		id;				//!< Feature identifier (e.g. triangle index for meshes)
		public Vector3	dir;			//!< Motion direction
		public float	length;			//!< Motion length

		public NxShape Shape
			{get{return NxShape.createFromPointer(shapePtr);}}

		public NxController Controller
			{get{return NxController.createFromPointer(controllerPtr);}}

		//This converts the double values to a Vector3
		public Vector3 WorldPos
		{
			get{return new Vector3((float)worldPosX,(float)worldPosY,(float)worldPosZ);}
			set{worldPosX=(double)value.X; worldPosY=(double)value.Y; worldPosZ=(double)value.Z;}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public class NxControllersHit
	{
		public IntPtr controllerPtr;	//!< Current controller	//Originally NxController*
		public IntPtr otherPtr;			//!< Touched controller	//Originally NxController*
		
		public NxController Controller
			{get{return NxController.createFromPointer(controllerPtr);}}

		public NxController Other
			{get{return NxController.createFromPointer(otherPtr);}}
	}
	
	

	abstract unsafe public class NxUserControllerHitReport
	{
		static ArrayList userControllerHitReportList=new ArrayList();

		private IntPtr nxUserControllerHitReportPtr=IntPtr.Zero;
		private OnShapeHitDelegate onShapeHitDelegate=null;
		private OnControllerHitDelegate onControllerHitDelegate=null;


		public NxUserControllerHitReport()
			{create();}

		~NxUserControllerHitReport()
			{destroy();}


		public IntPtr NxUserControllerHitReportPtr
			{get{return nxUserControllerHitReportPtr;}}


		private void create()
		{
			if(!userControllerHitReportList.Contains(this))
			{
				setCallbacks();
				nxUserControllerHitReportPtr=wrapper_UserControllerHitReport_create(onShapeHitDelegate,onControllerHitDelegate);
				userControllerHitReportList.Add(this);
			}
		}
		
		private void destroy()
		{
			if(userControllerHitReportList.Contains(this))
			{
				userControllerHitReportList.Remove(this);
				wrapper_UserControllerHitReport_destroy(nxUserControllerHitReportPtr);
			}
			nxUserControllerHitReportPtr=IntPtr.Zero;
		}
		
		static public NxUserControllerHitReport getFromPointer(IntPtr reportPtr)
		{
			foreach(NxUserControllerHitReport hitReport in userControllerHitReportList)
			{
				if(hitReport.nxUserControllerHitReportPtr==reportPtr)
					{return hitReport;}
			}
			return null;
		}
		

		private void setCallbacks()
		{
			onShapeHitDelegate=new OnShapeHitDelegate(this.onShapeHit);
			onControllerHitDelegate=new OnControllerHitDelegate(this.onControllerHit);
		}


		public abstract NxControllerAction onShapeHit(NxControllerShapeHit hit);
		public abstract NxControllerAction onControllerHit(NxControllersHit hit);


		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_UserControllerHitReport_create(OnShapeHitDelegate onShapeHitDelegate,OnControllerHitDelegate onControllerHitDelegate);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_UserControllerHitReport_destroy(IntPtr userControllerHitReportPtr);
	}
}

