//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxTriangleMeshShapeDesc : NxShapeDesc
	{
		public IntPtr			meshDataPtr;	//!< References the triangle mesh that we want to instance.
		public uint				meshFlags;		//!< Combination of ::NxMeshShapeFlag
		public float			scale;   		//not used yet

		public static NxTriangleMeshShapeDesc Default
			{get{return new NxTriangleMeshShapeDesc();}}
		
		public NxTriangleMeshShapeDesc()
			{setToDefault();}
		
		public override void setToDefault()
		{
			base.setToDefault();
			meshDataPtr	= IntPtr.Zero;
			meshFlags	= 0;
			scale		= 1.0f;
			type=NxShapeType.NX_SHAPE_MESH;
		}
		
		public NxTriangleMesh MeshData
		{
			get{return NxTriangleMesh.createFromPointer(meshDataPtr);}
			set
			{
				if(value==null)
					{meshDataPtr=IntPtr.Zero;}
				else
					{meshDataPtr=value.NxTriangleMeshPtr;}
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









