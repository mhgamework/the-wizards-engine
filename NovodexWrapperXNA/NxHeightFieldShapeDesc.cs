//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxHeightFieldShapeDesc : NxShapeDesc
	{
		public IntPtr	heightFieldPtr;			//References the height field that we want to instance.
		public float	heightScale;			//Multiplier to transform sample height values to shape space y coordinates.
		public float	rowScale;				//Multiplier to transform height field rows to shape space x coordinates.
		public float	columnScale;			//Multiplier to transform height field columns to shape space z coordinates.
		public ushort	materialIndexHighBits;	//The high 9 bits of this number are used to complete the material indices in the samples. The remaining low 7 bits must be zero.
		public ushort	holeMaterial;			//The the material index that designates holes in the height field. This number is compared directly to sample materials. Consequently the high 9 bits must be zero.
		public uint		meshFlags;				//Combination of enum NxMeshShapeFlag
		
		
		public static NxHeightFieldShapeDesc Default
			{get{return new NxHeightFieldShapeDesc();}}

		public NxHeightFieldShapeDesc()
			{setToDefault();}

		public NxHeightFieldShapeDesc(NxHeightField heightField,float heightScale,float rowScale,float columnScale,ushort materialIndexHighBits,ushort holeMaterial,uint meshFlags)
		{
			setToDefault();
			this.HeightField=heightField;
			this.heightScale=heightScale;
			this.rowScale=rowScale;
			this.columnScale=columnScale;
			this.materialIndexHighBits=materialIndexHighBits;
			this.holeMaterial=holeMaterial;
			this.meshFlags=meshFlags;
		}

		override public void setToDefault()
		{
			base.setToDefault();
			heightFieldPtr			= IntPtr.Zero;
			heightScale				= 1.0f;
			rowScale				= 1.0f;
			columnScale				= 1.0f;
			materialIndexHighBits	= 0;
			holeMaterial			= 0;
			meshFlags				= 0;
			type=NxShapeType.NX_SHAPE_HEIGHTFIELD;
		}

		public NxHeightField HeightField
		{
			get{return NxHeightField.createFromPointer(heightFieldPtr);}
			set
			{
				if(value==null)
					{heightFieldPtr=IntPtr.Zero;}
				else
					{heightFieldPtr=value.NxHeightFieldPtr;}
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

