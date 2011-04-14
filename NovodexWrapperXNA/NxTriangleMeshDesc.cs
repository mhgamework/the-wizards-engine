//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxTriangleMeshDesc : NxSimpleTriangleMesh
	{
		public int					materialIndexStride;
		public IntPtr				materialIndicesPtr;
		public NxHeightFieldAxis	heightFieldVerticalAxis;
		public float				heightFieldVerticalExtent;
		public NxPMap				pmap;
		public float				convexEdgeThreshold;


		protected ushort[] internalMaterialIndiceArray=null;		//this in case the materialIndiceArray that the user passed in is deleted before the object is created


		public static NxTriangleMeshDesc Default
			{get{return new NxTriangleMeshDesc();}}

		public NxTriangleMeshDesc()
			{setToDefault();}

		public override void setToDefault()
		{
			base.setToDefault();
			
			materialIndexStride			= 0;
			materialIndicesPtr			= IntPtr.Zero;
			heightFieldVerticalAxis		= NxHeightFieldAxis.NX_NOT_HEIGHTFIELD;
			heightFieldVerticalExtent	= 0;
			convexEdgeThreshold			= 0.001f;
			pmap						= null;
		}
		
		
		
		
		
		//materialArray.Length should match numTriangles
		public void setMaterialIndices(ushort[] materialIndiceArray,bool copyToInternalArray)
		{
			if(materialIndiceArray==null)
			{
				internalMaterialIndiceArray=null;
				materialIndicesPtr=IntPtr.Zero;
				return;
			}
			
			internalMaterialIndiceArray=materialIndiceArray;

			if(copyToInternalArray)
			{
				int length=materialIndiceArray.Length;
				internalMaterialIndiceArray=new ushort[length];
				for(int i=0;i<length;i++)
					{internalMaterialIndiceArray[i]=materialIndiceArray[i];}
			}
			else
				{internalMaterialIndiceArray=materialIndiceArray;}
			
			unsafe
			{
				materialIndexStride=sizeof(ushort);
				fixed(void* p=&internalMaterialIndiceArray[0])
					{materialIndicesPtr=new IntPtr(p);}
			}
		}
	}
}




