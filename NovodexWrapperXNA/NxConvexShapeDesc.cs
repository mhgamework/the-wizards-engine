//By Jason Zelsnack, All rights reserved
 
using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxConvexShapeDesc : NxShapeDesc
	{
		public IntPtr			meshDataPtr;	//!< References the triangle mesh that we want to instance.
		public uint				meshFlags;		//!< Combination of ::NxMeshShapeFlag
		public float			scale;			//not used yet

		public static NxConvexShapeDesc Default
			{get{return new NxConvexShapeDesc();}}
		
		public NxConvexShapeDesc()
			{setToDefault();}
		
		public override void setToDefault()
		{
			base.setToDefault();
			meshDataPtr	= IntPtr.Zero;
			meshFlags	= 0;
			scale		= 1.0f;
			type		= NxShapeType.NX_SHAPE_CONVEX;
		}
		
		public NxConvexMesh MeshData
		{
			get{return NxConvexMesh.createFromPointer(meshDataPtr);}
			set
			{
				if(value==null)
					{meshDataPtr=IntPtr.Zero;}
				else
					{meshDataPtr=value.NxConvexMeshPtr;}
			}
		}
		
		
		
		public bool FlagMeshSmoothSphereCollisions
		{
			get{return NovodexUtil.areBitsSet(meshFlags,(uint)NxMeshShapeFlag.NX_MESH_SMOOTH_SPHERE_COLLISIONS);}
			set{meshFlags=NovodexUtil.setBits(meshFlags,(uint)NxMeshShapeFlag.NX_MESH_SMOOTH_SPHERE_COLLISIONS,value);}
		}

		public bool FlagMeshDoubleSided
		{
			get{return NovodexUtil.areBitsSet(meshFlags,(uint)NxMeshShapeFlag.NX_MESH_DOUBLE_SIDED);}
			set{meshFlags=NovodexUtil.setBits(meshFlags,(uint)NxMeshShapeFlag.NX_MESH_DOUBLE_SIDED,value);}
		}
	}
}

